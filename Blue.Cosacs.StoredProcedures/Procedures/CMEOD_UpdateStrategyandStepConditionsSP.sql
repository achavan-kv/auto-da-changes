
if exists (select * from sysobjects where name ='CMEOD_UpdateStrategyandStepConditionsSP')
drop procedure CMEOD_UpdateStrategyandStepConditionsSP
go

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[CMEOD_UpdateStrategyandStepConditionsSP]
-- ==========================================================================================================
-- Version	: 002
-- Project      : CoSACS .NET
-- File Name    : CMEOD_UpdateStrategyandStepConditionsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Credit Collections -Update Strategy and Step Conditions
-- Author       : Alex Ayscough
-- Date         : 2007
--
-- This procedure will Update the Strategy and Step Conditions.
-- 
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 25/06/09  jec Credit Collection Process steps changes
-- 09/07/09  jec Credit Collection Process steps changes - InStrat
-- 14/07/09	 jec NoMove condition  
-- 15/07/09	 jec PayRecv condition  
-- 16/07/09 IP/JEC cast to decimal when dividing integers as otherwise result will be an integer. 
-- 28/07/09  jec UAT fixes
-- 30/07/09  jec UAT767 New Strategy Variable tables
-- 31/07/09  jec UAT733	allow for variable in SRA condition  	
-- 21/08/09  IP  UAT(799)  
-- 02/09/09  IP  UAT(828) Was incorrectly updating 'HoldDays' and 'HoldAccPTP' conditions in the CMStrategyVariablesStep table
--						  before an account was in the strategy and step that the condition is meant to be updated for.   
-- 08/09/09  IP  UAT(809) Added conditions 'SPAB4DUEDAY', 'SPAARREXP' and modified the 'SPALESS' condition. 
-- 09/09/09  jec UAT835 add minus sign in dateadd calc
-- 11/09/09  jec UAT855 PTP include NON arrears strategy in ArrearsPC condition check
-- 16/09/09  IP  UAT(868) If an account is in a strategy that checks the 'PrevStrat' condition, this condition should always be set 
--						  to 'Y' so that it can be proceQssed when processing the steps for 'PrevStrat'.
-- 15/12/09  IP  UAT(937) Condition 'SRC' changed operator to >. Only accounts that have closed Service requests where one was closed more than 30 days ago will exit to NON.
-- 23/12/09  IP  UAT(937) Modified the 'DAYARRS' condition logic as previously was not using the @Operand.
-- 13/01/10 jec UAT967 Corrected AllocXtimes & AllocXtimesSB routines
-- 20/01/10 jec UAT846 Check previous arrears daily entry for DAYARRS
-- 27/04/10 jec UAT1003 Deferred Account strategy
-- 14/06/10 IP  UAT11 UAT5.2 External - Added Id to key due to duplicate key error.
-- 07/02/12 jec #9521 CR9417 - duplication of existing strategies
-- 09/02/12 jec #9519 CR9417 - SC existing days in arrears 
-- 09/02/12 jec #9512 CR9417 - new SC due date condition
-- 09/02/12 jec #9517 CR9417 - new SC balance condition 
-- 10/02/12 jec #9513 CR9417 - new SC Arrears condition
-- 13/02/12 jec #9516 CR9417 - new SC multiple missed payments
-- 13/02/12 jec #9520 CR9417 - Storecard existing recurring arrears
-- 14/02/12 ip  Replace GETDATE() with @rundate
-- 16/02/12 jec #9519 CR9417 - SC existing days in arrears  - cast hours/24 as decimal
-- 07/03/12 ip  #9735 - LW74766 - settled accounts in Bailiff Strategy. Once an account 
--				is settled it should leave the Bailiff strategy. Checking on Instalamount = 0.
-- 08/03/12 jec #9742 CR9417 - New SC Payments in arrears condition
-- 12/03/12 jec #9778 Account is in wrong strategy instead of SC Low Risk
-- 13/03/12 jec #9790 Changed condition to work same way as 'XINSMS' (i don't think it's correct though - operand should work on first operator)
-- 13/03/12 JC/IP #9793 - Changed SCORE condition to cater for Store Card.
-- 14/03/12 JC/IP #9794 - Changed RECARREARS condition to cater for Store Card. Previously was incorrectly checking for missed payments. Should check Customer.Recurringarrears
-- 10/04/12 JC/IP #9846  Storecard - months in arrears
-- 13/04/12 jec  #9804 Accounts in SCCOL (Store Card Collector) strategy are not allocated to a collector
-- 17/04/12 IP   #9884 - Accounts written off whilst in a strategy for e.g. SCMR were not leaving the strategy to SCNON and then entering SCPWO. They remained in the strategy.
--				 Changed condition ArrearsPCSC to check for arrears = 0
-- 27/11/12 IP   #10912 - Integration - Push to CoSACS on Logging - Updating condition to look at new table SR_Summary (Service now moved to web)
-- 07/08/19 Zensar  Optimised the stored procedure for performance by putting Nolock and replcaing * with 1 in all exist checks.
-- 09/08/19 Zensar Strategy Job Optimization : Store Card functionality check based on country maintenance 
-- ============================================================================================================



	@lastrundate datetime,
	@rundate datetime,						--IP - 13/02/12
	@loadtype varchar(12),
	@return int OUT
