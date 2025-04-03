SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[dn_getacctsforalloc_sp]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[dn_getacctsforalloc_sp]
GO

CREATE procedure dn_getacctsforalloc_sp
(
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : dn_getacctsforalloc_sp.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get accounts for allocation
-- Author       : ???
-- Date         : ???
--
--  
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 11/07/07	Jec livewire 69115 incl. ensuring all date ranges use correct from and to timestamp
-- 08/11/07 Jec UAT 53 Status Code check not working when NL selected.	

--------------------------------------------------------------------------------
    -- Parameters
	@alreadyallocated varchar (3),
	@minstatus varchar(2)= '',
	@currstatus varchar (2),
	@employeetype varchar(15),
	@datestartallocated datetime,
	@datefinishallocated datetime,
	@actionchoice varchar (3),--to choose whether action selection is on date due or date of action
	@actionstart datetime,--to choose data  start of any action
	@actionend  datetime,--to choose date of end of any action
	@minarrears money,
	@statustype  varchar (12),--status  equals  single status  or "between" certain status codes
	@Maxarrears money,
	@arrearschoice varchar (3),-- 1 for arrears > 0  other choices for arrears <  or >certain amount
	@arrears money,
	@actioncode  varchar (6),--to choose whether a certain action has taken  place against  an account
	@lettercode  varchar (10),
	@letterRestriction  varchar (4),
	@letterradio  bit,--to choose whether dates are due  or sent
	@letterstart datetime,
	@letterfinish datetime,
	@actionrestriction varchar (4),--AA action for any employee, AE action for this employee NE no action for this employee NA no action for any employee
	@actiondatestart datetime,	-- This is a duplicated param of @actionstart
	@actiondatefinish datetime,	-- This is a duplicated param of @actionend
	@includephone bit,
	@includeaddresses bit,
	@return integer output,
	@empeeno integer,
	@empeetype varchar(4),
	@branch  integer,
	@branchset varchar(32),
	@acctnobranch integer= 0,
	@proppoints smallint, --if > 0 then will be selecting points from proposal
	@proppointsdirection char(2)= '', --> or < 
	@coderestriction varchar(3)='NR', --account or customer code
	@code varchar (8)= '',
	@addresssearch varchar (30) = ' ',
	@pocodesearch varchar (6) = '',
	@viewtop bit,
	@accountbranch varchar(4),
	@rowlimited bit out,
	@user integer,
	@actionsfilter smallint, -- number of actions to filter and not paid in the last month
	@actionsoperand varchar(1), -- > or < or =  if ='' then don't filter
	@actionsthisempeeno smallint, -- are the number of actions for the allocated employee or for all
	@balance money, -- money balance to restrict on
	@balanceoperand char(1), -- > or < if blank wont restrict
	@arrearsinccharges smallint, -- 1 or 0 true or false
	@datemovedarrsfrom datetime, -- arrears movement date from
	@datemovedarrsto datetime, -- arrears movement date to
	@datemovedrestriction varchar(2), -- 'NL' no limitation 'AM' Accounts moving into arrears,'NA' new accounts in arrears
	@datelastpaid datetime, -- restrict by acct.datelastpaid
	@lastpaidoperand char(1), -- '>' or '<'  if blank no restriction
	@actionDueDate  bit,        -- use date added or date due -Bailaction   jec 67902  
	@credit bit,					-- CR813 RD 05/09/06
	@cash bit,						-- CR813 RD 05/09/06
	@service bit = 0				-- CR802 PC 16/Nov/06
)
AS 

declare
	@columns varchar (2000),
	@top varchar(12),
	@maxrow int ,
	@where varchar (2500),
	@tables varchar (300),
	@statement SQLText,
	@final_statement SQLText,
	@debug smallint

