-- **********************************************************************
-- Title:
-- Developer: Alex Ayscough
-- Date: April 2007
-- Purpose: This creates 3 triggers tr_CMfintrans tr_CMBailaction and tr_CMAcctcode
-- which move accounts from one strategy to another given certain actions e.g. Promise to Pay. 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 21/08/09  IP  UAT(800) - Added 'PTPCOMP'
-- 09/09/09  IP  UAT(837) - Added 'REPO'
-- 16/09/10  IP  Check that instalment!=0 as previously was getting divide by zero error.
-- 01/02/12  jec #9521 increase number of @statement variables due to StoreCard strategies
-- 08/03/12  IP  CR9417 - #9745 - Added condition 'ArrearsPCSC' when generating fintrans trigger
-- 14/03/12  jec CR9417 - cater for SCNON strategy
-- **********************************************************************
if exists  (select * from sysobjects where name ='CM_GenerateCollectionsTriggersSP')
drop procedure CM_GenerateCollectionsTriggersSP
go
-- here we are going to generate triggers based on what is stored in the ... table
-- the actions that would cause an account to exit a strategy are as follows:
--1.)	Account can only exit if settled or falls into arrears (if in non-arrears)  -- on acct table
--2.)	Account can exit to another strategy if it meets that criteria  -- end of day
--3.)	Customer makes a payment resulting in arrears of < ?x? dollar value  -- on fintrans/ acct table ?
--4.)	Customer makes a payment resulting in arrears of < than ?x?% of one instalment -- on fintrans/acct table??
/*5.)	Customer makes a payment of greater than ?x? % but less then ?x?% resulting in account going to strategy ?xyz? --?? on fintrans/acct table
6.)	Arrears are less than or greater than ?x?% of one instalment --on fintrans/acct table
7.)	Customer misses a PTP payment and goes to broken PTP. -- end of day process
8.)	Promise to Pay completed -- on fintrans table?? 
9.)	Promise to pay entered on account-- on bailaction table -- instant??
10.)	Arrangement entered on account-- on spa table -- instant??
11.)	Account allocated to a bailiff -- on follupalloc table -- should be instant really
14.)	Repossession conducted on account so goes to repossession strategy-- on fintrans table -- doesn't have to be instant
15.)	An arrangement is entered on the account routing the account to the Arrangement Strategy-- on spa table -- doesn't have to be instant
16.)	A Promise to Pay is entered on the account routing the account to the PTP Strategy-- on bailaction table.  -- doesn't have to be instant
*/
create procedure CM_GenerateCollectionsTriggersSP

@return int OUTPUT
as

set @return = 0
set nocount on 
declare @statement sqltext,  @statement1 sqltext, @statement2 sqltext, @statement3 sqltext, @statement4 sqltext, @statement5 sqltext,
		@statement6 sqltext, @statement7 sqltext, @statement8 sqltext, @statement9 sqltext, @statement10 sqltext,  
		@statement11 sqltext, @statement12 sqltext, @statement13 sqltext, @statement14 sqltext, @statement15 sqltext,
		@statement16 sqltext, @statement17 sqltext, @statement18 sqltext, @statement19 sqltext, @statement20 sqltext,
		@statement21 sqltext, @statement22 sqltext, @statement23 sqltext, @statement24 sqltext, @statement25 sqltext,
		@statement26 sqltext, @statement27 sqltext, @statement28 sqltext, @statement29 sqltext, @statement30 sqltext,
		@statement31 sqltext, @statement32 sqltext, @statement33 sqltext, @statement34 sqltext, @statement35 sqltext,
		@statement36 sqltext, @statement37 sqltext, @statement38 sqltext, @statement39 sqltext, @statement40 sqltext
--sqltext
,@newline varchar(24),@counter int
set @newline = '
'
set @statement = '' set @counter =1 set @statement1 = '' set @statement2 = '' set @statement3 = '' set @statement4 = '' set @statement5 = ''
set @statement6 = '' set @statement7 = '' set @statement8 = '' set @statement9 = '' set @statement10 = ''
  set @statement11 = '' set @statement12 = '' set @statement13 = '' set @statement14 = '' set @statement15 = ''
set @statement16 = '' set @statement17 = '' set @statement18 = '' set @statement19 = '' set @statement20 = ''
 set @statement21 = '' set @statement22 = '' set @statement23 = '' set @statement24 = '' set @statement25 = ''
set @statement26 = '' set @statement27 = '' set @statement28 = '' set @statement29 = '' set @statement30 = ''
set @statement31 = '' set @statement32 = '' set @statement33 = '' set @statement34 = '' set @statement35 = ''
set @statement36 = '' set @statement37 = '' set @statement38 = '' set @statement39 = '' set @statement40 = ''

