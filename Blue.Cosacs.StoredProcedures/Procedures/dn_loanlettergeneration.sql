

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[dn_loanlettergeneration]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[dn_loanlettergeneration]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON 
GO
-- ==========================================================================================================
-- Version		: 002
-- Project      : CoSACS .NET
-- File Name    : dn_loanlettergeneration.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Credit Collections - Generates legal letters
-- Author       :
-- Date         : 
---- This procedure generates loan letters.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/08/19 Zensar  Optimised the stored procedure for performance by  replacing * with 1 in all exist checks, 
--			max with order by and used NoLock hints.
-- ============================================================================================================

CREATE procedure [dbo].[dn_loanlettergeneration] @gen  integer ,
       @runno  integer,
       @lettercode varchar (10),
       @datestart         DateTime ,
       @datefinish        DateTime 
as declare
    @errortext  varchar(256) ,
    @status  integer,
    @fileout      varchar (2000),
    @error_file   varchar(200),
    @errout       varchar (2000),
    @lineout      varchar(256),
    @countu  integer ,
    @numstage  integer ,
    @stringo  varchar (2000),
    @countrycode varchar(3),
    @file_type    varchar(2),
    @sqltxt       varchar (2000),
    @dd1      varchar(2) ,
    @dd2      varchar(2) ,
    @mm       varchar(2) ,
    @yyyy     varchar(4) ,
    @num_accts    integer ,
    @tindex  varchar(32),
	@HighStatusTimeFrame int,				-- #17132
	@maxstatus		char(1),				-- #17132
	@AcctsToCheck		BIT,			-- #17132
	@AccTypeToQualify	VARCHAR(5),		-- 17132
    @mssql_txt SQLText
    -- normally we are supplied with the date letter generated and this is usually based on a fixed date
    -- for the Sunday of the run number
    -- but we are also  now generating letters with a datetimestamp created during the process