set @debug =0 -- FOR debugging 
set nocount on
set @return =0
--setting these days to start of Day
--set @datestartallocated=dateadd(hour,-datepart(hour ,@datestartallocated),@datestartallocated)	removed jec 11/07/07 69115
--set @datestartallocated=dateadd(minute,-datepart(minute ,@datestartallocated),@datestartallocated)removed jec 11/07/07 69115
--set @actiondatestart=dateadd(hour,-datepart(hour ,@actiondatestart),@actiondatestart)		removed jec 11/07/07 69115
--set @actiondatestart=dateadd(minute,-datepart(minute ,@actiondatestart),@actiondatestart)	removed jec 11/07/07 69115

set @where = ''
--setting inclusive dates
-- 68180 - RD 16/05/2006 Modified as was adding 23 hours to current date
-- For example 
-- Datefinishallocated = 'Apr 30 2006  5:32:58:000PM' was being changed to  'May  1 2006  5:31PM'
-- Changed so datefinishallocated will update to 'Apr 30 2006 11:59PM'

--
-- Removing time element from dates
set @datefinishallocated = CONVERT(DATETIME,convert(char(12),@datefinishallocated,103),103)
-- Adding 1 day and then taking off one second to get to end of previous day
set @datefinishallocated = CONVERT(DATETIME,DATEADD(day,1,DATEADD(second,-1,@datefinishallocated)),103)

-- 68180 - RD 16/05/2006
-- Removing time element from date
set @actiondatefinish = CONVERT(DATETIME,convert(char(12),@actiondatefinish,103),103)
set @actionend = CONVERT(DATETIME,convert(char(12),@actionend,103),103)		-- duplicated parameter
-- Adding 1 day and then taking off one second to get to end of "to date" day
set @actiondatefinish = CONVERT(DATETIME,DATEADD(day,1,DATEADD(second,-1,@actiondatefinish)),103)
set @actionend = CONVERT(DATETIME,DATEADD(day,1,DATEADD(second,-1,@actionend)),103)	-- duplicated parameter

-- 69115 - jec 11/07/07 (for all date parameters)
-- Removing time element from date
set @datestartallocated = CONVERT(DATETIME,convert(char(12),@datestartallocated,103),103)
set @actiondatestart = CONVERT(DATETIME,convert(char(12),@actiondatestart,103),103)
set @actionstart = CONVERT(DATETIME,convert(char(12),@actionstart,103),103)		-- duplicated parameter
set @letterstart = CONVERT(DATETIME,convert(char(12),@letterstart,103),103)
set @datemovedarrsfrom = CONVERT(DATETIME,convert(char(12),@datemovedarrsfrom,103),103)
set @datelastpaid = CONVERT(DATETIME,convert(char(12),@datelastpaid,103),103)
set @letterfinish = CONVERT(DATETIME,convert(char(12),@letterfinish,103),103)
set @datemovedarrsto = CONVERT(DATETIME,convert(char(12),@datemovedarrsto,103),103)

-- subtracting one second to get to end of previous "from date" day
set @datestartallocated = CONVERT(DATETIME,DATEADD(second,-1,@datestartallocated),103)
set @actiondatestart = CONVERT(DATETIME,DATEADD(second,-1,@actiondatestart),103)
set @actionstart = CONVERT(DATETIME,DATEADD(second,-1,@actionstart),103)
set @letterstart = CONVERT(DATETIME,DATEADD(second,-1,@letterstart),103)
set @datemovedarrsfrom = CONVERT(DATETIME,DATEADD(second,-1,@datemovedarrsfrom),103)
-- set date to 1 second before midnight if operand is > otherwise selection will not include 00:00:00 time
if @lastpaidoperand='>'
	set @datelastpaid = CONVERT(DATETIME,DATEADD(second,-1,@datelastpaid),103)
-- Adding 1 day and then taking off one second to get to end of "to date" day
set @letterfinish = CONVERT(DATETIME,DATEADD(day,1,DATEADD(second,-1,@letterfinish)),103)
set @datemovedarrsto = CONVERT(DATETIME,DATEADD(day,1,DATEADD(second,-1,@datemovedarrsto)),103)