declare @Thain char(1),@countrycode char(1),
   @Condition varchar(12) ,
   @Operand varchar(10), --- between >< = 
   @Operator1 varchar(24), -- value.....
   @Operator2 varchar(24), -- 
   @OrClause char(1),  --  can be a/b/c/1/2/3 - used to match orclauses
   @step smallint,
   @ActionCode varchar(12),@strategy varchar(7),
   @StepActiontype char(1)

select @countrycode =countrycode from country
if @countrycode = 'H'
   set @ThaiN ='n'
else
   set @ThaiN =''
set @statement = 'if exists (select * from sysobjects where name =''tr_CMfintrans'') ' +
                 '   drop trigger tr_CMfintrans '
execute sp_executesql @statement

set @statement = 'create trigger tr_CMfintrans on fintrans FOR INSERT ' + @NEWLINE +
' /*trigger created automatically by CM_GenerateCollectionsTriggersSP - AA ' + convert(varchar,getdate()) 
+ '*/' +@newline +
'as declare @transvalue money,@acctno ' + @ThaiN + 'char(12),@balance money,@instalment money,@currentDate datetime,@storeCardMinPay money ' + @newline +		--IP - 08/03/12 - #9745 - CR9417 - added @storeCardMinPay
' ,@transtypecode varchar(3), @balexcharges money,@arrearsplusBDW money,@strategy varchar(7) ' + @newline +
' select @transvalue = transvalue,  @acctno = acctno , @transtypecode = transtypecode from Inserted ' +  + @newline +
' select @balance =sum(transvalue) from fintrans where acctno = @acctno and transtypecode !=''BDW'' ' + @newline + 
' select @storeCardMinPay = MinimumPayment from StoreCardPaymentDetails where acctno = @acctno ' + @newline +		--IP - 08/03/12 - #9745 - CR9417 - added @storeCardMinPay
' set @currentDate = dateadd(second,2,getdate()) ' + @newline + 
' select @instalment = instalamount from instalplan where acctno = @acctno and agrmtno =1 ' + @newline +
' if @transtypecode not in (''INT'',''ADM'',''REP'',''RDL'') ' + @newline +
'  select @balexcharges = sum(transvalue) from fintrans where acctno = @acctno and @transtypecode not in (''INT'',''ADM'') ' + @newline + 
'  select @arrearsplusBDW= arrears + isnull(bdwbalance,0) + isnull(bdwcharges,0)  from acct where acctno = @acctno ' + @newline +
'  if @transtypecode in (''INT'',''ADM'',''BDW'') ' + @newline +
'	  select @arrearsplusBDW= @arrearsplusBDW - @transvalue' + @newline /* increase value of arrears by value of  */

--print @statement
if exists (SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,C.StepActiontype 
   from CMStrategyCondition C, CMStrategy S,Cmcondition O
   where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
   and c.condition in ('Pays',   'PTPLESS','PartPay','BALEXCHARGES',   'ArrearsPC' , 'balanc','LASTPTPPAID', 'PTPCOMP','Woff', 'REPO', 'ArrearsPCSC') --IP - 08/03/12 - #9745 - CR9417 - added ArrearsPCSC -- UAT829 jec Woff  --IP - 09/09/09 - UAT(837) - REPO
   and O.Condition= C.condition 
   --and o.type in ('S','X') 
   and isnull(C.SavedType,'') = 'X' and s.isactive !=0)
   