AS
	
		declare --@firstofmonth datetime,
		@rowcount int,
		@newline varchar(64)
		set @newline =''

		-- Get Months no Move value from country parameters for condition 'NoMove'		-- jec 14/07/09
		declare @mthsNoMove varchar(2)
		declare @strategy varchar(6)
		select @mthsNoMove=cast(value as varchar(2)) from countrymaintenance With(NoLock) where codename='mthsreposs'

		if OBJECT_ID('#prevArrears','U') is not null 
		drop table #prevArrears
				
		if OBJECT_ID('#missedpay','U') is not null 
		drop table #missedpay
		
		if OBJECT_ID('#arrearsmths','U') is not null 
		drop table #arrearsmths

		--IP/JC - 09/12/09 - Commented out the updates to Agreement.Datenextdue (should be done by arrearscalc)




		--set @firstofmonth = dateadd(day,-datepart(day,getdate()),getdate())
		--update agreement set datenextdue = dateadd(day,datepart(day,i.datefirst)-1,@firstofmonth)
		--from instalplan i,acct a
		--where i.acctno = agreement.acctno 
		--and a.acctno = i.acctno and i.agrmtno=1 and agreement.agrmtno=1
		--and a.outstbal>0 and a.agrmttotal >0 and isnull(agreement.datedel,'1-jan-1900') !='1-jan-1900'










		--update agreement set datenextdue =dateadd(month,1,datenextdue)
		--from instalplan i, acct a 
		--where i.acctno = agreement.acctno 
		--and a.acctno = i.acctno and i.agrmtno=1 and agreement.agrmtno=1
		--and a.outstbal>0 and a.agrmttotal >0 and isnull(agreement.datedel,'1-jan-1900') !='1-jan-1900'
		--and agreement.datenextdue <getdate()

		declare @statement sqltext,@statement2 sqltext
		declare @Entrystatement sqltext,
				@Stepsstatement sqltext,
				@Exitstatement sqltext,
				@StrategyVariabletable varchar(30)
		  
		declare    @Type char(1) ,
		   @Condition varchar(12) ,
		   @Operand varchar(10), --- between >< = 
		   @Operator1 varchar(24), -- value.....
		   @Operator2 varchar(24), -- 
		   @OrClause char(1),  --  can be a/b/c/1/2/3 - used to match orclauses
		   @step smallint,
		   @ActionCode varchar(12),
		   @StepActiontype char(1), @type1 char(1),@type2 char(1),@removestatement sqltext,
		   @ConditionType char(1)	-- UAT767 jec
   
			if @loadtype = 'Entry' -- we will populate entry conditions
				begin
					set @type1 = 'N'
					set @type2 ='N'
				end
			else
				 begin  -- we will populate step and exit conditions
				   set @type1 ='S'
				   set @type2='X'
				 end
 
			-- select previous arrearsdaily to temp table for DAYARRS check UAT846 jec 20/01/10
			select i.acctno, coalesce(MAX(a.dateto), 
			(select dateadd(day, -1, min(datefrom))	from arrearsdaily x WITH(NoLock) where x.acctno = i.acctno),
				--getdate()) as dateenteredarrears
			@rundate) as dateenteredarrears						--IP - 13/02/12 - use @rundate
			into #prevArrears
			from instalplan i  WITH(NoLock)
			left outer join ArrearsDaily a  WITH(NoLock)
			on  a.dateto is not null
			and a.acctno=i.acctno
			--and a.arrears < .2 * i.instalamount
			and ((a.arrears < .2 * i.instalamount and a.acctno not like '___9%')		-- #9778 Credit Accounts
				or (a.arrears =0 and a.acctno like '___9%'))								-- #9804 #9778 StoreCard Accounts
			group by i.acctno
	

			alter TABLE #prevArrears ADD  CONSTRAINT [pk_prevArrears] PRIMARY KEY CLUSTERED 
			(

				[acctno] ASC
			)

			-------------- Modified By Zensar on 08/07/2019 ------- Start

			IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')
			BEGIN


				--#9516 multiple missed payments - need to get payment dates and due amounts 
					select distinct acctno,dateto,datepaymentdue,stmtminpayment,CAST(0 as MONEY) as payment
					into #missedpay
					from StoreCardStatement s  WITH(NoLock)

					alter TABLE #missedpay ADD  CONSTRAINT [pk_missedpay] PRIMARY KEY CLUSTERED 
					(
						[acctno] ASC,dateto asc
					)
					-- update with payments received by due date - only payments received by due date count
					UPDATE #missedpay set payment=payment + transvalue 
					from fintrans f  WITH(NoLock), #missedpay m 
					where f.acctno=m.acctno and transtypecode='PAY'
					and datetrans between dateto and DATEADD(d,1,datepaymentdue)		--#9795	



					-----Modified by Zensar on 08/08/2019 as the account number mentioned is hard coded-------Start	
					-- #9846  Storecard - months in arrears
					select 
					--distinct d.acctno,arrears,d.datefrom,CAST(0 as MONEY) as minpayment,CAST(0 as DECIMAL(5,2)) as MonthsArrs 
					TOP 1 d.acctno,arrears,d.datefrom,CAST(0 as MONEY) as minpayment,CAST(0 as DECIMAL(5,2)) as MonthsArrs 
					into #arrearsmths
					from ArrearsDaily d  WITH(NoLock) INNER JOIN storecardstatement s  WITH(NoLock) on d.acctno=s.acctno
					where d.arrears>0
					--where d.datefrom > DATEADD(m,-8,'2013-09-09 10:52:39.030') and d.arrears>0
					--and d.acctno='780900022591'

					TRUNCATE TABLE #arrearsmths

					alter TABLE #arrearsmths ADD  CONSTRAINT [pk_ArrsMths] PRIMARY KEY CLUSTERED 
					(

						[acctno] ASC,datefrom asc
					)

					 --update with minimum payment
					UPDATE #arrearsmths set minpayment=prevstmtminpayment,MonthsArrs=m.Arrears/prevstmtminpayment
					from storecardstatement s  WITH(NoLock), #arrearsmths m 


					where s.acctno=m.acctno 
					and s.datepaymentdue=(select MAX(datepaymentdue) from storecardstatement s2 where m.acctno=s2.acctno and s2.datepaymentdue<m.datefrom)








					---Commented out by Zensar on 08/08/2019 as the account number mentioned is hard coded-------End	










			END
			-------------- Modified By Zensar on 08/07/2019 ------- END


















			IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')
			BEGIN
				DECLARE StrategyCondition_cursor CURSOR 
				FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,
				C.ActionCode,C.StepActiontype,c.savedtype 
				from CMStrategyCondition C  WITH(NoLock), CMStrategy S  WITH(NoLock)
				where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
				and c.condition in ('DayX','HoldAccPTP','HoldDays','HoldDDMin','MINXMN','Pays','PTPBROKE','PAYLESSINST',	-- UAT1003
				'PTPLESS','RETCHQ','Score','SPALESS','SPANODAYS','PartPay','RECARRS','SCMTHS','BALEXCHARGES',
				'ArrearsPC' ,'ARRSPC','balanc','dayarrs','XINSMS','ArrPaid','SRC','AllocXtimes','AllocXtimeSB','B4DUEDAY', 'SPAB4DUEDAY', 'SPAARREXP', -- IP - 03/09/09 - UAT(809) - Added 'SPAB4DUEDAY' --IP - 07/09/09 - Added 'SPAARREXP'
				'PaidPcent','Grace','InStrat','PrevStrat', 'MthArrMths', 'MaxItemVal', 'PayRecv', 'NoMove', 'WoffRepo', 'NoFollup',		-- 	25/06/09, IP - 14/07/09 - Added 'MthArrMths' and 'MaxItemVal', IP - 16/07/09 - Added 'WoffRepo', 'NoFollup'	
				'SRA','ProvisionPct','ProvisionAmt',  -- UAT733 
				'ArrearsPCSC','XPAYMS','BalSCard','DaysDue','MthArrMthsSC')		-- #9521 #9512 #9742
		   		and c.savedtype in (@type1,@type2)
		   
				OPEN StrategyCondition_cursor
				
				FETCH NEXT FROM StrategyCondition_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
				   @OrClause,@step,@ActionCode,@StepActiontype,@ConditionType
			END
			Else
			BEGIN 
		
				DECLARE StrategyCondition_cursor CURSOR 
				FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,
				C.ActionCode,C.StepActiontype,c.savedtype 
				from CMStrategyCondition C  WITH(NoLock), CMStrategy S  WITH(NoLock)
				where S.isActive !=0 AND C.Strategy = S.Strategy --and C.Stepactiontype is not null
				and c.condition in ('DayX','HoldAccPTP','HoldDays','HoldDDMin','MINXMN','Pays','PTPBROKE','PAYLESSINST',	-- UAT1003
				'PTPLESS','RETCHQ','Score','SPALESS','SPANODAYS','PartPay','RECARRS','SCMTHS','BALEXCHARGES',
				'ArrearsPC' ,'ARRSPC','balanc','dayarrs','XINSMS','ArrPaid','SRC','AllocXtimes','AllocXtimeSB','B4DUEDAY', 'SPAB4DUEDAY', 'SPAARREXP', -- IP - 03/09/09 - UAT(809) - Added 'SPAB4DUEDAY' --IP - 07/09/09 - Added 'SPAARREXP'
				'PaidPcent','Grace','InStrat','PrevStrat', 'MthArrMths', 'MaxItemVal', 'PayRecv', 'NoMove', 'WoffRepo', 'NoFollup',		-- 	25/06/09, IP - 14/07/09 - Added 'MthArrMths' and 'MaxItemVal', IP - 16/07/09 - Added 'WoffRepo', 'NoFollup'	
				'SRA','ProvisionPct','ProvisionAmt',  -- UAT733 
				'ArrearsPCSC','BalSCard','DaysDue')		-- #9521 #9512 #9742
				 and c.savedtype in (@type1,@type2)
		   
				OPEN StrategyCondition_cursor
				
				FETCH NEXT FROM StrategyCondition_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
				   @OrClause,@step,@ActionCode,@StepActiontype,@ConditionType
			End
	   
			WHILE (@@fetch_status <> -1)
			BEGIN
				IF (@@fetch_status <> -2)
   				begin
					-- here we are populating the strategy CMStrategyVariables 
					--set @statement = ' update CMStrategyVariables set ' + @condition + ' = ''Y'' where exists' + @newline + ' ( select * from '
					-- UAT767 jec
					set @Entrystatement = ' update CMStrategyVariablesEntry set ' + @condition + ' = ''Y'' where exists' + @newline + ' ( select 1 from '
					set @Stepsstatement = ' update CMStrategyVariablesSteps set ' + @condition + ' = ''Y'' where exists' + @newline + ' ( select 1 from '
					set @Exitstatement = ' update CMStrategyVariablesExit set ' + @condition + ' = ''Y'' where exists' + @newline + ' ( select 1 from '
					-- set statement & table for correct table depending on condition type
					set @statement=case
						when @ConditionType ='N' then @Entrystatement
						when @ConditionType ='S' then @Stepsstatement
						when @ConditionType ='X' then @Exitstatement
						end 

				   set @StrategyVariabletable=case
						when @ConditionType ='N' then 'CMStrategyVariablesEntry'
						when @ConditionType ='S' then 'CMStrategyVariablesSteps'
						when @ConditionType ='X' then 'CMStrategyVariablesExit'
						end 

					-- Not required now?? 
					 --if @loadtype = 'Entry' -- remove any existing step or exit conditions
					 --begin
					 --   set @removestatement 
					 --   =' update CMStrategyVariables set ' + @condition + ' = '''' where strategy = ' + '''' + @strategy + ''''
					 --   execute sp_executesql @removestatement 
					 --   if @@error !=0
					 --     print @removestatement
					 --end

				--    if @condition ='DayX' -- on day X do... - we are checking that X number of days have passed. Given it could be run at different times at night take off a few hours so that 2 days 9 hours becomes 3 days
				--     begin
				--          set @statement = @statement + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno  ' + @newline + 
				--          ' and CMStrategyVariables.strategy =' + '''' + @strategy + '''' + @newline + 
				--          ' and a.dateto is null and datediff(hour,dateadd(hour,-4,a.datefrom),getdate())/24 ' + '>=' +  @Operator1  
				--     end
     
				--IP - UAT(536)
					if @condition ='DayX' -- on day X do... - we are checking that X number of days have passed. Given it could be run at different times at night take off a few hours so that 2 days 9 hours becomes 3 days
					begin
						--set @statement = @statement + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno  ' + @newline +
						--' and CMStrategyVariables.strategy =' + '''' + @strategy + '''' + @newline +  
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno  ' + @newline + 
						' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						--' and a.dateto is null and datediff(hour,dateadd(hour,-4,a.datefrom),getdate())/24 ' + '>=' +  @Operator1 +
						' and a.dateto is null and datediff(hour,dateadd(hour,-4,a.datefrom),' + '''' + convert(varchar,@rundate) + '''' + ')/24 ' + '>=' + @Operator1 +
						' and a.strategy =' + '''' + @strategy + '''' + @newline + 
						' and a.currentstep =' + convert(varchar,@Step)
					end


					if @condition = 'ArrPaid' -- at least x % of arrpaid. 
					 begin 
        
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) ,fintrans f  WITH(NoLock) ,spa s ' +
						--' where a.acctno = CMStrategyVariables.acctno and f.acctno = a.acctno  ' + @newline + 
						--' and  CMStrategyVariables.strategy =' + '''' + @strategy + '''' + @newline + 
						' where a.acctno = ' + @StrategyVariabletable + '.acctno and f.acctno = a.acctno  ' + @newline + 
						' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						' and -f.transvalue* ' + @Operator1 + '/100   > s.spainstal' + @newline +        
						' and a.dateto is null and f.transtypecode in (''PAY'',''DDN'') and f.datetrans> '  + '''' + convert(varchar,@lastrundate) + ''''
					 end

					 


					 if @condition = 'XINSMS' --Customer missed X instalment after account being setup for Y months 
					 begin
						 --set @statement = @statement + '  instalplan i, acct a where a.acctno = CMStrategyVariables.acctno and ' +
						 --' CMStrategyVariables.strategy =' + '''' + @strategy + '''' + @newline + 
						 set @statement = @statement + '  instalplan i  WITH(NoLock), acct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno and '
						 + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						 -- so if X instalments in arrears then  effectively arrears /X > instalment
						 ' and a.acctno = i.acctno and a.arrears/' + convert(varchar,@Operator1) + '>=.5*i.instalamount ' +
						 ' and a.AcctType!=''T'' '	+	-- #9521  Not StoreCard
						 -- and account has been setup for at least Y months
					   --' and datediff(day,i.datefirst,getdate())/30.34  '+ @Operand + @Operator2  
						 ' and datediff(day,i.datefirst,' + '''' + CONVERT(VARCHAR, @rundate) + '''' + ')/30.34 '+ @Operand + @Operator2	--IP - 14/02/12 - use @rundate
					 END

     
					 if @condition = 'XPAYMS' -- #9516 Customer missed X payments after account being setup for Y months 
					 begin 
						  set @statement=REPLACE(@statement,'*','COUNT(*)')				-- need to count 
						  set @statement = @statement +' #missedpay m  ' + @newline +          
						  ' where m.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
						  ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						  ' and stmtminpayment+payment > 0' + @newline + 
						  --' and datepaymentdue < (select DATEADD(m,cast(' + @Operator2 + ' as INT),MIN(datetrans)) from fintrans f where m.acctno=f.acctno and f.transtypecode=''SCT'')' +
						  --' having COUNT(*) ' + @Operand + ' cast(' + @Operator1 + ' as INT) '			-- jec 12/03/12 
						  ' and datepaymentdue ' + @Operand + ' (select DATEADD(m,cast(' + @Operator2 + ' as INT),MIN(datetrans)) from fintrans f  WITH(NoLock) where m.acctno=f.acctno and f.transtypecode=''SCT'')' +
						  ' having COUNT(*) >= cast(' + @Operator1 + ' as INT) '			-- #9846 #9790 jec 13/03/12 
					 end 

     
					 if @condition ='DAYARRS' -- number of days in arrears. Use the arrears daily
					 -- set it to true unless there exists an arrears amount < 90% of one instalment within the last seven days
					 begin
						  set @statement=REPLACE(@statement,'exists','(exists')			-- #9519 open bracket required because of OR condition
						  set @statement = @statement +' acct a  WITH(NoLock), instalplan i  WITH(NoLock)  ' + @newline + 
						  --' where a.acctno = CMStrategyVariables.acctno ' +@newline + 
						  --' and CMStrategyVariables.strategy =' + '''' + @strategy + '''' + @newline + 
						  ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
						  ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						  ' and a.acctno  = i.acctno and a.arrears >=.2 * i.instalamount and a.outstbal >=.2 * i.instalamount ' +
						  ' and a.Accttype!=''T'' '  +		-- not storecard
						  ' and exists (select 1 from arrearsdaily d  WITH(NoLock) where d.acctno = i.acctno and '  + @newline +
						  ' d.datefrom = ( ' + @newline +
						  ' select MIN(datefrom) '  + @newline +
						  ' from ArrearsDaily ad, #prevArrears p '  + @newline +
						  ' where p.acctno = ad.Acctno '  + @newline +
						  ' and ad.acctno = a.acctno '  + @newline +
						  ' and ad.datefrom > = p.dateenteredarrears  '  + @newline +
						  ' ) and '  + @newline +
						  --' (datediff(hour, d.datefrom, getdate())/24 ' + convert(varchar,@Operand) + ' ' + CONVERT(VARCHAR, @Operator1) + '))' + @newline +
							' (cast(datediff(hour, d.datefrom,' + '''' + convert(varchar,@rundate) + '''' + ')/24.00 as DECIMAL (7,2)) ' + convert(varchar,@Operand) + ' ' + convert(varchar, @Operator1) + '))' + @newline +		-- jec 17/02/12 cast hours/24 as decimal --IP - 14/02/12 - use @rundate
						  ')' +

						  -- check previous arrearsdaily entry, as if in arrears shouldn't wait for 7 days  UAT846 jec 20/01/10
						  --' or exists (select * from #prevArrears d2 where d2.acctno = a.acctno)) )'
          
						  -- Storecard accounts #9519
						  ' or exists (select 1 from  acct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
						  ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						  '	and a.arrears >0 and a.outstbal >0 and a.Accttype=''T'' ' + @newline + 		  
						  ' and exists (select 1 from arrearsdaily d  WITH(NoLock) where d.acctno = a.acctno and  '  + @newline +
						  ' d.datefrom = (  select  MIN(datefrom)  from ArrearsDaily ad  WITH(NoLock), #prevArrears p  
											where p.acctno = ad.Acctno  and ad.acctno = a.acctno  and ad.datefrom > = p.dateenteredarrears ) and ' + @newline +
						  --' (datediff(hour, d.datefrom, getdate())/24 ' + convert(varchar,@Operand) + ' ' + CONVERT(VARCHAR, @Operator1) + '))' + @newline +
						   ' (cast(datediff(hour, d.datefrom, ' + '''' + convert(varchar,@rundate) + '''' + ')/24.00 as DECIMAL (7,2))' + convert(varchar,@Operand) + ' ' + convert(varchar, @Operator1) + '))' + @newline +		-- jec 17/02/12 cast hours/24 as decimal	--IP - 14/02/12 - use @rundate
						  ')'          

          
					 end
					
					if exists(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')  ------if condition added by Zensar on 30/08/2019----
						begin
							if @condition ='DaysDue' -- #9512 number of days after DueDate - StoreCard
							begin
								set @statement = @statement +' acct a  WITH(NoLock), StoreCardPaymentDetails p  WITH(NoLock) ' + @newline + 
								  --' where a.acctno = CMStrategyVariables.acctno ' +@newline + 
								  --' and CMStrategyVariables.strategy =' + '''' + @strategy + '''' + @newline + 
								  ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
								  ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
								  ' and a.acctno  = p.acctno and a.outstbal > 0 ' +
								  ' and a.Accttype=''T'' '  +		-- storecard account         
								  --' and getdate() ' + convert(varchar,@Operand) + ' ' + 'dateadd(day, ' + @Operator1 + ', p.DatePaymentDue) ' + @newline +
								  ' and ' + '''' + convert(varchar,@rundate) + '''' + convert(varchar,@Operand) + ' ' + 'dateadd(day, ' + @Operator1 + ', p.DatePaymentDue) ' + @newline +		--IP - 14/02/12 - use @rundate
								  ' and not exists(select 1 from fintrans f WITH(NoLock) where f.acctno=a.acctno and transtypecode=''PAY'' and f.datetrans >= ' + @newline +
								  ' (select  max(s.dateto) from StoreCardStatement s  WITH(NoLock) where s.acctno=a.acctno ) )'

     
							 end
						 end
















					if @condition ='HoldAccPTP' --Hold account for ?x? days before PTP instalment due date
					begin 
   
						 --IP - 21/08/09 - UAT(799) - Hold the account up until x days before the PTP Instalment due date.
						 --Changed to update = 'Y' if the condition has been met for consistency with all other conditions.
						 --IP - 02/09/09 - 5.2 UAT(828) - Was incorrectly updating the condition before an account was in the strategy and step the condition is meant to be updated for.
						  set @statement = @statement + ' bailactionPTP b WITH(NoLock), cmstrategyacct a WITH(NoLock)'   + @newline --where b.code =''PTP'' ' 
							--'b.acctno = CMStrategyVariables.acctno and ' + @newline 
							+' where b.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline 
							+'and a.acctno = b.acctno ' + @newline
							+'and a.strategy = ' + '''' + @strategy + '''' + @newline
							+'and a.currentstep = ' + convert(varchar,@Step) + @newline
							+'and a.dateto is null ' + @newline
							+'and b.dateadded = (select  MAX(dateadded) from bailactionPTP b1  WITH(NoLock) where b1.acctno = b.acctno ) ' +
							--'and getdate() >= dateadd(day,-'  + @Operator1+ ',b.datedue) '
							'and ' + '''' + convert(varchar, @rundate) + '''' + ' >= dateadd(day,-'  + @Operator1+ ',b.datedue) '		--IP - 14/02/12 - use @rundate
 
							set @statement2 = ' update ' + @StrategyVariabletable + ' set ' + @condition + ' = ''R'' where exists ( select 1 from '
							+ ' bailactionPTP b WITH(NoLock), cmstrategyacct a  WITH(NoLock) where b.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy = ' + '''' + @strategy + '''' + @newline  
							+ ' and a.acctno = b.acctno ' 
							+ ' and a.strategy = ' + '''' + @strategy + ''''
							+ ' and a.currentstep = ' + convert(varchar,@Step)
							+ ' and a.dateto is null'
							 + ' and b.dateadded = (select MAX(dateadded) from bailactionPTP b1  WITH(NoLock) where b1.acctno = b.acctno  ' 
							--+ 'and getdate() < dateadd(day,-'  + @Operator1+ ',b.datedue)))'
							+ ' and ' + '''' + convert(varchar, @rundate) + '''' + ' < dateadd(day,-'  + @Operator1+ ',b.datedue)))'		--IP - 14/02/12 - use @rundate

					 end    


					 if @condition ='AGRNOTEXP' --Agreement not expired
					 begin 
						--set @statement = @statement + ' instalplan i where i.acctno = CMStrategyVariables.acctno and ' + @newline
						set @statement = @statement + ' instalplan i  WITH(NoLock) where i.acctno = ' 
							+ @StrategyVariabletable + '.acctno and ' + @newline 
							--+ ' i.datelast >getdate()  '
							+ ' i.datelast > '+ '''' + convert(varchar, @rundate) + '''' + ' '			--IP - 14/02/12 - use @rundate  
	     
					 end      



					 if @condition ='HoldDays'--Hold Account for X days in current step
					 begin
						 -- --set @statement = @statement + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' + @newline 
						 -- set @statement = @statement + ' CMStrategyAcct a where a.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy =' + @newline 
						 --      + '''' + @strategy + '''' + ' and dateadd(day,' + @Operator1+ ',a.dateincurrentstep) <= getdate() ' + @newline		
						 ----set @statement2 = ' update CMStrategyVariables set ' + @condition + ' = ''R'' where exists ( select * from ' + @newline 
						 ---- + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' + @newline 
						 -- set @statement2 = ' update ' + @StrategyVariabletable + ' set ' + @condition + ' = ''R'' where exists ( select * from ' + @newline 
						 -- + ' CMStrategyAcct a where a.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy =' + @newline 
						 --      + '''' + @strategy + '''' + ' and dateadd(day,' + @Operator1+ ',a.dateincurrentstep) > getdate() )' 
        
					   --IP - 02/09/09 - 5.2 UAT(828) - Was incorrectly updating the condition before an account was in the strategy and step the condition is meant to be updated for.
						 --set @statement = @statement + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' + @newline 
						  set @statement = @statement + ' CMStrategyAcct a WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
							   + ' and ' + @StrategyVariabletable + '.strategy = ' + '''' + @strategy + '''' + @newline
							   + ' and a.strategy = ' + '''' + @strategy + '''' + @newline
							   + ' and a.currentstep = ' + convert(varchar,@Step) + @newline
							   + ' and a.dateto is null ' + @newline 
							   --+ ' and dateadd(day,' + @Operator1+ ',a.dateincurrentstep) <= getdate() ' + @newline	
							   + ' and dateadd(day,' + @Operator1+ ',a.dateincurrentstep) <= ' + '''' + convert(varchar, @rundate) + '''' + ' ' + @newline		--IP - 14/02/12 - use @rundate
						 --set @statement2 = ' update CMStrategyVariables set ' + @condition + ' = ''R'' where exists ( select * from ' + @newline 
						 -- + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' + @newline 
						  set @statement2 = ' update ' + @StrategyVariabletable + ' set ' + @condition + ' = ''R'' where exists ( select 1 from ' + @newline 
						  + ' CMStrategyAcct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy = ' + '''' + @strategy + '''' + @newline
						  + ' and a.strategy = ' + '''' + @strategy + '''' + @newline
						  + ' and a.currentstep = ' + convert(varchar,@Step) + @newline
						  + ' and a.dateto is null ' + @newline 
						  --+ ' and dateadd(day,' + @Operator1+ ',a.dateincurrentstep) > getdate() )'  
						  + ' and dateadd(day,' + @Operator1+ ',a.dateincurrentstep) > ' + '''' + convert(varchar, @rundate) + '''' + ' )'		--IP - 14/02/12 - use @rundate
					 end

         
         
			--     if @condition ='HoldDDMin'--Hold until due date minus X days
			--     begin
			--          set @statement = @statement + ' agreement g where g.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' 
			--               + '''' + @strategy + '''' + ' and dateadd(day,-' + convert(varchar,@Operator1) + ',g.datenextdue) < getdate() '+ @newline 
			--         set @statement2 = ' update CMStrategyVariables set ' + @condition + ' = ''R'' where exists ' + @newline + ' ( select * from '
			--          + ' agreement g where g.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' 
			--               + '''' + @strategy + '''' + ' and dateadd(day,' + @Operator1 + ',g.datenextdue) > getdate()) '
			--     end
		
					--IP - 17/10/08 - Update the condition = 'Y'if we have passed the min number of days
					--to hold the account before the due date, but have not passed the due date.
					--Update the condition to 'R' if we have passed the due date.
					if @condition IN ('HoldDDMin','B4dueday')--Hold until due date minus X days
					begin
							--set @statement = @statement + ' agreement g where g.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' 
							set @statement = @statement + ' agreement g  WITH(NoLock) where g.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy =' 
							   --+ '''' + @strategy + '''' + ' and dateadd(day,-' + convert(varchar,@Operator1) + ',g.datenextdue) < getdate() and getdate() < g.datenextdue'+ @newline 
								 + '''' + @strategy + '''' + ' and dateadd(day,-' + convert(varchar,@Operator1) + ',g.datenextdue) < ' + '''' + convert(varchar, @rundate) + '''' + '
								 and ' + '''' + convert(varchar, @rundate) + '''' + ' < g.datenextdue' + @newline		--IP - 14/02/12 - use @rundate
							--set @statement2 = ' update CMStrategyVariables set ' + @condition + ' = ''R'' where exists ' + @newline + ' ( select * from '
				   --       + ' agreement g where g.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' 
							set @statement2 = ' update ' + @StrategyVariabletable + ' set ' + @condition + ' = ''R'' where exists ' + @newline + ' ( select * from '
							+ ' agreement g where g.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy ='
							   --+ '''' + @strategy + '''' + ' and getdate() > g.datenextdue) '
							+ '''' + @strategy + '''' + ' and ' + '''' + convert(varchar, @rundate) + '''' + ' > g.datenextdue) '		--IP - 14/02/12 - use @rundate
					end
		
					
					--IP - 03/09/09 - UAT(809) - Added condition to check if todays date is x number of days before the next SPA due date.
					--IP - 07/09/09 - UAT(809) - Changed condition to use the SPASummary table which is populated from the Credit Collections EOD.
					if @Condition = 'SPAB4DUEDAY'
					begin
		
							set @statement = @statement + ' spasummary s  WITH(NoLock), cmstrategyacct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
							+ ' and a.acctno = s.acctno ' + @newline
							+ ' and a.strategy = ' + '''' + @strategy + '''' + @newline
							+ ' and a.currentstep = ' + convert(varchar, @step) + @newline
							+ ' and a.dateto is null ' + @newline
							+ ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline
							--+ ' and s.dateexpiry = (select min(s1.dateexpiry) from spa s1 ' + @newline
							--+ ' where s1.dateexpiry > getdate() ' + @newline
							--+ ' and s1.acctno = s.acctno ) ' + @newline
							--+ ' and getdate() >= dateadd(day, -' + convert(varchar, @Operator1) + ',s.NextDateExpiryDue) ' + @newline
							+ ' and ' + '''' + convert(varchar, @rundate) + '''' + ' >= dateadd(day, -' + convert(varchar, @Operator1) + ',s.NextDateExpiryDue) ' + @newline	--IP - 14/02/12 - use @rundate
							--+ ' and getdate() < s.NextDateExpiryDue ' 
							+ ' and ' + '''' + convert(varchar, @rundate) + '''' + ' < s.NextDateExpiryDue '	--IP - 14/02/12 - use @rundate
			
						
							set @statement2 =  'update ' + @StrategyVariabletable + ' set ' + @condition + ' = ''R'' where exists ' + @newline
							+ ' ( select 1 from spasummary s  WITH(NoLock), cmstrategyacct a WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
							+ ' and a.acctno = s.acctno ' + @newline
							+ ' and a.strategy = ' + '''' + @strategy + '''' + @newline
							+ ' and a.currentstep = ' + convert(varchar, @step) + @newline
							+ ' and a.dateto is null ' + @newline
							+ ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline
							--+ ' and s.dateexpiry = (select min(s1.dateexpiry) from spa s1 ' + @newline
							--+ ' where s1.dateexpiry > getdate() ' + @newline
							--+ ' and s1.acctno = s.acctno ) ' + @newline
							--+ ' and getdate() < dateadd(day, -' + convert(varchar, @operator1) + ',s.NextDateExpiryDue)) '
							+ ' and ' + '''' + convert(varchar, @rundate) + '''' + ' < dateadd(day, -' + convert(varchar, @operator1) + ',s.NextDateExpiryDue)) ' --IP - 14/02/12 - use @rundate
						
					end

					if @condition ='MINXMN'--Missed instalment within X months of account being opened 
					begin
							--set @statement = @statement + ' instalplan i,acct a where a.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' + @newline 
							set @statement = @statement + ' instalplan i  WITH(NoLock) ,acct a  WITH(NoLock) where a.acctno =  ' + @StrategyVariabletable + '.acctno and  ' + @StrategyVariabletable + '.strategy =' + @newline  
							--+ '''' + @strategy + '''' + ' and a.arrears > .9 * i.instalamount and a.acctno = i.acctno and dateadd(month,' + @Operator1+ ',a.dateacctopen) < getdate() ' 
							+ '''' + @strategy + '''' + ' and a.arrears > .9 * i.instalamount and a.acctno = i.acctno and dateadd(month,' + @Operator1+ ',a.dateacctopen) '
							+ ' < ' + '''' + convert(varchar, @rundate) + '''' + ' '	--IP - 14/02/12 - use @rundate
					 end


					/* if @condition ='NOTINSTRAT'--Account is not in strategy X 
					 begin     

					   set @statement = @statement +
					 end*/

					 if @condition ='Pays'--Customer Makes payment resulting in arrears < $ value
					 begin
						  --set @statement = @statement + ' acct a,fintrans f where a.acctno =  CMStrategyVariables.acctno and CMStrategyVariables.strategy =' + @newline 
						  set @statement = @statement + ' acct a  WITH(NoLock) ,fintrans f  WITH(NoLock) where a.acctno =  ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy =' + @newline 
							   + '''' + @strategy + '''' + ' and a.arrears <' + @Operator1+ ' and f.acctno= a.acctno and f.transtypecode ' +
							   ' in (''PAY'',''DDN'') AND f.transvalue <0 and f.datetrans > ' + '''' + convert(varchar,@lastrundate) + ''''
					 end


					 if @condition ='PTPBROKE'--Promise to pay not received after X days 
					 begin
						  set @statement = @statement + ' bailactionPTP b WITH(NoLock) where b.acctno = ' + @StrategyVariabletable + '.acctno and ' + @newline 
						   --+ ' getdate() < dateadd(day,'  + @Operator1+ ',b.datedue) '
						   + ' ' + '''' + convert(varchar, @rundate) + '''' + ' < dateadd(day,'  + @Operator1+ ',b.datedue) '		--IP - 14/02/12 - use @rundate
					 end


					--IP - 21/08/09 - UAT(799) - Passing in the @Operand - operator used and changed logic.
					if @condition ='PTPLESS'--Customer paid less than X of PTP instalment -- so bailaction due and customer pays less than x amount....
					begin
						  set @statement = @statement + ' bailactionPTP b WITH(NoLock) where b.acctno = ' + @StrategyVariabletable + '.acctno and ('+ @newline 
						   + ' b.dateadded = (select MAX(dateadded) from bailactionPTP b1  WITH(NoLock) where b1.acctno = b.acctno ) ' +
						   + ' and (select isnull(sum(-transvalue),0) from fintrans f '
						   + ' where f.acctno = b.acctno and f.datetrans > b.dateadded and f.transtypecode in (''PAY'',''DDN'',''COR'',''REF'',''RET'' ) ) ' 
						   + @Operand + 'cast(' + @Operator1 + ' as decimal(11,2)) / 100 * b.actionvalue ' +@newline
						   + ' or not exists (select 1 from fintrans ff WITH(NoLock) where ff.acctno =' + @StrategyVariabletable + '.acctno and  ' +@newline 
						   + '  ff.datetrans> b.dateadded and ff.transtypecode in (''PAY'',''DDN'',''COR'',''REF'',''RET'' ) ))' 
						   --' group by acctno having (isnull(sum(-transvalue),0) < ' + @Operator1+ '/100 * b.actionvalue )) ' --IP - 20/08/09 - UAT(799) added isnull check
					end

					if @condition ='RETCHQ'--Customer has X returned cheques in the last month 
					begin
						-- set @statement = @statement + ' acct a,custacct ca where a.acctno = CMStrategyVariables.acctno and ' +
						--' CMStrategyVariables.strategy ='             + '''' + @strategy + '''' +  @newline +
						set @statement = @statement + ' acct a WITH(NoLock) ,custacct ca WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno and ' +
						' ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' +  @newline +
						' and ca.hldorjnt = ''H'' and ca.acctno = a.acctno and exists (select cu.custid from fintrans f WITH(NoLock), custacct cu WITH(NoLock) '+ @newline  +
						' where cu.custid = ca.custid and cu.hldorjnt =''H'' and f.acctno = cu.acctno and f.transtypecode =''RET'' '+ @newline  +
						--' and f.datetrans > dateadd(month,-' + @Operator2 + ' ,getdate()) ' +
						' and f.datetrans > dateadd(month,-' + @Operator2 + ' , '+ '''' + convert(varchar, @rundate) + '''' + ' ) ' +		--IP - 14/02/12 - use @rundate
						' group by cu.custid having count(f.transvalue) >= ' + @Operator1 + ')'
					end
 

			   --if @condition ='SCORE'--Score is less than or greater than X 
				--  begin
				--      -- set @statement = @statement + ' proposal p where p.acctno = CMStrategyVariables.acctno and ' +
				--      --'  CMStrategyVariables.strategy ='  + '''' + @strategy + '''' +  @newline +
		   --      set @statement = @statement + ' proposal p where p.acctno = ' + @StrategyVariabletable + '.acctno and ' 
		   --      + @StrategyVariabletable + '.strategy ='  + '''' + @strategy + '''' +  @newline +
		   --      ' and p.points >0 and p.points ' +  @operand +  @Operator1 
		   --  end
   
					if @condition ='SCORE'--Score is less than or greater than X 
					begin
		
						 set @statement=REPLACE(@statement,'exists','(exists')			-- #9519 open bracket required because of OR condition
						 set @statement = @statement + ' proposal p WITH(NoLock) inner join acct a WITH(NoLock) on p.acctno = a.acctno where p.acctno = ' + @StrategyVariabletable + '.acctno and ' 
						 + @StrategyVariabletable + '.strategy ='  + '''' + @strategy + '''' +  @newline +
						 ' and p.points >0 and p.points ' +  @operand +  @Operator1 +
						 ' and a.accttype != ''T'' )' + @newline +
						 ' or exists ( select 1 from acct a WITH(NoLock) inner join custacct ca WITH(NoLock) on a.acctno = ca.acctno and ca.hldorjnt = ''H'' ' +		--IP - 13/03/12 - #9793 - IP/JC - Cater for checking points for Store Card
						 ' inner join proposal p1 WITH(NoLock) on ca.custid = p1.custid ' +
						 ' where a.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy ='  + '''' + @strategy + '''' +  @newline +
						 ' and p1.dateprop = (select max(p2.dateprop) from proposal p2 WITH(NoLock) where p2.custid = p1.custid and p2.points <> 0 )' +
						 ' and p1.points ' +  @operand +  @Operator1 +
						 ' and a.accttype = ''T'' ) ' 
         
					end

				

					 --IP - 04/09/09 - UAT(809) - Changed the logic to set the SPALESS condition to use the 
					 --SPASummary table which is populated when Credit Collection EOD is run.
					if @condition ='SPALESS'--Customer paid less than X of Arrangement amount 
						 -- to do this get dateadded and then check total value of fintrans payments since spa added....
						 -- and multiply this by the number of months due.... 
					begin
						  ----set @statement = @statement + ' spa s, fintrans f where s.acctno = CMStrategyVariables.acctno and isnull(s.dateexpiry,''1-jan-2022'') >getdate() ' + 
						  --set @statement = @statement + ' spa s, fintrans f where s.acctno = ' + @StrategyVariabletable + '.acctno and isnull(s.dateexpiry,''1-jan-2022'') >getdate() ' + 
						  -- @newline  + ' and exists (select sum(f.transvalue) from fintrans f where f.acctno= s.acctno and f.datetrans > s.dateadded ' +
						  -- @newline +' and f.transtypecode in (''PAY'',''DDN'',''COR'',''REF'',''RET''  ) group by f.acctno having sum(-transvalue) ' +  @Operand +
						  --' (datediff(month,dateadded,getdate()) +1)* ' + @Operator1  + '/100*s.spainstal )'
          
							set @statement = @statement + ' spasummary s ' + @newline
							+ ' where s.acctno = ' + @StrategyVariabletable + '.acctno '+ @newline 
							+ ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline
							+ ' and exists(select 1 from CMStrategyAcct a  WITH(NoLock) where a.acctno = s.acctno) ' + @newline
							+ ' group by s.Acctno, s.Dateadded, s.NextDateExpiryDue, s.ArrangementDateExpiry, s.TotalSPAInstalDue ' + @newline
							+ ' having (select isnull(sum(-f.transvalue),0) from fintrans f WITH(NoLock) ' + @newline
							+ ' where f.acctno = s.acctno ' + @newline
							+ ' and f.datetrans between s.dateadded and s.NextDateExpiryDue ' + @newline
							+ ' and f.transtypecode = ''PAY'') ' + @Operand + ' cast( ' + @Operator1 + ' as decimal(11,2))/100 * s.TotalSPAInstalDue'
									  							
					end
     
					--IP - 07/09/09 - UAT(809) - Condition to check if the SPA Arrangment on an account has expired.
					if @Condition = 'SPAARREXP'
					begin
     
						set @statement = @statement + ' spasummary s ' + @newline
						+ ' where s.acctno = ' + @StrategyVariabletable + '.acctno '+ @newline 
						+ ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline
						+ ' and exists(select 1 from CMStrategyAcct a  WITH(NoLock) where a.acctno = s.acctno) ' + @newline
						--+ ' and s.ArrangementDateExpiry < getdate() '
						+ ' and s.ArrangementDateExpiry < ' + '''' + convert(varchar, @rundate) + '''' + ' '			--IP - 14/02/12 - use @rundate
										
					end

					if @condition ='SPANODAYS'--Arrangement not received after X days --spa Dateadded will be used to store date of first instalment due....
					begin
						--set @statement = @statement + ' spa s where s.acctno = CMStrategyVariables.acctno and isnull(s.dateexpiry,''1-jan-2022'') >getdate() ' +  
						--set @statement = @statement + ' spa s where s.acctno = ' + @StrategyVariabletable + '.acctno and isnull(s.dateexpiry,''1-jan-2022'') >getdate() ' +  
						set @statement = @statement + ' spa s where s.acctno = ' + @StrategyVariabletable + '.acctno and isnull(s.dateexpiry,''1-jan-2022'') '
						+' > '+ '''' + convert(varchar, @rundate) + '''' + ' ' +
						 @newline + ' and not exists (select 1 from fintrans f  WITH(NoLock) where f.acctno = s.acctno and f.transtypecode in (''PAY'',''DDN'',''COR'',''REF'',''RET''  ) and datetrans >s.dateadded)'
						--+ ' and getdate() > dateadd(day,' + @Operator1 + ',s.dateadded) ' 
						+ ' and ' + '''' + convert(varchar, @rundate) + '''' + ' > dateadd(day,' + @Operator1 + ',s.dateadded) '					--IP - 14/02/12 - use @rundate
					end












					if @condition ='PartPay'--Customer Makes payment resulting in arrears greater than X% but less than Y% so account goes into different Strategy
					begin
						--set @statement = @statement + ' acct a,instalplan i where a.acctno = CMStrategyVariables.acctno and i.acctno = a.acctno ' +
						set @statement = @statement + ' acct a WITH(NoLock) ,instalplan i WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno and i.acctno = a.acctno ' +
						@newline + ' and a.outstbal >=a.arrears and i.agrmtno = 1 and (a.arrears/i.instalamount) *100 between '+  @operator1 + ' and ' + @operator2  + 
						@newline + ' and exists (select 1 from fintrans f  WITH(NoLock) where f.acctno = a.acctno and f.transtypecode in (''PAY'',''DDN'') AND f.datetrans > ' 
						+ @newline  + '''' +  convert(varchar,@lastrundate) + '''' + ' ) and i.instalamount>0 '
					end     

     
					if @condition = 'PAYLESSINST' -- re-instated  UAT1003 jec 27/04/10
					begin
						set @statement = @statement + ' acct a  WITH(NoLock),instalplan i  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno and i.acctno = a.acctno ' +
						' and a.outstbal >=a.arrears and i.agrmtno = 1 ' + 
						' and exists (select acctno from fintrans f  WITH(NoLock) where f.acctno = a.acctno and f.transtypecode in (''PAY'',''DDN'',''COR'',''REF'',''RET'') ' +
						' group by acctno having sum(-transvalue) ' + @Operand + ' cast( ' + @Operator1 + ' as decimal(11,2))/100 * i.instalamount) '
          
					 end 


					if @condition ='RECARRS'--Recurring Arrears X times in the last 6 months 
					begin
          				--set @statement=REPLACE(@statement,'exists','(exists')			-- #9520 open bracket required because of OR condition
						set @statement = @statement + ' acct a  WITH(NoLock),custacct ca  WITH(NoLock),customer c  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno and ca.acctno = a.acctno ' +
						--' and a.AcctType!=''T'' ' +
						' and ca.hldorjnt =''H'' and c.custid = ca.custid and c.recurringarrears >= ' + @Operator1  + @newline 
						--')' +
						---- #9520 Storecard recurring arrears							--JC/IP - 14/03/12 - #9794 - Removed the below as should look at Customer.recurringarrears for Store Card
						--' or exists(select COUNT(*) from #missedpay m  ' + @newline +          
						--' where m.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
						--' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						--' and stmtminpayment+payment > 0' + @newline +
						----' and datepaymentdue > DATEADD(m,-cast(' + @Operator2 + ' as INT),Getdate()) ' +
						-- ' and datepaymentdue > DATEADD(m,-cast(' + @Operator2 + ' as INT),' + '''' + convert(varchar, @rundate) + '''' + ' ) ' +		--IP - 14/02/12 - use @rundate
						--' having COUNT(*)>= cast(' + @Operator1 + ' as INT) ' + @newline +
						--')'
					end
 
					if @condition ='SCMTHS'--Account has been in status code X in the last Y months.
					begin
						--set @statement = @statement + ' acct a,status s,custacct c where a.acctno= CMStrategyVariables.acctno ' +
						set @statement = @statement + ' acct a  WITH(NoLock) ,status s  WITH(NoLock),custacct c  WITH(NoLock) where a.acctno= ' + @StrategyVariabletable + '.acctno ' +
						' and s.acctno = c.acctno and c.hldorjnt =''H'' ' + 
						' and c.acctno = a.acctno '
						--+ @newline + ' and a.currstatus =s.statuscode and a.currstatus=' + '''' + @Operator1 + ''''  + ' and dateadd(month,' + @Operator2 + ',s.datestatchge ) > getdate() '
						+ @newline + ' and a.currstatus =s.statuscode and a.currstatus=' + '''' + @Operator1 + ''''  + ' and dateadd(month,' + @Operator2 + ',s.datestatchge ) >'		--IP - 14/02/12 - use @rundate
						+ '''' + convert(varchar,@rundate) + '''' + ' '
         
					end

 

				/*    if @condition = 'XINSMS' -- Customer missed X instalments after account being setup for X months  TODO when understand what this requirement is
					begin
					end
				*/

					 if @condition ='BALEXCHARGES'-- as as400bal now stores balance ex charges         
					 begin  
					   --set @statement = @statement +  ' acct a where a.acctno = CMStrategyVariables.acctno ' +
					   set @statement = @statement +  ' acct a WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' +
						  ' and a.as400bal ' + @operand +  @Operator1
					 end

					-- #9517 Outstanding Balance is less than or greater than X% of store card limit
					if @condition ='BalSCard'          
					 begin        

					   set @statement = @statement +' acct a  WITH(NoLock), custacct ca  WITH(NoLock) , customer c  WITH(NoLock)  ' + @newline + 
						  ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
						  ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						  ' and a.acctno  = ca.acctno and ca.hldorjnt=''H'' and ca.custid=c.custid  ' +
						  ' and a.Accttype=''T'' '  +		-- storecard account
						  ' and ISNULL(c.StoreCardLimit,0)>0 ' + @newline +
						  ' and (c.StoreCardLimit-ISNULL(c.StoreCardAvailable,0)) / c.StoreCardLimit ' + @Operand + ' ' + 'cast(' + @Operator1+ ' as decimal(11,2))/100 '       
          
					 end


					if @condition ='BALANC'          
					 begin  
					   --set @statement = @statement +  ' acct  a where a.acctno = CMStrategyVariables.acctno ' +
					   set @statement = @statement +  ' acct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' +
						  ' and a.outstbal ' + @operand +  @Operator1
					 end


					 if @condition ='ArrearsPC' or @condition = 'ARRSPC'
					 begin
						-- account may not exist on cmstrategyacct UAT764
						  set @statement = @statement + ' acct a  WITH(NoLock), instalplan i  WITH(NoLock) left outer join cmstrategyacct ca WITH(NoLock) on ca.acctno=i.acctno ' +	--UAT764 jec
						  ' where  a.acctno = ' + @StrategyVariabletable + '.acctno and i.acctno = a.acctno and ' +
						  --'a.arrears/i.instalamount ' + @operand + 'cast(' +  @Operator1 + 'as decimal(11,2))/100 and i.instalamount >0 ' + --IP/JC - 16/07/09 - Cast to decimal
						   '(i.instalamount >0 and a.arrears/i.instalamount ' + @operand + 'cast(' +  @Operator1 + ' as decimal(11,2))/100 or (i.instalamount = 0))' +  --IP - 07/03/12 - #9735 - LW74766 - check for instalamount = 0--IP/JC - 16/07/09 - Cast to decimal
						  --' and a.acctno = ca.acctno and ca.strategy != ''NON''' +
						  --'and isnull(ca.strategy,'''') != ''NON'' ' +		--UAT855 jec 11/09/09 removed  UAT764 jec 
						  ' and ca.dateto is null'
					 end

  
     
					 -- #9513 Arrears are less than or greater than X% of current minimum payment
					 if @condition ='ArrearsPCSC' 
					 begin
						set @statement = @statement +' acct a  WITH(NoLock) , StoreCardPaymentDetails p  WITH(NoLock) ' + @newline +          
						  ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
						  ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
						  ' and a.acctno  = p.acctno ' +
						  ' and a.Accttype=''T'' '  +		-- storecard account
						  ' and (a.arrears >0 ' +
						  ' and a.arrears / case when p.MinimumPayment = 0 then a.arrears else p.MinimumPayment end ' + @Operand + ' ' + 'cast(' + @Operator1 + ' as decimal(11,2))/100 ' +
						  ' or (a.arrears = 0)) '						--IP - 17/04/12 - #9884
						  --' and a.arrears >0 ' +
						  --' and a.arrears / case when p.MinimumPayment = 0 then a.arrears else p.MinimumPayment end ' + @Operand + ' ' + 'cast(' + @Operator1 + ' as decimal(11,2))/100 '   
      				 end



     

					 if @condition ='ArrsExChgePC'
					 begin
						  --set @statement = @statement + ' acct a, instalplan i where a.acctno = CMStrategyVariables.acctno ' +
						  set @statement = @statement + ' acct a  WITH(NoLock), instalplan i  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' +
						  ' and i.acctno = a.acctno and i.instalamount >0 and (a.arrears-(a.outstbal-a.as400bal))/i.instalamount ' + @operand + 'cast(' + @Operator1+ 'as decimal(11,2))/100 ' --IP/JC - 16/07/09 - Cast to decimal
					 end


					-- new exit condition added for service requests
				   --IP - 15/12/09 - UAT(937) - only accounts that have closed Service requests where one was closed more than 30 days ago will exit to NON.
					--if @condition = 'SRC'
				 --    begin 
					--	--SET @statement = ' update CMStrategyVariables set OpenSR = ''Y'' where exists' + @newline + ' ( select * from '
				 --       set @statement = @statement + ' SR_ServiceRequest sr join SR_Resolution srr on sr.ServiceRequestNo = srr.ServiceRequestNo ' + @newline + 
					--		--' where sr.acctno = ' + @StrategyVariabletable + '.acctno and sr.Status = ''C'' and DATEDIFF(DAY,srr.DateClosed,GETDATE()) > 30 ' + @newline + --IP - 15/12/09 - UAT(937) Changed operator to >
					--	    ' where sr.acctno = ' + @StrategyVariabletable + '.acctno and sr.Status = ''C'' '+
					--	    ' and DATEDIFF(DAY,srr.DateClosed,' + '''' + convert(varchar, @rundate) + '''' + ' ) > 30 ' + @newline +	--IP - 14/02/12 - use @rundate
					--		' and srr.DateClosed =(select max(srr1.DateClosed) from SR_Resolution srr1 inner join SR_ServiceRequest sr1 on sr1.ServiceRequestNo = srr1.ServiceRequestNo ' + @newline + 
					--		' where sr1.acctno = sr.acctno) ' + @newline +
					--		' and not exists(select * from SR_ServiceRequest sr2 where sr2.AcctNo = sr.AcctNo AND sr2.Status!=''C'')'
					--		--' where sr.acctno = CMStrategyVariables.acctno and sr.Status = ''C'' and DATEDIFF(DAY,srr.DateClosed,GETDATE()) < 30 '     
				 --    end  

 
 					if @condition = 'SRC'	--#10912		
					 begin 
						set @statement = @statement + ' SR_Summary sr  WITH(NoLock) ' + @newline +
							' where sr.acctno = ' + @StrategyVariabletable + '.acctno and sr.DateClosed != ''01-01-1900'' ' + 
							' and DATEDIFF(DAY,sr.DateClosed,' + '''' + convert(varchar, @rundate) + '''' + ' ) > 30 ' + @newline +	
							' and not exists(select 1 from SR_Summary sr2  WITH(NoLock) where sr2.AcctNo = sr.AcctNo AND sr2.DateClosed =''01-01-1900'')'
					 end  

      
				-- 2 new conditions for allocating bailiff/super-bailiff
      
					 if @condition = 'AllocXtimes' --Account allocated to bailiff 5 times
					 begin
						 set @statement = @statement + '  follupalloc f  WITH(NoLock) , acct a  WITH(NoLock) ,cmstrategyacct s  WITH(NoLock) ' +	--UAT967 jec 13/01/10 removed courtsperson c,
						 ' where a.acctno = ' + @StrategyVariabletable + '.acctno and s.acctno= a.acctno and ' 
						 + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline +
						 'and s.strategy=' + @StrategyVariabletable + '.strategy ' +
						 --' and c.empeeno =  f.empeeno ' +	-- and c.empeetype = ''B''' +		-- jec 25/06/09  --UAT967 jec 13/01/10
						 ' and f.acctno=a.acctno' +
						 ' and f.datealloc> s.datefrom and s.dateto is null ' +
						 -- so if X instalments in arrears then  effectively arrears /X > instalment
						 --' group by a.acctno having count(*) >'+ @Operator1 
						 ' group by a.acctno having count(*) ' + convert(varchar,@Operand) + ' ' + @Operator1	--UAT967 jec 13/01/10
					 end



					 if @condition = 'AllocXtimeSB' --Account allocated to bailiff 6 times
					 begin
						 set @statement = @statement + '  follupalloc f  WITH(NoLock) , acct a  WITH(NoLock) ,courtsperson c  WITH(NoLock) ,cmstrategyacct s  WITH(NoLock)  ' +	--UAT967 jec 13/01/10
						 ' where a.acctno = ' + @StrategyVariabletable + '.acctno and ' 
						 + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline +
						 ' and s.strategy= '+ @StrategyVariabletable + '.strategy ' +		--UAT967 jec 13/01/10
						 ' and f.acctno=a.acctno and s.acctno= a.acctno' +		--UAT967 jec 13/01/10
						 ' and c.empeeno =  f.empeeno and c.empeetype = ''SB'' ' + 
						 ' and f.datealloc> s.datefrom and s.dateto is null' +		--UAT967 jec 13/01/10
						 -- so if X instalments in arrears then  effectively arrears /X > instalment
						 ' group by a.acctno having count(*) ' + convert(varchar,@Operand) + ' ' + @Operator1	--UAT967 jec 13/01/10
					 end

     
					 -- Grace Period	-- 25/06/09
					if @condition = 'Grace'
					 begin
						declare @mthsreposs int
						select @mthsreposs=cast(value as int) from CountryMaintenance where CodeName='mthsreposs'
						select @mthsreposs
						--set @statement = @statement + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno and CMStrategyVariables.strategy =' + @newline 
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno and ' + @StrategyVariabletable + '.strategy =' + @newline
								--+ ' and dateadd(day,' + @mthsreposs + ',a.dateincurrentstep) < getdate() ' + @newline    
								  + ' and dateadd(day,' + @mthsreposs + ',a.dateincurrentstep) < ' + '''' + convert(varchar, @rundate) + '''' + ' ' +  @newline	--IP - 14/02/12 - use @rundate
		  
					 end    

     
					-- In strategy for x days
					if @condition = 'InStrat'
					 begin 
						--set @statement = @statement + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno ' + @newline
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and a.dateto is null'				
						--+ ' and dateadd(day,' + @Operator1  + ',a.datefrom) < getdate() ' + @newline    
						+ ' and dateadd(day,' + @Operator1  + ',a.datefrom) < ' + '''' + convert(varchar, @rundate) + '''' + ' ' + @newline		--IP - 14/02/12 - use @rundate   
					 end

     
					-- Previous Strategy   jec 10/07/09    
					if @condition = 'PrevStrat'
					 begin 
						--set @statement = @statement + ' CMStrategyAcct a where a.acctno = CMStrategyVariables.acctno ' + @newline
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and a.strategy='  + '''' + @strategy + '''' +  @newline
						--+ ' and a.currentstep='  + '''' + convert(varchar,@Step) + '''' 	+  @newline	 --IP - 16/09/09 - UAT(868)	- Commented this line out.
                     
					 end    


    
					--IP - 14/07/09 - Max Item Value 
					if @condition = 'MaxItemVal'
					begin
						--set @statement = @statement + ' acct a, Lineitem l where a.acctno = CMStrategyVariables.acctno ' + @newline
						set @statement = @statement + ' acct a  WITH(NoLock), Lineitem l  WITH(NoLock) where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and a.acctno = l.acctno ' + @newline 
						+ ' and l.itemtype = ''S'' ' + @newline 
						+ ' having max(l.ordval) >' + @Operator1 + @newline
		
					end
	
					--IP - 14/07/09 - Account atleast X months instalments in arrears in the past Y months.
					if @condition = 'MthArrMths'
					begin
						set @statement = @statement + ' acct a  WITH(NoLock) inner join instalplan i  WITH(NoLock) on a.acctno = i.acctno ' + @newline
						+ ' inner join arrearsdaily ad  WITH(NoLock) on a.acctno = ad.acctno ' + @newline
						--+ ' where a.acctno = CMStrategyVariables.acctno ' + @newline
						+ ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline 
						+ ' and ad.arrears > 0 ' + @newline
						+ ' and i.instalamount > 0 ' + @newline
						+ ' and ad.arrears / i.instalamount >=' + @Operator1 + @newline
						--+ ' and ad.datefrom > dateadd(m,-' + @Operator2 + ',getdate())' + @newline 
						+ ' and ad.datefrom > dateadd(m,-' + @Operator2 + ', ' 
						+ '''' + convert(varchar, @rundate) + '''' + ' ) ' + @newline					--IP - 14/02/12 - use @rundate
					end
	
					
					if exists(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')  ------if condition added by Zensar on 30/08/2019----
						begin
							--Jec - 08/03/12 - Account has been at least X months payments in arrears in last Y months of StoreCard payments.
							if @condition = 'MthArrMthsSC'
							begin
								set @statement=REPLACE(@statement,'*','Max(MonthsArrs)')				-- #9846 need max months
								  set @statement = @statement +' #arrearsmths m  ' + @newline +          
								  ' where m.acctno = ' + @StrategyVariabletable + '.acctno ' +@newline + 
								  ' and ' + @StrategyVariabletable + '.strategy =' + '''' + @strategy + '''' + @newline + 
								  ' and m.datefrom between DATEADD(m,-cast(' + @Operator2 + ' as INT), ' + '''' + convert(varchar,@rundate) + ''''  + ' ) and ' + '''' + convert(varchar,@rundate) + '''' + 
								  ' group by m.acctno having Max(MonthsArrs) ' + @operand + ' cast(' + @Operator1 + ' as INT) '
							end			
						end
						

	

					-- No Movement   jec 14/07/09    
					if @condition = 'NoMove'
					begin
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) INNER JOIN fintrans f  WITH(NoLock) on f.acctno = a.acctno ' + @newline
						--+ ' where a.acctno = CMStrategyVariables.acctno ' + @newline
						+ ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and f.transtypecode not in (''INT'',''ADM'') ' + @newline 
						--+ ' having dateadd(m,' + convert(varchar,@mthsNoMove) +  ',max(f.datetrans)) < getdate() '+ @newline 	
						+ ' having dateadd(m,' + convert(varchar,@mthsNoMove) 
						+  ',max(f.datetrans)) < ' + '''' + convert(varchar, @rundate) + '''' + ' ' + @newline 		--IP - 14/02/12 - use @rundate					
    
					End
    
					-- Payment Reveived   jec 15/07/09    
					if @condition = 'PayRecv'
					begin
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) INNER JOIN fintrans f  WITH(NoLock) on f.acctno = a.acctno ' + @newline
						--+ ' where a.acctno = CMStrategyVariables.acctno ' + @newline
						+ ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and f.transtypecode = ''PAY'' ' + @newline 
						+ ' and f.datetrans > a.datefrom and a.dateto is null '+ @newline 								
    
					End
    
    
					--	 --IP - 15/07/09 - Account written off after repossession
					if @condition = 'WoffRepo'
					begin
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) inner join fintrans f1  WITH(NoLock) on a.acctno = f1.acctno ' + @newline
						--+ ' where a.acctno = CMStrategyVariables.acctno ' + @newline
						+ ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and f1.transtypecode = ''BDW'' and f1.datetrans > ' + @newline
						+ ' (select max(f2.datetrans) from fintrans f2 WITH(NoLock)' + @newline
						+ '		where f2.acctno = f1.acctno ' + @newline
						+ '		and f2.transtypecode = ''REP'' )'

					end

					--IP - 16/07/09 - No Further Follow up
					if @condition = 'NoFollup'
					begin
						set @statement = @statement + ' CMStrategyAcct a  WITH(NoLock) inner join bailaction b  WITH(NoLock) on a.acctno = b.acctno ' + @newline
						--+ ' where a.acctno = CMStrategyVariables.acctno ' + @newline
						+ ' where a.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and b.code = ''NFA''' + @newline
						+ ' and b.dateadded > a.datefrom ' + @newline
						+ ' and a.dateto is null '
					end
	
					--UAT733 jec - Service Request
					if @condition = 'SRA'
					begin
						set @statement = @statement + ' SR_ServiceRequest sr  WITH(NoLock) INNER JOIN SR_Resolution r  WITH(NoLock) on sr.serviceRequestNo=r.serviceRequestNo ' + @newline
						--+ ' where a.acctno = CMStrategyVariables.acctno ' + @newline
						+ ' where sr.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' and ServiceType = ''C'' AND Status = ''C'' ' + @newline
						--+ ' and r.DateClosed ' + @operand + ' dateadd(d,-' + @Operator1 + ',getdate()) '	-- UAT835 jec add minus sign
						+ ' and r.DateClosed ' + @operand + ' dateadd(d,-' + @Operator1 
						+ ', ' + '''' + convert(varchar, @rundate) + '''' + ' )'					--IP - 14/02/12 - use @rundate
					--PRINT @statement
					end		


					if @condition = 'ProvisionAmt'
					begin
						set @statement = @statement + ' View_Provision p' + @newline
						+ ' WHERE p.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' AND p.provision BETWEEN ' + @operator1 + ' AND ' + @operator2	
					--PRINT @statement
					end		

			
					if @condition = 'ProvisionPct'
					begin
						set @statement = @statement + ' View_ProvisionPercent p' + @newline
						+ ' WHERE p.acctno = ' + @StrategyVariabletable + '.acctno ' + @newline
						+ ' AND p.provision BETWEEN ' + @operator1 + ' AND ' + @operator2	
					--PRINT @statement
					end		

    
					SET @statement= @statement +  ') AND ISNULL(' + @condition + ','''')  != ''Y'' ' 

					-- Add general Strategy check to @statement
					--set @statement = @statement + ') and CMStrategyVariables.strategy = ' + '''' + ltrim(@strategy) + '''' 
					set @statement = @statement + ' and ' + @StrategyVariabletable + '.strategy = ' + '''' + ltrim(@strategy) + '''' 
					execute sp_executesql @statement
     
					select @return = @@error,@rowcount = @@rowcount
    
					if @return !=0
						begin
							print @statement     
							return
					end
					
					-----Commented by Zensar on 30/08/2019----start
					--If @rowcount >1000000
					--begin
					--	print @statement
					--end
					-----Commented by Zensar on 30/08/2019----end


					if @statement2 !=''
					begin
						--print @statement2    
						execute sp_executesql @statement2   
						set @return= @@error
						if @return !=0
						begin
							print @statement2     
							return
						end

					end
					set @statement2 =''
   					--then we are going to loop through each strategy and do the entry conditions first, then do the steps and then do the exit conditions
					-- although the exit conditions are also going to be done through a trigger.

				END
			FETCH NEXT FROM StrategyCondition_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
           @OrClause,@step,@ActionCode,@StepActiontype,@ConditionType 

		END
	CLOSE StrategyCondition_cursor
	DEALLOCATE StrategyCondition_cursor

	alter table #prevArrears drop constraint pk_prevArrears

	IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')
	BEGIN
			alter table #missedpay drop constraint pk_missedpay
			
			alter table #arrearsmths drop constraint pk_ArrsMths
    END

		
	if OBJECT_ID('#prevArrears','U') is not null 
	drop table #prevArrears
				
	if OBJECT_ID('#missedpay','U') is not null 
	drop table #missedpay
		
	if OBJECT_ID('#arrearsmths','U') is not null 
	drop table #arrearsmths

go