/*columns required are arrears, instalment, balance, first name, name, address, account number, status,
percentage paid-*/

-- create the table if not exist - zz_AccountReportsScript for storing @final_statement sql script
-- jec 67902 useful for debugging
  --68629 removed 
-- debugging
--IF not EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
--	   WHERE  table_name = 'zz_AccountReportsScript')
--create table .dbo.zz_AccountReportsScript
--(
--ScriptDateTime datetime,
--Script varchar(100)
--)



CREATE TABLE #allocquery (acctno varchar (12),
						  currstatus varchar (1),
						  datelastpaid DateTime,
                     outstbal money,
                     Arrears money,
                     agrmttotal money,
                     paidpcent float,
                     bdwbalance money,
                     bdwcharges money,
  		     arrearsexcharges money,
          	     arrearslevel float,
                     Title varchar (25),
                     Name varchar (60),
                     ethnicity varchar (6),
                     FirstName varchar (30),
                     cusaddr1 varchar (50),
                     cusaddr2 varchar (50),
                     cusaddr3 varchar (50), 
                     postcode varchar (10),
                     instalamount money,
           	     dueday smallint,
                     arrearsinst float,
                     hldorjnt char(1),
          	     custid varchar (20),
                     numaccs smallint,
                     cashLoan BIT ,                -- CR906 JH 
                    datestatchge Datetime,             --  jec 67911 
   		     empeeno integer,
                     allocno smallint,
                     dateAlloc DateTime,
                     datedeAlloc DateTime,
                     bailfee money,
                     allocprtflag varchar (2),
                     actionno smallint,
			SR varchar(20) -- pc CR802                  
		    )
 
declare @nl varchar(13)
set @nl = '
 '
set nocount  on
--here we are going to restrict the initial select 
   set @maxrow =15000
   select @maxrow =isnull(maxrow,0) from courtsperson where UserId =@user
   if @maxrow =0
     set @maxrow =10000
  
   set @top = isnull(convert (varchar(10),@maxrow),'900')

-- UAT 338 For Service all data is required in #allocquery
IF @service = 1
BEGIN
	SET @top = '100 percent'
END

-- 67945 RD 08/02/06 Removed instalamount and dueday, these will be updated later
-- 67911 jec 07/04/06 Added date status code changed
set @columns = ' insert into #allocquery select  top ' + @top +  
   ' acct.acctno, acct.currstatus,acct.datelastpaid,' + @nl +
    ' acct.outstbal, acct.arrears, acct.agrmttotal, ' + @nl +
    'acct.paidpcent, acct.bdwbalance, acct.bdwcharges, ' + @nl +
    ' acct.arrears,0, ' + 'convert (varchar (10), '''') as title, convert (varchar (30), '''') as name, ' + @nl +
    'convert (varchar (6), '''') as ethnicity, ' +   ' '''' as firstname, convert (varchar (50), '''') as cusaddr1, ' + @nl +
     'convert (varchar (50), '''') as cusaddr2, convert (varchar (50), '''') as cusaddr3 , ' + ' convert (varchar (10), '''') as postcode, ' + @nl +
     ' 0,0, convert (float,0) as arrearsinst, '+ @nl +
     ' custacct.hldorjnt, custacct.custid, 0 as numaccs,IsLoan AS cashLoan,status.datestatchge,  ' + @nl 

-- 67945 RD 08/02/06 Removed linked to instalplan
-- 67911 JEC 07/04/06 add link to status
set @tables = ' from acct,TermsTypeTable,custacct left outer join status on custacct.acctno = status.acctno
    and status.datestatchge = (Select max(datestatchge) from status where custacct.acctno=status.acctno)' --instalplan,
  