begin
   -- now if we load up all the exit conditions or stepss,,,,, 
   DECLARE FintransTrigger_cursor CURSOR 
  	   FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,C.StepActiontype 
      from CMStrategyCondition C, CMStrategy S,Cmcondition O
      where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
      and c.condition in ('Pays',   'PTPLESS','PartPay','BALEXCHARGES',   'ArrearsPC' , 'balanc','LASTPTPPAID', 'PTPCOMP','Woff', 'REPO', 'ArrearsPCSC') --IP - 08/03/12 - #9745 - CR9417 - added ArrearsPCSC
      and O.Condition= C.condition 
      --and o.type in ('S','X') 
      and isnull(C.SavedType,'') = 'X' and s.isactive !=0
      OPEN FintransTrigger_cursor
      FETCH NEXT FROM FintransTrigger_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
              @OrClause,@step,@ActionCode,@StepActiontype 
      WHILE (@@fetch_status <> -1)
      BEGIN
	      IF (@@fetch_status <> -2)
   	   begin
                   
            if @actionCode !=''
            begin   
               select @strategy,@actioncode,@operator1,@operand
               set @statement = @statement + ' select @strategy = isnull(strategy,'''') from CMStrategyAcct where acctno = @acctno and dateto is null ' + @newline
               set @statement = @statement + ' if @strategy =' + '''' + @strategy + '''' + @newline
               if @condition ='PAYS'
               begin
                  set @statement = @statement + ' and @balance   ' + convert(varchar,@operand) + ' ' + @operator1
               end
               if @condition ='PartPay' and isnull(@Operator2,'') !='' and @Operand like 'between%'
               begin
               set @statement = @statement + ' and @instalment!=0 ' + ' and @arrearsplusBDW/@instalment*100  ' + @operand + ' ' --IP - 16/09/10 - Prevent divide by zero
                     + @operator1 + ' and ' + isnull(@Operator2,'')
               end
               if @condition ='PartPay' and @Operator2 is null
               begin
                 set @statement = @statement + ' and @instalment!=0 ' +' and @arrearsplusBDW/@instalment*100  ' + @operand + ' ' +  @operator1  --IP - 16/09/10 Prevent divide by zero
               end
               if @condition ='PartPay' and isnull(@Operator2,'') !='' and @Operand not like 'between%'
               begin
                  set @statement = @statement + ' and @instalment!=0 ' + ' and @arrearsplusBDW/@instalment*100  ' + @operand + ' ' +  @operator1 --IP - 16/09/10 Prevent divide by zero
                  + ' and  @balance/@instalment*100 <' + @operator2  
               end
               
               if @condition ='LASTPTPPAID' -- check no pending ptp's or arrears >0
               begin
                  set @statement = @statement + ' and (@arrearsplusBDW <=0 or -@transvalue > (select ' + @operator1 + '/100 * actionvalue from bailaction b1 where b.acctno =@acctno and ' +
                  ' b1.code =''PTP'' and b.dateadded = (select max(dateadded) from bailaction b2 where b2.acctno =b1.acctno and b2.code =''PTP''))) '
               end
  
               if @condition ='BALEXCHARGES'
               begin
                  set @statement = @statement + ' and @balexcharges ' +  convert(varchar,@Operand) + ' '  + @operator1
               end

               if @condition ='ArrearsPC'
               begin
                   set @statement = @statement + ' and @instalment!=0 '+ ' and @arrearsplusBDW/@instalment*100 '  +  convert(varchar,@Operand) +  --IP - 16/09/10 Prevent divide by zero
                   ' ' + @operator1
               end

               if @condition ='balanc'
               begin
                  set @statement = @statement + ' and @balance ' +  convert(varchar,@Operand) + ' ' + @operator1 
               end
               
               --IP 24/08/09 - UAT(800) - PTPCOMP - If payments have been made to complete the PTP on the account prior to the due date
               --send to the exit strategy.
               if @condition = 'PTPCOMP'
               begin
				  set @statement = @statement + ' and exists (select * from bailaction b' +
												' where b.acctno = @acctno ' +
												' and b.code = ''PTP'' ' +
												' and b.dateadded = (select max(b1.dateadded) from bailaction b1' +
												' where b1.acctno = b.acctno and b1.code = ''PTP'') ' +
												' and exists (select * from fintrans f where f.acctno = b.acctno ' +
												' and f.datetrans > b.dateadded ' +
												' and f.transtypecode = ''PAY'' ' +
												' and f.datetrans < dateadd(d, 1, b.datedue) ' +
												' having sum(-f.transvalue) >= b.actionvalue)) '
               END
               
               --jec 08/09/09 - UAT829 - WOFF - If BDW processd on account
               --send to the exit strategy.
               if @condition = 'WOFF'
               begin
				  set @statement = @statement + ' and @transtypecode =''BDW'' and @transvalue <=0 '
               END
               
               --IP - 09/09/09 - UAT(837) - Reposession processed on account.
               if @condition = 'REPO'
               begin
				   set @statement = @statement + 'and @transtypecode =''REP'' '
               end
               
               --IP - 08/03/12 - #9745 - CR9417
               if @condition ='ArrearsPCSC'
               begin
                   set @statement = @statement + ' and @storeCardMinPay!=0 '+ ' and @arrearsplusBDW/@storeCardMinPay*100 '  +  convert(varchar,@Operand) + 
                   ' ' + @operator1
               end


               set @statement = @statement + @newline + ' begin ' + @newline +
                                             '   update CMStrategyAcct set dateto = @currentdate where acctno = @acctno and strategy = @strategy and dateto is null' + @newline +
                                             '   update CMWorklistsAcct set dateto = @currentdate where acctno = @acctno and strategy = @strategy and dateto is null' + @newline +
                                       '   ' + @newline +
                                       '   insert into CMStrategyAcct ' + @newline +
                                       '  (Acctno,strategy,datefrom,dateto,currentstep,dateincurrentstep) ' + @newline +
                                       '  values(@acctno , ' + '''' + @actioncode + '''' + ', @currentdate, null,0,@currentdate ) ' + @newline +
									   'IF @strategy in (''non'',''SCNON'')	'	+ @newline +			-- jec 14/03/12				
									   'BEGIN 
									    UPDATE followupalloc 
											SET datedelloc = getdate()  
											WHERE dateadelloc IS NULL 
											AND acctno = @acctno 
									      END
                                        END ' + @newline
            end
            set @statement = @statement + @newline + @newline
            --print 'datalength 1 ' + convert(varchar,datalength(@statement))
            
            if datalength(@statement) > 3000 -- we are running near the 6000 maximum so push this across to another statement
            begin
              if @counter = 1
              begin
                  set @statement1 = @statement
                  set @statement = ''
              end
              if @counter = 2
              begin
                  set @statement2 = @statement
                  set @statement = ''
              end
              if @counter = 3
              begin
                  set @statement3 = @statement
                  set @statement = ''
              end
              if @counter = 4
              begin
                  set @statement4 = @statement
                  set @statement = ''
              end
              if @counter = 5
              begin
                  set @statement5 = @statement
                  set @statement = ''
              end
              if @counter = 6
              begin
                  set @statement6 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 7
              begin
                  set @statement7 = @statement
                  set @statement = ''
              end
              if @counter = 8
              begin
                  set @statement8 = @statement
                  set @statement = ''
              end
              if @counter = 9
              begin
                  set @statement9 = @statement
                  set @statement = ''
              end
              if @counter = 10
              begin
                  set @statement10 = @statement
                  set @statement = ''
              end
			  if @counter = 11
              begin
                  set @statement11 = @statement
                  set @statement = ''
              end
              if @counter = 12
              begin
                  set @statement12 = @statement
                  set @statement = ''
              end
              if @counter = 13
              begin
                  set @statement13 = @statement
                  set @statement = ''
              end
              if @counter = 14
              begin
                  set @statement14 = @statement
                  set @statement = ''
              end
              if @counter = 15
              begin
                  set @statement15 = @statement
                  set @statement = ''
              end
              if @counter = 16
              begin
                  set @statement16 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 17
              begin
                  set @statement17 = @statement
                  set @statement = ''
              end
              if @counter = 18
              begin
                  set @statement18 = @statement
                  set @statement = ''
              end
              if @counter = 19
              begin
                  set @statement19 = @statement
                  set @statement = ''
              end
              if @counter = 20
              begin
                  set @statement20 = @statement
                  set @statement = ''
              end
              if @counter = 21
              begin
                  set @statement21 = @statement
                  set @statement = ''
              end
              if @counter = 22
              begin
                  set @statement22 = @statement
                  set @statement = ''
              end
              if @counter = 23
              begin
                  set @statement23 = @statement
                  set @statement = ''
              end
              if @counter = 24
              begin
                  set @statement24 = @statement
                  set @statement = ''
              end
              if @counter = 25
              begin
                  set @statement25 = @statement
                  set @statement = ''
              end
              if @counter = 26
              begin
                  set @statement26 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 27
              begin
                  set @statement27 = @statement
                  set @statement = ''
              end
              if @counter = 28
              begin
                  set @statement28 = @statement
                  set @statement = ''
              end
              if @counter = 29
              begin
                  set @statement29 = @statement
                  set @statement = ''
              end
              if @counter = 30
              begin
                  set @statement30 = @statement
                  set @statement = ''
              end
              if @counter > 30 
              begin
		         RAISERROR('Error saving trigger', 16, 1,'')
              end
              set @counter = @counter +1
            end
         end
         --print @statement
         FETCH NEXT FROM FintransTrigger_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
              @OrClause,@step,@ActionCode,@StepActiontype 

      END
     CLOSE FintransTrigger_cursor
     DEALLOCATE FintransTrigger_cursor
    
      -- this creates the fintrans trigger
      exec (@statement1 + @statement2 + @statement3 + @statement4 + @statement5 + 
			@statement6 + @statement7 + @statement8 + @statement9 + @statement10 + 
			@statement11 + @statement12 + @statement13 + @statement14 + @statement15 + 
			@statement16 + @statement17 + @statement18 + @statement19 + @statement20 +
			@statement21 + @statement22 + @statement23 + @statement24 + @statement25 + 
			@statement26 + @statement27 + @statement28 + @statement29 + @statement30
			+@statement)
      --exec (@statement5 + @statement4 + @statement3 + @statement2 + @statement1 + @statement)
            if @@error > 0
      begin
       print '' + @statement1
        print '' + @statement2 
        print '' + @statement3 
        print '' + @statement4 
        print '' + @statement5 
		print '' + @statement6
        print '' + @statement7 
        print '' + @statement8 
        print '' + @statement9 
        print '' + @statement10 
		 print '' + @statement11
        print '' + @statement12 
        print '' + @statement13 
        print '' + @statement14 
        print '' + @statement15 
		print '' + @statement16
        print '' + @statement17 
        print '' + @statement18 
        print '' + @statement19 
        print '' + @statement20
        print '' + @statement21
        print '' + @statement22 
        print '' + @statement23 
        print '' + @statement24 
        print '' + @statement25 
		print '' + @statement26
        print '' + @statement27 
        print '' + @statement28 
        print '' + @statement29 
        print '' + @statement30  
        print '' + @statement    
      end

end -- only create the trigger if exists
   -- now create the bailaction trigger
set @statement1 =''
set @statement2 =''
set @statement3 =''
set @statement4 =''
set @statement5 =''
set @statement6 =''
set @statement7 =''
set @statement8 =''
set @statement9 =''
set @statement10 =''
set @statement11 =''
set @statement12 =''
set @statement13 =''
set @statement14 =''
set @statement15 =''
set @statement16 =''
set @statement17 =''
set @statement18 =''
set @statement19 =''
set @statement20 =''
set @statement21 = '' set @statement22 = '' set @statement23 = '' set @statement24 = '' set @statement25 = ''
set @statement26 = '' set @statement27 = '' set @statement28 = '' set @statement29 = '' set @statement30 = ''
set @statement31 = '' set @statement32 = '' set @statement33 = '' set @statement34 = '' set @statement35 = ''
set @statement36 = '' set @statement37 = '' set @statement38 = '' set @statement39 = '' set @statement40 = ''
set @counter = 1
set @statement = ' if exists ('

set @statement = 'if exists (select * from sysobjects where name =''tr_CMBailaction'') ' +
                 '   drop trigger tr_CMBailaction '
execute sp_executesql @statement

if exists( SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,C.StepActiontype 
   from CMStrategyCondition C, CMStrategy S,Cmcondition O
   where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
   and c.condition in ('PTPEnt','BailAlloc','Arrange')  and actioncode !=''
   and O.Condition= C.condition 
   --and o.type in ('S','X')
   and isnull(C.SavedType,'') ='X'
   and s.isactive !=0)
BEGIN
   print 'doing bailaction'
   set @statement = 'create trigger tr_CMBailaction on bailaction FOR INSERT ' + @NEWLINE +
   ' /*trigger created automatically by CM_GenerateCollectionsTriggersSP - AA ' + convert(varchar,getdate()) 
   + '*/' + @newline + 
   'as declare @code char(3),@acctno ' + @ThaiN + 'char(12),@balance money,@instalment money,@empeetype varchar(3) ' + @newline +
   ' ,@transtypecode varchar(3), @transvalue money, @balexcharges money,@strategy varchar(7),@currentdate DATETIME ' + @newline +
   ' set @currentdate = dateadd(second,2,getdate()) ' + @newline +
   ' select @code = Inserted.code,  @acctno = Inserted.acctno' +  @newline +
   ' from Inserted,courtsperson c where c.Userid = Inserted.empeeno ' +  + @newline 
   set @statement = @statement + ' select @strategy = isnull(strategy,'''') from CMStrategyAcct where acctno = @acctno and dateto is null ' + @newline + @newline
   DECLARE ActionTrigger_cursor CURSOR 
  	   FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,C.StepActiontype 
      from CMStrategyCondition C, CMStrategy S,Cmcondition O
      where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
      and c.condition in ('PTPEnt','BailAlloc','Arrange')  and actioncode !=''
      and O.Condition= C.condition 
      --and o.type in ('S','X') 
      and isnull(C.SavedType,'') ='X' 
      and s.isactive !=0
      OPEN ActionTrigger_cursor
      FETCH NEXT FROM ActionTrigger_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
              @OrClause,@step,@ActionCode,@StepActiontype 
      WHILE (@@fetch_status <> -1)
      BEGIN
	      IF (@@fetch_status <> -2)
   	   begin
              if @condition ='PTPEnt'
              begin
               set @statement = @statement + ' if @strategy =' + '''' + @strategy + '''' + ' and  @code = ''PTP''  ' + @newline
              end
			  --IP - 11/06/09 - Credit Collection Walkthrough Changes - changed 'ARR' to 'SPA' for special arrangement.
			  if @condition ='Arrange'
              begin
               set @statement = @statement + ' if @strategy =' + '''' + @strategy + '''' + ' and  @code = ''SPA''  ' + @newline
              end

              if @condition ='BailAlloc'
              begin
               set @statement = @statement + ' if @strategy =' + '''' + @strategy + '''' + ' and  (@code = ''ALL'' AND @empeetype =''B'') -- allocated to bailiff' + @newline
              end
            
              set @statement = @statement +  ' begin ' + @newline +
                                             '   update CMStrategyAcct set dateto = @currentDate where acctno = @acctno and strategy = @strategy and dateto is null' + @newline +
                                             '   update CMWorklistsAcct set dateto = @currentDate where acctno = @acctno and strategy = @strategy and dateto is null' + @newline +
                                       '   ' + @newline +
                                       '   insert into CMStrategyAcct ' + @newline +
                                       '  (Acctno,strategy,datefrom,dateto,currentstep,dateincurrentstep) ' + @newline +
                                       '  values(@acctno , ' + '''' + @actioncode + '''' + ', @currentDate, null,0,@currentDate ) ' + @newline +
                                       ' end ' + @newline
 
            --print 'datalength 2 ' + convert(varchar,datalength(@statement))
             if datalength(@statement) > 3000 -- we are running near the 6000 maximum so push this across to another statement
            begin
              if @counter = 1
              begin
                  set @statement1 = @statement
                  set @statement = ''
              end
              if @counter = 2
              begin
                  set @statement2 = @statement
                  set @statement = ''
              end
              if @counter = 3
              begin
                  set @statement3 = @statement
                  set @statement = ''
              end
              if @counter = 4
              begin
                  set @statement4 = @statement
                  set @statement = ''
              end
              if @counter = 5
              begin
                  set @statement5 = @statement
                  set @statement = ''
              end
              if @counter = 6
              begin
                  set @statement6 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 7
              begin
                  set @statement7 = @statement
                  set @statement = ''
              end
              if @counter = 8
              begin
                  set @statement8 = @statement
                  set @statement = ''
              end
              if @counter = 9
              begin
                  set @statement9 = @statement
                  set @statement = ''
              end
              if @counter = 10
              begin
                  set @statement10 = @statement
                  set @statement = ''
              end
			  if @counter = 11
              begin
                  set @statement11 = @statement
                  set @statement = ''
              end
              if @counter = 12
              begin
                  set @statement12 = @statement
                  set @statement = ''
              end
              if @counter = 13
              begin
                  set @statement13 = @statement
                  set @statement = ''
              end
              if @counter = 14
              begin
                  set @statement14 = @statement
                  set @statement = ''
              end
              if @counter = 15
              begin
                  set @statement15 = @statement
                  set @statement = ''
              end
              if @counter = 16
              begin
                  set @statement16 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 17
              begin
                  set @statement17 = @statement
                  set @statement = ''
              end
              if @counter = 18
              begin
                  set @statement18 = @statement
                  set @statement = ''
              end
              if @counter = 19
              begin
                  set @statement19 = @statement
                  set @statement = ''
              end
              if @counter = 20
              begin
                  set @statement20 = @statement
                  set @statement = ''
              end
              if @counter = 21
              begin
                  set @statement21 = @statement
                  set @statement = ''
              end
              if @counter = 22
              begin
                  set @statement22 = @statement
                  set @statement = ''
              end
              if @counter = 23
              begin
                  set @statement23 = @statement
                  set @statement = ''
              end
              if @counter = 24
              begin
                  set @statement24 = @statement
                  set @statement = ''
              end
              if @counter = 25
              begin
                  set @statement25 = @statement
                  set @statement = ''
              end
              if @counter = 26
              begin
                  set @statement26 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 27
              begin
                  set @statement27 = @statement
                  set @statement = ''
              end
              if @counter = 28
              begin
                  set @statement28 = @statement
                  set @statement = ''
              end
              if @counter = 29
              begin
                  set @statement29 = @statement
                  set @statement = ''
              end
              if @counter = 30
              begin
                  set @statement30 = @statement
                  set @statement = ''
              end
              if @counter > 30 
              begin
		         RAISERROR('Error saving trigger', 16, 1,'')
              end
              set @counter = @counter +1
            end
         end
         --print @statement
         FETCH NEXT FROM ActionTrigger_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
              @OrClause,@step,@ActionCode,@StepActiontype 

      END
     CLOSE ActionTrigger_cursor
     DEALLOCATE ActionTrigger_cursor
    
      --create the statement for the bailaction trigger
			
        exec (@statement1 + @statement2 + @statement3 + @statement4 + @statement5 + 
			@statement6 + @statement7 + @statement8 + @statement9 + @statement10 + 
			@statement11 + @statement12 + @statement13 + @statement14 + @statement15 + 
			@statement16 + @statement17 + @statement18 + @statement19 + @statement20 +
			@statement21 + @statement22 + @statement23 + @statement24 + @statement25 + 
			@statement26 + @statement27 + @statement28 + @statement29 + @statement30
			+@statement)
			
      if @@error !=0
      begin
       print '' + @statement1
        print '' + @statement2 
        print '' + @statement3 
        print '' + @statement4 
        print '' + @statement5 
		print '' + @statement6
        print '' + @statement7 
        print '' + @statement8 
        print '' + @statement9 
        print '' + @statement10 
		 print '' + @statement11
        print '' + @statement12 
        print '' + @statement13 
        print '' + @statement14 
        print '' + @statement15 
		print '' + @statement16
        print '' + @statement17 
        print '' + @statement18 
        print '' + @statement19 
        print '' + @statement20
        print '' + @statement21
        print '' + @statement22 
        print '' + @statement23 
        print '' + @statement24 
        print '' + @statement25 
		print '' + @statement26
        print '' + @statement27 
        print '' + @statement28 
        print '' + @statement29 
        print '' + @statement30  
        print '' + @statement
      end
      --exec (@statement5 + @statement4 + @statement3 + @statement2 + @statement1 + @statement)