/*    select @datestart = datestart,@datefinish = datefinish from 
    interfacecontrol
	 where runno =@runno and interface = 'CHARGES'
    and datestart > dateadd (day, - 100, getdate()) --make sure get the most recent run number.
*/
    set dateformat dmy
    select @countrycode = countrycode from country
    set @fileout = ''

    Print 'Processing step 1 of 12...' +  convert (varchar,@datestart) + ' ' + @lettercode
   
	select distinct addtype 
	into #addtype 
	from custaddress
	where addtype like '%H%' or addtype like '%W%'
   
   
    select l.acctno, l.dateacctlttr, l.datedue, l.lettercode,a.dateacctopen,a.highststatus,  
    convert (varchar (90), '') as printname,  
    convert (varchar (60), '') as name,  
    convert (varchar (30), '') as firstname, '               ' as prtacctno,  
    convert (varchar (25), '') as title, convert (varchar (25), '') as branchname,  
    convert (varchar (26), '') as branchaddr1, convert(varchar(26),'') as branchaddr2,  
    convert (varchar (26), '') as branchaddr3, convert (varchar (10), '' ) as branchpocode,  
    convert (varchar (20), '') as telno,
    convert (varchar (50), '') as cusaddr1, convert(datetime,'') as datenextdue,  
    convert (varchar (50), '') as cusaddr2, convert (varchar (50), '') as cusaddr3,  
    convert (varchar (10), '' )  as cuspocode,
    addtype as addtype,
    convert (varchar (34), '') as homephone,
    convert (varchar (34), '') as mobile1,
    convert (varchar (34), '') as workphone,
    convert (varchar (34), '') as mobile2,
    convert (varchar (34), '') as deliveryphone,
    convert (varchar (34), '') as mobile3, ' ' as hldorjnt, a.outstbal, a.arrears, 
    convert(money,0) as instalamount, convert(money,0) as charges, convert(money,0) as creditavailable, 
    convert(money,0) as combinedinstalment,
    convert(money,0) as addtoinstalment,
    convert(varchar(20),'') as custid,  
    convert(integer,0) as branchnohdle, convert(datetime,'') as datefirst,
    -- Privilege Club Tier1/2
    a.currstatus,
    convert(money,0) as availablespend,
    convert(datetime,'') as datelastfreeinstalment,
    convert(money,0) as valuelastfreeinstalment,
    convert(datetime,'') as datenextfreeinstalment,
    convert(money,0) as valuenextfreeinstalment,
    convert(money,0) as totalcashspend,
    convert(datetime,'') as dateintotier2
     into   #tempaccts 
     from LETTER l with(Nolock), ACCT a with(Nolock), #addtype
     where  
     LetterCode =   @lettercode  
     and l.acctno = a.acctno 
     and DateAcctLttr between  @datestart and @datefinish

    set @status =@@error
    if @status != 0 begin
    set @errortext = 'Failed to create temp table 1.'
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     

    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to select data from temporary table.'
        drop table #tempaccts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status 
    end
     
    SELECT  @num_accts =count(*) 
    from #tempaccts 

    if @num_accts = 0 begin
        set @errortext = 'No accounts matched the criteria entered. '
        Print ''
        drop table #tempaccts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status 
    end
   
    /* updating the customer details */
     update   #tempaccts
            set     hldorjnt     = CUSTACCT.hldorjnt,
            custid       = CUSTACCT.custid,
            firstname    = isnull(CUSTOMER.firstname,''),
            name         = CUSTOMER.name,
            title        = CUSTOMER.title,
            branchnohdle = convert(integer,left(custacct.acctno,3)),
            availablespend = CUSTOMER.availablespend
            from    CUSTACCT  , CUSTOMER 
            where   CUSTACCT.acctno   =  #tempaccts.acctno
            and     CUSTACCT.custid   = CUSTOMER.custid
            and     CUSTACCT.hldorjnt = 'H'
        
    UPDATE #tempaccts SET availablespend = 0 WHERE availablespend < 0
        
    set @status =@@error
    if @status != 0 
    begin
         
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

	-- #17132 Loan Letters
	if @lettercode like 'LOAN%'
	Begin
		set @HighStatusTimeFrame = (select value from countrymaintenance where codename='CL_HighStatusTimeFrame')		-- #17132
		set @maxstatus = (select value from countrymaintenance where codename='CL_HighSettStat2Yr')		-- #17132
		set @AcctsToCheck=0			-- #17132
		set @AccTypeToQualify= (select value from countrymaintenance where codename='CL_AccountType')	-- CR1232	

		-- Accounts to check - HP, RF or both 
		create table #AcctsToCheck			-- 17132
		(
			acctno				varchar(12),
			acctType            CHAR(1)					
		)

		-- Get accounts to check   
		if @AccTypeToQualify!='Both'
		Begin
			insert into #AcctsToCheck (acctno,accttype) 
			select a.acctno,a.accttype from acct a with(Nolock) INNER JOIN custacct ca with(Nolock) on a.acctno = ca.acctno and ca.hldorjnt='H'
										INNER JOIN #tempaccts i on i.custid=ca.custid
			where (accttype='O' and @AccTypeToQualify in ('HP'))
				  or 
				  (accttype='R' and @AccTypeToQualify in ('RF'))
				  or a.currstatus='9'		-- #17342 - Customer has Staff account
			set @AcctsToCheck=1
		End

		-- Get Highest status of Holder live accounts
		--insert into #maxstatus (custid,datesettled,status)
		select i.custid, null as datestatchge, max(s.statuscode) as maxstatus
		into #maxstatus 
		From #tempaccts i inner join custacct ca on i.custid=ca.custid inner join acct a on a.acctno=ca.acctno
				inner join status s on a.acctno=s.acctno
		Where ca.hldorjnt='H' and statuscode NOT IN ('U','O','0','S')
			and datestatchge>=dateadd(m,@HighStatusTimeFrame * -1,getdate())
			and a.agrmttotal>0		-- exclude cancelled a/cs 69427 jec 14/12/07
			and not exists(select 1 from cancellation c 
									where c.acctno = ca.acctno)	--IP - 24/02/11 - #3196 - LW73237 - Exclude Cancelled accounts
			and (exists(select 1 from #AcctsToCheck ck where ck.acctno=a.acctno) or @AcctsToCheck=0)		-- #8478
		Group by i.custid

		update #tempaccts
			set highststatus= maxstatus
		from #tempaccts t inner join #maxstatus m on t.custid=m.custid
	
	end
     
/*  -
Putting spaces in the account number AA - moving this up the list so for
cash accounts to get the benefit.*/
    update #tempaccts
    set prtacctno = left(acctno,3) + ' ' + left(right(acctno,9),4) + ' ' +
            left(right(acctno,5),4) + ' ' + right(acctno,1) 
/* updating charges from Cosacs letters and charges generation */
if @runno > 0
begin
    update   #tempaccts 
    SET charges = chargesdata.intchargesapplied + isnull (chargesdata.admchargesapplied,0), 
    arrears =  #tempaccts.arrears - chargesdata.intchargesapplied - isnull (chargesdata.admchargesapplied,0)
    from chargesdata 
    WHERE chargesdata.acctno =  #tempaccts.acctno 
    AND chargesdata.runno =  @runno 
    
    set @status =@@error
end
    if @status !=0 begin
        set @errortext = 'error updating intchargesapplied  '
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status 
    end


/* all the below not necessary for cash accounts */
if  LEFT(@lETTERCODE,1) = 'C' or @LETTERCODE in ('WCR', 'WGL', 'WR')
begin
    execute @status = dn_lgcashletter @lettercode=@lettercode,@runno=@runno; 
    return @status
end

    Print 'Processing step 2 of 12...'
 

    /* To prevent problem where accounts had more than 1 instalplan */
    
    select ip.acctno, max(ip.datefirst) as maxdatefirst
    into  #temp_max 
    from INSTALPLAN ip,  #tempaccts  t
    where ip.acctno = t.acctno 
    group by ip.acctno
    
    
    set @status =@@error

    if @status != 0 begin
         
        set @errortext = 'Failed to create temp table 2.'
        drop table #tempaccts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
    /*
    ** Create a temporary table to deal with all accounts associated with a custid
    ** so as to get the combined rebate figure for each custid initially loaded.
    */

    select distinct ac.AcctNo, ac.AcctType, ac.OutstBal, isnull(ac.TermsType,'00') as termstype,
    ca.custid, ip.InstalNo, ip.InstalAmount, ip.DateFirst, ip.DateLast, ag.Servicechg,
    convert(money,0) as rebate, convert(float,0) as mthstorun, convert(float,0) as servpcent, ag.datenextdue,
    CONVERT(VARCHAR,GETDATE(),106) as temp_date 
    into  #temp_work 
    from ACCT ac, AGREEMENT ag, INSTALPLAN ip, CUSTACCT ca,  #tempaccts  t,  #temp_max  x
    where ac.AcctNo    = ca.AcctNo
    and ac.AcctNo      = ag.AcctNo
    and ac.AcctNo      = ip.AcctNo
    and ag.AgrmtNo     = 1
    and ag.AgrmtNo     = ip.AgrmtNo
    and t.custid       = ca.custid
    and ca.hldorjnt    = 'H'
    and x.acctno       = ac.acctno
    and x.maxdatefirst = ip.datefirst
 
    set @status =@@error
    if @status != 0 
    begin
        set @errortext = 'Failed to create temp table 3.'
        drop table #tempaccts  
        drop table #temp_max  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

    Print 'Processing step 3 of 12...'
    /* Getting the right service charge for add-to letters
    This is actually the current service charge on the account*/
     update  #temp_work 
            set servpcent = termstype.servpcent
            from termstype
            where  #temp_work.termstype = termstype.termstype
            and termstype.countrycode = @countrycode
      
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts 
        drop table #temp_max 
        drop table #temp_work 
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

    update #temp_work
    set mthstorun = instalno
    where datelast = '1-jan-1900' or datelast is null 
    set @status =@@error

    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts 
        drop table #temp_max 
        drop table #temp_work 
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

/* AA - don't know whats happening here*/
    
    update   #temp_work 
    set temp_date = dateadd(month,1,temp_date) 
    where datepart(day,temp_date) > datepart(day,datelast) 
   
    set @status =@@error

    if @status != 0 begin
         
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts 
        drop table #temp_max 
        drop table #temp_work 
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

/* - this is for add-letters only number of months left on the agreement - datelast related
*/
    
    update  #temp_work  set mthstorun = 
    convert(float,((datepart(year,datelast) - datepart(year,temp_date)) * 12  +
    (datepart(month,datelast) - datepart(month,temp_date)))) 
        WHERE  datelast != '' AND datelast is not null 
  
    set @status =@@error
    if @status != 0 
    begin
        set @errortext = 'Failed to update temp_work 3.'
        drop table #tempaccts  
        drop table temp_max  
        drop table temp_work  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     

/* if calculated number of mths to run exceeds instalplan then reduce it to number
   of instalments*/
    update #temp_work
    set mthstorun = convert(float,instalno - 1)
    where mthstorun >= instalno 
    set @status =@@error

    if @status != 0 begin
        set @errortext = 'Failed to update temp_work 2.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
/* fiji or malaysia have a stingy rebate calculation */
if @countrycode = 'F' OR @countrycode = 'Y'
begin
/*  */
     
     set @mssql_txt =' update #temp_work'
            + ' set rebate = servicechg * convert(float,((MthstoRun * ( MthsToRun + 1)) / ' +
                                     ' (InstalNo * (InstalNo + 1)))) where instalno > 0 ' 
    execute sp_executesql @mssql_txt 
end    
else
begin
        set @mssql_txt =
            ' update  #temp_work ' +
            ' set rebate = servicechg * convert(float,((MthstoRun * ( MthsToRun - 1)) / ' +
                                 ' (InstalNo * (InstalNo + 1)))) where instalno > 0 '  
        execute sp_executesql @mssql_txt 
        
end

    set @status =@@error
    if @status != 0 begin
         
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts 
        drop table #temp_max 
        drop table #temp_work 
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
    /*
    ** Calculate the values of combined instalment, credit available and add-to instalment
    */

    Print 'Processing step 4 of 12...'
  
    select custid, max(instalno) as maxinstalno,'               ' as acctno 
    into  #temp_maxinstalno 
    from  #temp_work 
    where outstbal > convert(money, 0)
    group by custid
    
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to create temp table 4.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    
    select t.custid, count(*) as total, max(t.acctno) as acctno
    into  #temp_maxacctno 
    from  #temp_work  t, #temp_maxinstalno  i
    where t.custid = i.custid
    and t.instalno = i.maxinstalno
    and outstbal > 0
    group by t.custid
    having count(*) > 1
    
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to create temp table 5.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

    Print 'Processing step 5 of 12...'
    
     update  #temp_maxinstalno 
            set acctno =  #temp_maxacctno.acctno
            from  #temp_maxacctno 
            where  #temp_maxinstalno.custid =  #temp_maxacctno.custid 
    set @status =@@error

    if @status != 0 begin
         
        set @errortext = 'Failed to update maxinstalno 1.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

    /* max instalno is to get the longest agreement for this customer */
    update  #temp_maxinstalno 
            set acctno =  #temp_work.acctno
            from  #temp_work 
            where  #temp_maxinstalno.custid =  #temp_work.custid
            and  #temp_maxinstalno.maxinstalno =  #temp_work.instalno
            and  #temp_maxinstalno.acctno = ''
            and  #temp_work.outstbal > 0 
   

    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update maxinstalno 2.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     

    Print 'Processing step 6 of 12...'

    /* Combinst is the combined instalments from all the customers open accounts.
    */
    select custid, convert(float,0) as servp, convert(integer,0) as longestterm,
    isnull(sum(instalamount),0) as combinst, isnull(sum(outstbal),0) as total,
    isnull(sum(rebate),0) as rebtotal
    into  #temp_sumaccounts 
    from  #temp_work 
    where outstbal > 0
    group by custid
    
/* STL 18-Aug-2000 END */
    
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to create temp table 6.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
    /* Update it with longest terms and service percent of termstype*/
     update  #temp_sumaccounts 
            set servp       =  #temp_work.servpcent,
            longestterm =  #temp_maxinstalno.maxinstalno
            from  #temp_maxinstalno  ,  #temp_work 
            where  #temp_sumaccounts.custid =  #temp_maxinstalno.custid
            and  #temp_sumaccounts.custid =  #temp_work.custid
            and  #temp_work.acctno =  #temp_maxinstalno.acctno
   
    
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update totals.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    Print 'Processing step 7 of 12...'
    /*credit available and addtoinstalment are to send letters out to customers
    inviting them to spend more and extend their agreement
    while retaining their monthly instalment the below calculation is overwritten
    in process 7A which is called from crtchargesdata OpenROAD function*/
     update  #tempaccts 
        set combinedinstalment =  #temp_sumaccounts.combinst,
        creditavailable = (( #temp_sumaccounts.combinst * longestterm) /
        (1 + ((( #temp_sumaccounts.servp/100) * longestterm)/12))) -
        (total - rebtotal),
        addtoinstalment = ((total - rebtotal) * (1 + (((+ #temp_sumaccounts.servp/100) * longestterm) /
        12))) / longestterm
        from  #temp_sumaccounts 
        where + #tempaccts.custid = + #temp_sumaccounts.custid and longestterm >0
     
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
   Print 'Processing step 7A of 12...'
    
     update  #tempaccts 
            set creditavailable = a.addtovalue, 
            addtoinstalment = a.instalamount 
            from  #tempaccts  b, addtoletter a 
            where a.acctno = b.acctno and 
            a.runno =  @runno 

    -- 64592 do not send letters if addtoletter and credit available < 0
    delete from letter where lettercode in ('1','2A','3') and dateacctlttr 
    between @datestart and @datefinish and exists (select * from 
    #tempaccts where #tempaccts.acctno = letter.acctno   and creditavailable <0)

	DELETE FROM  #tempaccts 
   where creditavailable <0 AND 
   lettercode in ('1','2A','3') 
   

    
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     

   Print 'Processing step 7b of 12...'
    
     update  #tempaccts 
            set instalamount =  #temp_work.instalamount,
            datefirst    =  #temp_work.datefirst,
            datenextdue  =  #temp_work.datenextdue
            from  #temp_work 
            where  #tempaccts.acctno =  #temp_work.acctno
    
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update temp table 2.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
   Print 'Processing step 7C of 12...'


if @runno > 0
begin
    update   #tempaccts 
    SET datenextdue = chargesdata.datenextdue 
    from chargesdata 
    WHERE chargesdata.acctno =  #tempaccts.acctno 
    AND chargesdata.runno =  @runno 
    set @status =@@error
end
if @status != 0 begin
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
end
      -- removing letter if arrears letter and datefirst not set
IF @lettercode not in ('TIER1HPL','TIER1HPS','TIER1RF',
       'TIER2HPL','TIER2HPS','TIER2RF','INSTCR','LOANE', 'LOANR')
BEGIN
	delete from #tempaccts
    where datefirst = '1-jan-1900' or datefirst is null 
END
    


    set @status =@@error

    if @status != 0 begin
        set @errortext = 'Failed to select letter details.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

    /*
    ** Load financial details, for all accounts whose arrears amount is equal or
    ** less than the arrears charges, remove it from the main temporary table
    ** and reverse the charges
    */
    /*
    ** Edit customer name
    */

     update   #tempaccts
    set printname = isnull (firstname, '')  + ' ' + name
    where /* locate(name,'/') > size(name) Am removing this functionality  and amending it*/
     hldorjnt in('H', 'J', 'G') 

    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update temp table. '
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
    /*
    ** Get address details
    */

    Print 'Processing step 11 of 12...'
    select c.custid, c.addtype, max(c.datein) as maxdatein
    into  #temp_addr 
    from custaddress c,  #tempaccts  t
    where t.custid = c.custid
    and c.addtype in('P', 'H', 'W')
    and not (c.cusaddr1 =''
    and (c.cusaddr2 is null or c.cusaddr2 ='')) 
    and (c.datemoved = ''or c.datemoved is null)
    group by c.custid, c.addtype
    set @status =@@error

    if @status != 0 begin
        set @errortext = 'Failed to create temp table 9.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext
        return @status  
    end
    
     update  #tempaccts 
            set cusaddr1 = custaddress.cusaddr1,
            cusaddr2 = isnull(custaddress.cusaddr2,''),
            cusaddr3 = isnull(custaddress.cusaddr3,''),
            cuspocode = isnull(custaddress.cuspocode,''),
            addtype  = custaddress.addtype
            from custaddress ,  #temp_addr 
            where  #tempaccts.custid = custaddress.custid
            and #tempaccts.addtype = 'H'
            and  #tempaccts.custid =  #temp_addr.custid
            and custaddress.addtype =  #temp_addr.addtype
            and custaddress.addtype = 'P'
            and custaddress.datein =  #temp_addr.maxdatein 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
     update  #tempaccts 
            set cusaddr1 = custaddress.cusaddr1,
            cusaddr2 = isnull(custaddress.cusaddr2,''),
            cusaddr3 = isnull(custaddress.cusaddr3,''),
            cuspocode= isnull(custaddress.cuspocode,''),
            addtype  = custaddress.addtype
            from custaddress ,  #temp_addr 
            where  #tempaccts.custid = custaddress.custid
            and  #tempaccts.custid =  #temp_addr.custid
            and custaddress.addtype =  #temp_addr.addtype
            and custaddress.addtype = 'H'
            and  #tempaccts.addtype = 'H'
            and custaddress.datein =  #temp_addr.maxdatein 

    set @status =@@error

    if @status != 0 begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    
    
    update  #tempaccts 
            set cusaddr1 = custaddress.cusaddr1,
            cusaddr2 = isnull(custaddress.cusaddr2,''),
            cusaddr3 = isnull(custaddress.cusaddr3,''),
            cuspocode = isnull(custaddress.cuspocode,''),
            addtype  = custaddress.addtype
            from custaddress ,  #temp_addr 
            where  #tempaccts.custid = custaddress.custid
            and #tempaccts.addtype = 'W'
            and  #tempaccts.custid =  #temp_addr.custid
            and custaddress.addtype =  #temp_addr.addtype
            and custaddress.addtype = 'W'
            and custaddress.datein =  #temp_addr.maxdatein 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
    /*
    ** Update Telephone Details
    */
    select c.custid, c.tellocn as addtype, max(c.dateteladd) as dateteladd
    into  #temp_tel 
    from custtel c,  #tempaccts  t
    where t.custid = c.custid
    and ltrim(rtrim(c.tellocn)) in ('D', 'D1', 'H', 'W', 'M', 'M2', 'M3')
    and not (isnull(c.telno, '') is null or isnull(c.telno, '') ='')
    and (c.datediscon = ''or c.datediscon is null)
    group by c.custid, c.tellocn
    set @status =@@error
    
    --select  * from #temp_tel where custid = 'G3818473'

    if @status != 0 begin
        set @errortext = 'Failed to create temp table tel.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext
        return @status  
    end
    
    
    --delivery phone
    
    update  #tempaccts 
            set deliveryphone = ''+isnull(c.dialcode, '') + isnull(c.telno, '') + isnull(c.extnno, '')
            from custtel c,  #temp_tel 
            where  #tempaccts.custid = c.custid
            and #tempaccts.addtype in ('H', 'P')
            and #tempaccts.dateacctopen = 
					(
						select max(dateacctopen) 
						from #tempaccts b
						where b.custid = #tempaccts.custid
						
					)
            and  #tempaccts.custid =  #temp_tel.custid
            and c.tellocn =  #temp_tel.addtype
            and c.tellocn = 'D'
            and c.dateteladd =  #temp_tel.dateteladd 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
    --Delivery 1 phone
    
     update  #tempaccts 
            set deliveryphone = ''+isnull(c.dialcode, '') + isnull(c.telno, '') + isnull(c.extnno, '')
            from custtel c,  #temp_tel 
            where  #tempaccts.custid = c.custid
            and #tempaccts.addtype in ('H', 'P')
            and #tempaccts.dateacctopen = 
					(
						select max(dateacctopen)  
						from #tempaccts b
						where b.custid = #tempaccts.custid
						
					)
            and  #tempaccts.custid =  #temp_tel.custid
            and c.tellocn =  #temp_tel.addtype
            and c.tellocn = 'D1'
            and c.dateteladd =  #temp_tel.dateteladd 
            and ltrim(rtrim(deliveryphone)) = ''
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end 
     
     
    --home phone
    
    update  #tempaccts 
            set homephone = ''+isnull(c.dialcode, '') + isnull(c.telno, '') + isnull(c.extnno, '')
            from custtel c,  #temp_tel 
            where  #tempaccts.custid = c.custid
            and #tempaccts.addtype in ('H', 'P')
            and #tempaccts.dateacctopen = 
					(
						select max(dateacctopen) 
						from #tempaccts b
						where b.custid = #tempaccts.custid
					)
            and  #tempaccts.custid =  #temp_tel.custid
            and c.tellocn =  #temp_tel.addtype
            and c.tellocn = 'H'
            and c.dateteladd =  #temp_tel.dateteladd 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    
    --work phone
    
    update  #tempaccts 
            set workphone = ''+isnull(c.dialcode, '') + isnull(c.telno, '') + isnull(c.extnno, '')
            from custtel c,  #temp_tel 
            where  #tempaccts.custid = c.custid
            and #tempaccts.addtype in ('W')
            and #tempaccts.dateacctopen = 
					(
						select max(dateacctopen) 
						from #tempaccts b
						where b.custid = #tempaccts.custid
					)
            and  #tempaccts.custid =  #temp_tel.custid
            and c.tellocn =  #temp_tel.addtype
            and c.tellocn = 'W'
            and c.dateteladd =  #temp_tel.dateteladd 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    
    --mobile1 phone
    
    update  #tempaccts 
            set mobile1 = ''+isnull(c.dialcode, '') + isnull(c.telno, '') + isnull(c.extnno, '')
            from custtel c,  #temp_tel 
            where  #tempaccts.custid = c.custid
            and #tempaccts.addtype in ('H', 'P')
            and #tempaccts.dateacctopen = 
					(
						select max(dateacctopen) 
						from #tempaccts b
						where b.custid = #tempaccts.custid
						
					)
            and  #tempaccts.custid =  #temp_tel.custid
            and c.tellocn =  #temp_tel.addtype
            and c.tellocn = 'M'
            and c.dateteladd =  #temp_tel.dateteladd 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    
    --mobile2 phone
    
    update  #tempaccts 
            set mobile2 = ''+isnull(c.dialcode, '') + isnull(c.telno, '') + isnull(c.extnno, '')
            from custtel c,  #temp_tel 
            where  #tempaccts.custid = c.custid
            and #tempaccts.addtype in ('H', 'P')
            and #tempaccts.dateacctopen = 
					(
						select max(dateacctopen) 
						from #tempaccts b
						where b.custid = #tempaccts.custid
						
					)
            and  #tempaccts.custid =  #temp_tel.custid
            and c.tellocn =  #temp_tel.addtype
            and c.tellocn = 'M2'
            and c.dateteladd =  #temp_tel.dateteladd 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    
    --mobile3 phone
    
    update  #tempaccts 
            set mobile3 = ''+isnull(c.dialcode, '') + isnull(c.telno, '') + isnull(c.extnno, '')
            from custtel c,  #temp_tel 
            where  #tempaccts.custid = c.custid
            and #tempaccts.addtype in ('H', 'P')
            and #tempaccts.dateacctopen = 
					(
						select max(dateacctopen) 
						from #tempaccts b
						where b.custid = #tempaccts.custid
						
					)
            and  #tempaccts.custid =  #temp_tel.custid
            and c.tellocn =  #temp_tel.addtype
            and c.tellocn = 'M3'
            and c.dateteladd =  #temp_tel.dateteladd 
    set @status =@@error
    if @status != 0
    begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
    
    
    
    /*
    ** Get branch details
    */
     update  #tempaccts 
            set branchname    = branch.branchname,
            branchaddr1   = branch.branchaddr1,
            branchaddr2   = isnull(branch.branchaddr2,''),
            branchaddr3   = isnull(branch.branchaddr3,''),
            branchpocode  = isnull(branch.branchpocode,''),
            telno         = isnull(branch.telno,'')
            from branch
            where  #tempaccts.branchnohdle = branch.branchno
      
    set @status =@@error
    if @status != 0 begin
        set @errortext = 'Failed to update temp table.'
        drop table #tempaccts  
        drop table #temp_max  
        drop table #temp_work  
        drop table #temp_maxinstalno  
        drop table #temp_maxacctno  
        drop table #temp_sumaccounts  
          
          
        drop table #temp_addr  
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end
     
    Print 'Processing step 12 of 12...'

    /*these accounts have arrears < charges-should never happen-so and removing these arrears letters
    */
    delete from #tempaccts
    where arrears <= charges
    and lettercode in('J', 'K', 'L', 'P', 'Q', 'V', 'X', 'R', 'U', '01', '0') 
    set @status =@@error

    if @status != 0 begin
        set @errortext = 'Failed to delete accounts.'
        drop table #tempaccts 
        drop table #temp_max 
        drop table #temp_work 
        drop table #temp_maxinstalno 
        drop table #temp_maxacctno 
        drop table #temp_sumaccounts 
         
         
        drop table #temp_addr 
        execute dn_lglogleterror @lettercode =@lettercode,@runno=@runno,@errortext=@errortext return @status  
    end

    /*
    ** Privilege Club
    */
    -- RF/HP last free instalment
    UPDATE #tempaccts
    SET    datelastfreeinstalment = fi.dateissued,
           valuelastfreeinstalment = fi.amount
    FROM   FreeInstalment fi
    WHERE  #tempaccts.LetterCode IN ('TIER1RF','TIER2RF',
                                     'TIER1HPS','TIER1HPL','TIER1HPO',
                                     'TIER2HPS','TIER2HPL','TIER2HPO')
    AND    fi.AcctNo = #tempaccts.AcctNo

    DECLARE @consecutiveInstalments INTEGER
    SELECT  @consecutiveInstalments = Value
    FROM    CountryMaintenance
    WHERE   CodeName = 'ConsecutiveInstalments'

    -- RF next free instalment
    UPDATE #tempaccts
    SET    datenextfreeinstalment =
               CASE WHEN datelastfreeinstalment IS NULL
               THEN DATEADD(Month, @consecutiveInstalments, ip.DateFirst)
               ELSE DATEADD(Month, 12, datelastfreeinstalment)
               END,
           valuenextfreeinstalment = ROUND((ip.instalAmount / 4.33),0,1)
    FROM   Instalplan ip
    WHERE  #tempaccts.LetterCode IN ('TIER1RF','TIER2RF')
    AND    ip.AcctNo = #tempaccts.AcctNo

    -- Tier1 Cash Spend
    UPDATE #tempaccts
    SET    totalcashspend = pc.Tier1CashSpend
    FROM   PCCustomerTiers pc
    WHERE  #tempaccts.LetterCode = 'TIER1C'
    AND    pc.AcctNo = #tempaccts.AcctNo

    -- Tier2 Cash Spend
    UPDATE #tempaccts
    SET    totalcashspend = pc.Tier2CashSpend
    FROM   PCCustomerTiers pc
    WHERE  #tempaccts.LetterCode = 'TIER2C'
    AND    pc.AcctNo = #tempaccts.AcctNo

    -- HP date went into Tier2
    UPDATE #tempaccts
    SET    dateintotier2 = 
               (SELECT MAX(ac.DateCoded) FROM AcctCode ac
                WHERE  ac.AcctNo = #tempaccts.AcctNo
                AND    Code = N'TIR2' )
    WHERE  #tempaccts.LetterCode IN ('TIER1HPS','TIER1HPL','TIER1HPO',
                                     'TIER2HPS','TIER2HPL','TIER2HPO')


     
    /*
    ** Select all letter details and write information to a file.
    */
   execute @status =  dn_loanwriteletter @lettercode =@lettercode ,@runno =@runno
   
    Print 'Processing complete'

    drop table #tempaccts  
    drop table #temp_max  
    drop table #temp_work  
    drop table #temp_maxinstalno  
    drop table #temp_maxacctno  
    drop table #temp_sumaccounts  
    drop table #temp_addr  

   return @status


GO