set @where = ' where custacct.acctno = acct.acctno and custacct.hldorjnt = ''H'' AND acct.termstype = TermsTypeTable.termstype ' + @nl --+
--        ' and instalplan.acctno =acct.acctno '
if @alreadyallocated = 'CP' --already allocated to courts person
begin
set @columns =@columns +
    ' follupalloc.empeeno, follupalloc.allocno, follupalloc.datealloc,' + @nl +
    ' follupalloc.datedealloc, follupalloc.bailfee,follupalloc.allocprtflag, 0, '''' '  + @nl
set @tables =@tables + ',follupalloc '

set @where =@where + ' and follupalloc.acctno = acct.acctno and follupalloc.datealloc > '''   + convert (varchar,@datestartallocated ) 
     + ' '' and follupalloc.datealloc <= '' ' + convert (varchar,@datefinishallocated) + 
     ''' and (follupalloc.datedealloc =''1-jan-1900'' or follupalloc.datedealloc is null) ' + @nl 

end
else
begin
set @columns =@columns +
    ' convert(integer, 0) as empeeno, convert (integer, 0) as allocno, ''1-jan-1900'' as datealloc, ' + @nl +
    ' convert (datetime, getdate()) as datedealloc, convert (money, 0) as bailfee,convert (varchar, '''') as allocprtflag, 0, '''' ' + @nl
set @where= @where + ' and not exists ( select acctno from follupalloc where follupalloc.acctno =acct.acctno ' +
     ' and  (follupalloc.datedealloc =''1-jan-1900'' or follupalloc.datedealloc is null)) '

end
if @lastpaidoperand !=''
   set @where = @where + ' and acct.datelastpaid ' + @lastpaidoperand + '''' + convert(varchar,@datelastpaid) + '''' + @nl 

if @accountbranch !='0%' and @accountbranch !='%' and @branchset =''
   set @where= @where + ' and acct.acctno like ''' + @accountbranch + ''' ' + @nl 

/* determines whether user has restricted select to those with letters */

	/* determines whether user has restricted select to those with specific actions */

--    'customer.custid = custacct.custid '
if @acctnobranch != 0 and @acctnobranch is not null and @branchset =''
begin
      set @where =@where + ' and acct.acctno like ''' + convert (varchar,@acctnobranch) + '%''  '
end
else
if @branchset !='' -- Load up branches from the set details screen
begin
    set @where = @where + ' and acct.branchno in ( '
    declare @setdata varchar(32),@counter int
    set @counter = 0
    DECLARE set_cursor CURSOR 
  	FOR SELECT isnull(data,'')
        from setdetails
	 where setname = @branchset

   OPEN set_cursor
   FETCH NEXT FROM set_cursor INTO @setdata

   WHILE (@@fetch_status <> -1)
   BEGIN
       IF (@@fetch_status <> -2)
       BEGIN
            set @counter = @counter + 1

            if @counter > 1
                set @where =@where + ','

            set @where =@where + @setdata

            IF @COUNTER >1000 --wont have more than 1000 items
               break    
       END
       FETCH NEXT FROM set_cursor INTO @setdata
   END
   CLOSE set_cursor
   DEALLOCATE set_cursor

    set @where = @where + ') '
end



-- CR813 RD 05/09/06 If credit selected
if @credit != 0 and @cash = 0
begin
      set @where =@where + ' and acct.accttype NOT IN (''C'', ''S'') '
end

-- if CR813 RD 05/09/06  Cash selected
if @credit = 0 and @cash != 0
begin
      set @where = @where + ' and acct.accttype = (''C'') '
end



if @proppoints> 0
begin
    set @tables =@tables + ',proposal '
    set @where =@where + ' and proposal.custid = custacct.custid and proposal.acctno = '+
        'custacct.acctno and proposal.points ' + @proppointsdirection +
        '   ' + convert (varchar,@proppoints)