END

   -- now create the Acctcode trigger for full and partial reposessions
set @statement1 =''
set @statement2 =''
set @statement3 =''
set @statement4 =''
set @statement5 =''
set @statement6 =''
set @statement7 =''
set @statement8 =''
set @statement9 =''
set @statement10 =''
set @statement11 =''
set @statement12 =''
set @statement13 =''
set @statement14 =''
set @statement15 =''
set @statement16 =''
set @statement17 =''
set @statement18 =''
set @statement19 =''
set @statement20 =''
set @statement21 = '' set @statement22 = '' set @statement23 = '' set @statement24 = '' set @statement25 = ''
set @statement26 = '' set @statement27 = '' set @statement28 = '' set @statement29 = '' set @statement30 = ''
set @statement31 = '' set @statement32 = '' set @statement33 = '' set @statement34 = '' set @statement35 = ''
set @statement36 = '' set @statement37 = '' set @statement38 = '' set @statement39 = '' set @statement40 = ''
set @counter = 1
set @statement = ' if exists ('

set @statement = 'if exists (select * from sysobjects where name =''tr_CMAcctcode'') ' +
                 '   drop trigger tr_CMAcctcode '
execute sp_executesql @statement

if exists( SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,C.StepActiontype 
   from CMStrategyCondition C, CMStrategy S,Cmcondition O
   where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
   and c.condition in ('FullRep','PartRep','Reposs')  and actioncode !=''
   and O.Condition= C.condition 
   --and o.type in ('S','X') 
   and isnull(C.SavedType,'') = 'X' 
   and s.isactive !=0)
BEGIN
   print 'doing Acctcode'
   set @statement = 'create trigger tr_CMAcctcode on Acctcode FOR INSERT ' + @NEWLINE +
   ' /*trigger created automatically by GenerateCollectionsTriggersSP - AA ' + convert(varchar,getdate()) 
   + '*/' + @newline +
   'as declare @code char(3),@acctno ' + @ThaiN + 'char(12),@balance money,@instalment money ' + @newline +
   ' ,@transtypecode varchar(3), @balexcharges money,@strategy varchar(7) ' + @newline +
   ' select @code = code,  @acctno = acctno from Inserted ' +  + @newline 
   +  ' select @strategy = isnull(strategy,''NON'') from CMStrategyAcct where acctno = @acctno and dateto is null ' + @newline
   DECLARE RepoTrigger_cursor CURSOR 
  	   FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,
       C.Operator2,C.OrClause,C.step,C.ActionCode,C.StepActiontype 
      from CMStrategyCondition C, CMStrategy S,Cmcondition O
      where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
      and c.condition in ('PartRep','Fullrep','Reposs')  and actioncode !=''
      and O.Condition= C.condition 
      --and o.type in ('S','X') 
      and isnull(C.SavedType,'') ='X' and s.isactive !=0
      OPEN RepoTrigger_cursor
      FETCH NEXT FROM RepoTrigger_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
              @OrClause,@step,@ActionCode,@StepActiontype 
      WHILE (@@fetch_status <> -1)
      BEGIN
	      IF (@@fetch_status <> -2)
   	   begin
               set @statement = @statement + ' if @strategy =' + '''' + @strategy + '''' + @newline 
   IF @condition = 'PartRep' 
               begin
                  set @statement = @statement + ' and @code = ''PREP'' '
               end
               IF @condition = 'FullRep' 
               begin
                  set @statement = @statement + ' and @code = ''FREP'' '
               end
               IF @condition = 'REPO' or @condition = 'REPOSS' OR @condition = 'Repval'
               begin
                  set @statement = @statement + ' and @code IN (''FREP'',''PREP'') '
               end
               set @statement = @statement + @newline + ' begin ' + @newline +
                                             '   update CMStrategyAcct set dateto = @currentDate where acctno = @acctno and strategy = @strategy and dateto is null' + @newline +
                                       '   ' + @newline +
                                       '   insert into CMStrategyAcct ' + @newline +
                                       '  (Acctno,strategy,datefrom,dateto,currentstep,dateincurrentstep) ' + @newline +
                                       '  values(@acctno , ' + '''' + @actioncode + '''' + ',  @currentDate , null,0, @currentDate  ) ' + @newline +
                                       ' end ' + @newline
 
            if datalength(@statement) > 3000 -- we are running near the 6000 maximum so push this across to another statement
            begin
              if @counter = 1
              begin
                  set @statement1 = @statement
                  set @statement = ''
              end
              if @counter = 2
              begin
                  set @statement2 = @statement
                  set @statement = ''
              end
              if @counter = 3
              begin
                  set @statement3 = @statement
                  set @statement = ''
              end
              if @counter = 4
              begin
                  set @statement4 = @statement
                  set @statement = ''
              end
              if @counter = 5
              begin
                  set @statement5 = @statement
                  set @statement = ''
              end
              if @counter = 6
              begin
                  set @statement6 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 7
              begin
                  set @statement7 = @statement
                  set @statement = ''
              end
              if @counter = 8
              begin
                  set @statement8 = @statement
                  set @statement = ''
              end
              if @counter = 9
              begin
                  set @statement9 = @statement
                  set @statement = ''
              end
              if @counter = 10
              begin
                  set @statement10 = @statement
                  set @statement = ''
              end
			  if @counter = 11
              begin
                  set @statement11 = @statement
                  set @statement = ''
              end
              if @counter = 12
              begin
                  set @statement12 = @statement
                  set @statement = ''
              end
              if @counter = 13
              begin
                  set @statement13 = @statement
                  set @statement = ''
              end
              if @counter = 14
              begin
                  set @statement14 = @statement
                  set @statement = ''
              end
              if @counter = 15
              begin
                  set @statement15 = @statement
                  set @statement = ''
              end
              if @counter = 16
              begin
                  set @statement16 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 17
              begin
                  set @statement17 = @statement
                  set @statement = ''
              end
              if @counter = 18
              begin
                  set @statement18 = @statement
                  set @statement = ''
              end
              if @counter = 19
              begin
                  set @statement19 = @statement
                  set @statement = ''
              end
              if @counter = 20
              begin
                  set @statement20 = @statement
                  set @statement = ''
              end
              if @counter = 21
              begin
                  set @statement21 = @statement
                  set @statement = ''
              end
              if @counter = 22
              begin
                  set @statement22 = @statement
                  set @statement = ''
              end
              if @counter = 23
              begin
                  set @statement23 = @statement
                  set @statement = ''
              end
              if @counter = 24
              begin
                  set @statement24 = @statement
                  set @statement = ''
              end
              if @counter = 25
              begin
                  set @statement25 = @statement
                  set @statement = ''
              end
              if @counter = 26
              begin
                  set @statement26 = @statement    
                             
                  set @statement = ''                  
              end
              if @counter = 27
              begin
                  set @statement27 = @statement
                  set @statement = ''
              end
              if @counter = 28
              begin
                  set @statement28 = @statement
                  set @statement = ''
              end
              if @counter = 29
              begin
                  set @statement29 = @statement
                  set @statement = ''
              end
              if @counter = 30
              begin
                  set @statement30 = @statement
                  set @statement = ''
              end
              if @counter > 30  
              begin
		         RAISERROR('Error saving trigger', 16, 1,'')
              end
              set @counter = @counter +1
            end
         end
         --print @statement
         FETCH NEXT FROM RepoTrigger_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
              @OrClause,@step,@ActionCode,@StepActiontype 
        END
      
     CLOSE RepoTrigger_cursor
     DEALLOCATE RepoTrigger_cursor
    
      --create the statement for the Acctcode trigger
         exec (@statement1 + @statement2 + @statement3 + @statement4 + @statement5 + 
			@statement6 + @statement7 + @statement8 + @statement9 + @statement10 + 
			@statement11 + @statement12 + @statement13 + @statement14 + @statement15 + 
			@statement16 + @statement17 + @statement18 + @statement19 + @statement20 +
			@statement21 + @statement22 + @statement23 + @statement24 + @statement25 + 
			@statement26 + @statement27 + @statement28 + @statement29 + @statement30
			+@statement)
      --exec (@statement1 + @statement2 + @statement3 + @statement4 + @statement5 + @statement)
      if @@error > 0
      begin
        select datalength(@statement5)
     select datalength(@statement4)
        select datalength(@statement3)
        select datalength(@statement2)
        select datalength(@statement1)
        select datalength(@statement6)
        select datalength(@statement7)
        select datalength(@statement8)
        select datalength(@statement9)
        select datalength(@statement10)
        select datalength(@statement)
        
        print '' + @statement1
        print '' + @statement2 
        print '' + @statement3 
        print '' + @statement4 
        print '' + @statement5 
		print '' + @statement6
        print '' + @statement7 
        print '' + @statement8 
        print '' + @statement9 
        print '' + @statement10 
		print '' + @statement11
        print '' + @statement12 
        print '' + @statement13 
        print '' + @statement14 
        print '' + @statement15 
		print '' + @statement16
        print '' + @statement17 
        print '' + @statement18 
        print '' + @statement19 
        print '' + @statement20
        print '' + @statement21
        print '' + @statement22 
        print '' + @statement23 
        print '' + @statement24 
        print '' + @statement25 
		print '' + @statement26
        print '' + @statement27 
        print '' + @statement28 
        print '' + @statement29 
        print '' + @statement30
        print '' + @statement
      end
END


GO 

-- this needs to be run as part of the upgrade so needs to be left in. 
exec CM_GenerateCollectionsTriggersSP @return = 0

/* 
SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,C.StepActiontype 
   from CMStrategyCondition C, CMStrategy S,Cmcondition O
   where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
   and c.condition in ('FullRep','PartRep','Reposs')  and actioncode !=''
   and O.Condition= C.condition 
   --and o.type in ('S','X') 
   and isnull(C.SavedType,'') = 'X' 
   and s.isactive !=0*/