end
   
    /* restric ting to the particular courtsperson(s) on here */
    if @alreadyallocated='CP'
    begin
    
       set @tables =@tables + ' ,courtsperson '
       set @where=@where + ' and follupalloc.empeeno =courtsperson.empeeno and courtsperson.empeetype = ' +
                                           ' ''' + @empeetype + ''' '
    
    
      if @empeeno !=0
       
         set @where=@where + ' and courtsperson.empeeno =' + convert (varchar,@empeeno)
    
    end
    
    
    /* The list items are: 1=BLANK 2=ALL 3=ALL BY BRANCH.
    ** DSR 28/02/01 Revisiting FR1175 after SQL Server migration.
    */

    
    
/* restricting the selection by status code - either between or a single status code */
    if @statustype !='NL'		-- jec 08/11/07	    if @currstatus !='NL'
    begin
      if @statustype !='Between'
		  begin
    		set @where =@where + ' and acct.currstatus = ''' + @currstatus + '''' 
			if @currstatus = 'S' --account settled but written off so could do with more chasing  -- jec 08/11/07	
			begin
			   set @where = @where + ' and exists (select acctno from fintrans where transtypecode = ''BDW'' ' +
			   ' and fintrans.acctno =acct.acctno ) '
			end
		  end
		  else
		   set @where =@where + ' and acct.currstatus between ''' + @minstatus + ''' and ''' +
			  @currstatus  + ''''
		   if @currstatus = 'S' --account settled but written off so could do with more chasing
		   begin
			   set @where = @where + ' and exists (select acctno from fintrans where transtypecode = ''BDW'' ' +
			   ' and fintrans.acctno =acct.acctno ) '
		   end
	      end
	  else
		BEGIN
			set @where =@where + ' and acct.currstatus != ''S'' '
		END
/* choosing using arrears - either high for immediate attention or low arrears perhaps for deallocation
    if @arrearschoice != 'NL' 
    	set @where =@where + ' and acct.arrears > 0 '
    else
*/
    if @arrearschoice != 'NL' 
    begin
         if  @arrearschoice = '>'
   	     set @where =@where + ' and acct.arrears > ' +  convert(varchar, @arrears)
         else
           if   @arrearschoice= '<'  
             set @where =@where  + ' and acct.arrears < '  +  convert(varchar, @arrears)
    end
/* if users wants to choose those accounts with letters against them */
    if @letterRestriction != 'NR'
     begin
    	 set @where =@where  + ' and exists (select * from letter where letter.acctno = acct.acctno and  ' 
    	if @letterradio = 1 
    		  set @where =@where + ' letter.dateacctlttr > ''' + convert (varchar,@letterstart) + 
                   ''' and letter.dateacctlttr < ''' + convert (varchar,@letterfinish) + ''' '
    	else 
                set @where =@where +  ' letter.datedue > ''' + convert (varchar,@letterstart)  +
                   ''' and letter.datedue <''' + convert (varchar,@letterfinish) + '''  '
  

/* if users wants those with specific letters  */
			if @letterRestriction = 'SL'
			   set @where =@where + ' and letter.lettercode = ''' +   @lettercode  + '''  '

		  set @where =@where + ') '
    end


/* if users wants those with specific actions eg. Promise to pay */
if @actionrestriction != 'NR'  and ( @alreadyallocated = 'CP' or @actioncode ='PAY' )
begin
    declare @not varchar (6),@table varchar (32),@datecolumn varchar (32),@partcolumn varchar (12)
    if @actioncode !='PAY' or @actionrestriction = 'AE' or  @actionrestriction ='NE' 
    begin
       set @table = 'bailaction'
       set @datecolumn = 'dateadded'
       set @partcolumn = '.'
        if @actionDueDate = 1
            set @datecolumn = 'datedue'    -- jec 67902
    end
    else
    begin
       set @table = 'fintrans'
       set @datecolumn = 'datetrans'
       set @partcolumn = '.transtype'
    end      
    if @actionrestriction ='NE' or @actionrestriction ='NA' 
        set @not ='not'
    else
        set @not =''
	 set @where  =@where + ' and ' + @not + ' exists (select * from ' +@table + ' where ' +@table + '.acctno =	acct.acctno and  ' +
          ' ' + @table + @partcolumn + 'code =  ''' + @actioncode + ''' ' 
  
  if @actionrestriction = 'AE' or  @actionrestriction ='NE'  /* [no] action for this employee only */
	  set @where =@where  + ' and ' +@table + '.empeeno =follupalloc.empeeno '
 
 
	set @where =@where + ' and ' +@table + '.' +@datecolumn + '  > ''' +  convert (varchar,@actionstart)
		 + ''' and ' +@table + '.' +@datecolumn + '  < ''' +  convert (varchar,@actionend)  + ''' )'

 end

  set  @final_statement =@columns + '
 ' + @tables + '
' + @where
  --select @final_statement
  --select @columns
  --select @tables
 --select @where  
--   print 'here'
--   print @final_statement
   if @debug =1
     print @final_statement

-- save SQL script for reference/debug  jec 67902
-- delete SQL data older than 30 days

--68629 removed  
-- debugging
--delete zz_AccountReportsScript
--    where scriptdatetime < getdate()-30
--
--declare @s smallint, @e smallint
--set @s=1 
--set @e=@s+79
--
----select @final_statement
---- 68629 removed this insert
--while @e< len(@final_statement)+ 79
--begin
--insert into zz_AccountReportsScript (scriptdatetime,script)
--select getdate(),substring(@final_statement,@s,80)
--set @s=@e+1
--set @e=@s+79
--end

-- end save of SQL script

   declare @rowcount integer
   execute sp_executeSQL  @final_statement
   set @rowcount =@@rowcount
  if @rowcount =@maxrow
  begin
  	 set @rowlimited=1
  end
  if @debug =1 
		print 'num rows ' + convert (varchar,@rowcount)

  if @coderestriction != 'NR' --NR = no restriction
     execute dn_getacctsforalloccodes_sp @code =@code,@coderestriction =@coderestriction


-- arrears excluding charges
    update #allocquery set arrearsexcharges = #allocquery.arrears - (isnull(acct.outstbal,0) - isnull(acct.as400bal, 0))   
	from #allocquery left outer join acct on #allocquery.acctno = acct.acctno
 
--    -isnull((select sum(transvalue) from fintrans where transtypecode in ('INT','ADM')
  --            and fintrans.acctno =#allocquery.acctno),0)


-- 67945 RD 08/02/06 Cash Accounts not loaded in Allocate Accounts screen
-- Update instalamount and dueday 
   update #allocquery 
   set 	  instalamount = i.instalamount,
	  dueday = convert(smallint,left(convert(varchar,i.datefirst,3),2))
   from   instalplan i 
   where i.acctno = #allocquery.acctno

  if @arrearsinccharges = 0
	update #allocquery set arrearslevel = arrears/instalamount where instalamount > 0
  else 
	update #allocquery set arrearslevel = arrearsexcharges/instalamount where instalamount > 0
	
  -- 68216 RD 28/06/06 Modifed to load the correct database based on the arrears selected
  if @arrearschoice ='A<I'
     delete from #allocquery where ROUND(arrearslevel,2) > @arrears

  if @arrearschoice= 'A>I'
     delete from #allocquery where ROUND(arrearslevel,2) < @arrears

  if @arrearschoice= 'A=I'
     delete from #allocquery where ROUND(arrearslevel,2) not between @arrears and @arrears 

  if @debug = 1 
   select 's1 ' + convert (varchar,count(*)) from  #allocquery
execute dn_customerandaddressdetailssp @table_name = '#allocquery',
@getaddress = 1,@gethomephone = 0,@getbasic = 1,@getallphones = 0
-- =0 
if @actionsoperand != '' 
begin
  if @actionsthisempeeno = 1 --number is only for this allocated person
   update #allocquery set actionno = 
      isnull((select count(*) from code c, bailaction b
       where c.code = b.code and c.category ='FUP' and c.reference != 1 
       and b.acctno =#allocquery.acctno and
       b.empeeno = #allocquery.empeeno
       and b.dateadded >dateadd(month,-1,getdate())),0)
  else
      update #allocquery set actionno = isnull((select count(*) from code c, bailaction b
       where c.code = b.code and c.category ='FUP' and c.reference != 1 
       and b.acctno =#allocquery.acctno
       and b.dateadded >dateadd(month,-1,getdate())),0)
      -- now we are removing
      set @statement =' delete from #allocquery where actionno !' + @actionsoperand + ' ' + convert(varchar,@actionsfilter)
      execute sp_executeSQL  @statement
end

  if @debug = 1 
   select 's2 ' + convert (varchar,count(*)) from  #allocquery

if @balanceoperand ='>' or @balanceoperand= '<'
begin
  set @statement =
  ' delete from #allocquery where outstbal !' + @balanceoperand
  + convert(varchar,@balance)
   execute sp_executeSQL  @statement
end

  if @debug = 1 
   select 's3 ' + convert (varchar, count(*)) from  #allocquery

if @datemovedrestriction !='NL' 
begin
  exec dn_getacctsforallochigherarrears 
             @datemovedarrsfrom = @datemovedarrsfrom,
             @datemovedarrsto =@datemovedarrsto ,
	     @datemovedrestriction = @datemovedrestriction
end
  if @debug = 1 
   select 's4 ' + convert (varchar,count(*)) from  #allocquery

-- delete the from the table where if service flag is on 
-- UAT 338 The primary charge to must be customer or deliverer
if @service = 1
BEGIN
	DELETE A
	FROM #allocquery  A LEFT OUTER JOIN 
		(SELECT SR.ServiceRequestNo, SR.AcctNo FROM SR_ServiceRequest SR INNER JOIN
		SR_Resolution SE ON SR.ServiceRequestNo = SE.ServiceRequestNo AND ISNULL(DateClosed, '1900-01-01') = '1900-01-01' AND (ChargeTo = 'CUS' OR ChargeTo = 'DEL')) SRJ ON SRJ.AcctNo = A.AcctNo
	WHERE SRJ.ServiceRequestNo IS NULL
END

-- update the srno field
UPDATE	A
SET		SR = dbo.fn_SRGetServiceRequestNo(SR.ServiceRequestNo)
FROM	#allocquery A JOIN 
	SR_ServiceRequest SR ON A.AcctNo = SR.AcctNo


if @viewtop = 0
  select a.acctno ,                   currstatus  as Status,
                     outstbal  as Balance,
                     Arrears ,
          		      ROUND(arrearslevel,2) as arrearslevel,
							arrearsexcharges,
                     paidpcent ,
                     instalamount ,
                     custid as CustomerId,
                     Title,
		     FirstName,
                     Name, 
	             cusaddr1 ,
                     cusaddr2,
                     cusaddr3 ,
                     postcode as cuspocode,
                     empeeno,
			     datelastpaid,
           		      dueday,
		     bdwbalance,
		     bdwcharges,
             datestatchge,                --  jec 67911 
			 SR	AS ServiceRequestNo,
			 cashLoan               -- CR906 JH
from #allocquery a 
else 
  select top 500 a.acctno ,                   currstatus as Status,
                     outstbal  as Balance,
                     Arrears ,
          		      ROUND(arrearslevel,2) as arrearslevel,
							arrearsexcharges,
                     paidpcent ,
                     instalamount ,
                     custid as CustomerId,
                     Title,
	     	     FirstName,
                     Name, 
                     cusaddr1 ,
                     cusaddr2,
                     cusaddr3 ,
                     postcode as cuspocode,
                     empeeno,
			 	   	     datelastpaid,
           		      dueday,
		     bdwbalance,
		     bdwcharges,
             datestatchge,               --  jec 67911 
			 SR AS ServiceRequestNo,
			 cashLoan               -- CR906 JH 
from #allocquery a 

GO


GRANT EXECUTE ON dn_getacctsforalloc_sp TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


