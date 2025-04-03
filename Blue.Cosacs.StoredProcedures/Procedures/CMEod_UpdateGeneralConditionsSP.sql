
GO
if exists (select * from sysobjects where name ='CMEod_UpdateGeneralConditionsSP')
drop procedure CMEod_UpdateGeneralConditionsSP
go

CREATE procedure [dbo].[CMEod_UpdateGeneralConditionsSP] @lastrundate datetime , @rundate datetime, @return int out
-- **********************************************************************
-- Version: 002
-- Title:
-- Developer: Alex Ayscough
-- Date: 2007
-- Purpose: This updates general conditions of accounts for insert into or update of strategies

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 26/06/09  jec Credit Collection Process steps changes
-- 28/07/09  jec UAT fixes
-- 30/07/09  jec UAT767 New Strategy Variable tables
-- 31/07/09  jec UAT733	allow for variable in SRA condition  
-- 26/08/09  ip  UAT816 accounts sent to a strategy manually through Telephone Action Screen using 'STS' code
-- were inserted with a currentstep = 0. In this instance the procedure CMEOD_UpdateStrategyandStepConditionsSP
-- called after this procedure would not update conditions such as DayX correctly 
-- as when joining onto CMStrategyCondition it expects the currentstep to equal 1. Update statement to update currentstep to 1 moved
-- from  CNEod_UpdateandDoStepsInStrategiesSP so that this condition is correctly set.
-- 27/08/09  ip UAT822 when checking if an account is up to date for condition AOK, need to ensure that the account has not been written off.
-- 02/09/09  ip UAT828 PTPComp condition was incorrectly being set as completed.
-- 07/09/09  jec UAT827 - Add 16 hours to repo transaction (not time stamp) so transdate will be > last run date.
--						- Only set AOK if status not ='6'
-- 09/9/09  jecc UAT835 Include accounts with service request in variables tables
-- 15/09/09  ip UAT862 - Updated condition 'LOAN' as previously this was not being updated, preventing accounts from entering the 
--						 'NSL (Non Starter Cash Loan)' strategy.'
-- 15/09/09  ip UAT857 - Include accounts written off within the last 3 months.	
-- 01/10/09  ip UAT855 - do not set the PTPEnt condition if the sum of payments made after a PTP has been entered >= the PTP amount.	
-- 02/10/09  ip UAT857 - Check if bdwbalance > 0 or bdwcharges as the bdwbalance may be 0.	
-- 05/10/09  ip UAT902 - When updating the WOFF condition (which is updated if the account has been written off) need to check that subsequently there
--						 has not been a BDW Reversal on the account.	
-- 06/10/09  ip UAT906 - Changed the PTPPEND condition to check for any payments made since PTP entered.
-- 10/12/09 jec - correct missing join.
-- 15/12/09  ip UAT937 - Service Accounts must first be in arrears in order to enter the Service Strategy.
-- 17/12/09  ip UAT929 - DFA Strategy accounts were not entering the strategy and when exiting were re-entering DFA incorrectly, therefore added check on agreement.datenextdue
-- 07/02/12 jec #9521 CR9417 - duplication of existing strategies
-- 08/02/12 jec #9514 CR9417 - new SC missed payment date
-- 13/02/12 jec #9518 CR9417 - SC existing account is up to date
-- 13/02/12 ip  Replace GETDATE() with @rundate
-- 12/03/12 jec #9778 Account is in wrong strategy instead of SC Low Risk
-- 26/11/12 ip  #10912 - Integration - Push to CoSACS on Logging - Updating condition OpenSR to look at new table SR_Summary (Service now moved to web)
-- 04/12/12 jec #11761 - Singer Migration - New Credit Collections entry conditiion
-- 16/01/13 ip  CR 11488 - #11761 - Singer Migration - New Credit Collections entry conditiion - Merged from CoSACS 6.5
-- 10/07/13 ip  #13720 - CR12949 - Capture Ready Assist accounts to enter the Ready Assist strategy.
-- 09/08/19     Applied country maintenece check on store card releated records
-- 09/08/19  Optimized sql statement

-- **********************************************************************
	-- Add the parameters for the stored procedure here
			--IP - 13/02/12
as

    declare 
		@statement sqltext
		--truncate table CMStrategyVariablesEntry
		truncate table CMStrategyVariablesEntry
		truncate table CMStrategyVariablesSteps
		truncate table CMStrategyVariablesExit

		Declare @IntFreeDays INT		--#9521
		select @IntFreeDays= ISNULL((select value from CountryMaintenance WITH(NoLock) where codename='SCardInterestFreeDays'),0)
				
		INSERT INTO bailactionPTP

		(acctno , allocno, empeeno,	dateadded, actionvalue ,datedue)
	
		SELECT acctno , allocno, empeeno,dateadded, actionvalue ,datedue 

		FROM bailaction b WITH(NoLock) ----INNER JOIN Acct a on b.acctno = a.acctno and a.accttype!='T'		-- #9521 not StoreCard
		WHERE  b.code = 'PTP'
		--AND b.dateadded > DATEADD(YEAR,-1,GETDATE())
		AND b.dateadded > DATEADD(YEAR,-1,@rundate)							--IP - 13/02/12 - use @rundate
		AND NOT EXISTS (SELECT 1 FROM bailactionPTP P WITH(NoLock)			---Zensar   - 07/08/19  --Modified the script replace * with 1

		WHERE p.acctno= b.acctno AND p.empeeno= b.empeeno
		AND b.dateadded = p.dateadded AND b.allocno = p.allocno)
		
		
	
			DECLARE @lastPostWriteOffStep INT, 
			@lastPostWriteOffStepSC INT		-- #9521
		SELECT @lastPostWriteOffStep = MAX(Step)
		FROM CMStrategyCondition WHERE Strategy = 'PWO'
		SELECT @lastPostWriteOffStepSC = MAX(Step) FROM CMStrategyCondition WHERE Strategy = 'SCPWO'			-- #9521 StoreCard
	
		
		-------------- Modified By Zensar on 08/07/2019 ------- Start
		IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')
			BEGIN

				  -- #9518 Storecard account upto date - need to get payment dates and due amounts 
				select acctno,MAX(Dateto) as DateTo,MAX(datepaymentdue) as datepaymentdue,SUM(stmtminpayment) as totminpayment,CAST(0 as MONEY) as stmtminpayment,CAST(0 as MONEY) as payment
				into #uptodate
				from dbo.StoreCardStatement s WITH(NoLock)
				group by acctno
	
				alter TABLE #uptodate ADD  CONSTRAINT [pk_uptodate] PRIMARY KEY CLUSTERED ([acctno] ASC)
				-- update with payments received 
				UPDATE #uptodate 
					set payment=(select ISNULL(SUM(transvalue),0) from fintrans f WITH(NoLock) where f.acctno=m.acctno and transtypecode='PAY'),
						stmtminpayment=(select SUM(stmtminpayment) from StoreCardStatement s WITH(NoLock) where s.acctno=m.acctno and m.datepaymentdue=s.datepaymentdue) -- last min payment


				from #uptodate m
			END


	
		if OBJECT_ID('tempdb..#AcctStrategy','U') is not null 
		drop table #AcctStrategy

		create table #AcctStrategy
		(Acctno char(12))
		
		INSERT INTO #AcctStrategy	  
		select distinct acctno 
		from CMStrategyAcct C WITH (NOLOCK) where c.dateto is NULL AND c.strategy !='NON' AND c.strategy !='SCNON'    -- #9521 StoreCard
        AND NOT (C.strategy = 'PWO' AND c.currentstep = @lastPostWriteOffStep)
        AND NOT (C.strategy = 'SCPWO' AND c.currentstep = @lastPostWriteOffStepSC)    


		INSERT INTO #AcctStrategy
		select a.acctno
		from  acct a WITH (NOLOCK)
		where (a.arrears >0 and a.outstbal>0 and a.accttype not in ('C','S')) and
		a.acctno not in (select acctno from #AcctStrategy)

			-- Third comparision set in temp table where records are not present in the first and second result set
		INSERT INTO #AcctStrategy
		SELECT distinct a.acctno FROM fintrans f WITH (NOLOCK) 
		inner join acct a WITH (NOLOCK) on a.acctno = f.acctno
		WHERE f.transtypecode = 'BDW'
        AND f.datetrans > DATEADD(m, -3, @rundate)
        AND (a.bdwbalance > 0 or a.bdwcharges >0) and a.accttype not in ('C','S')  
		and a.acctno not in (select acctno from #AcctStrategy)



		INSERT INTO #AcctStrategy




		select acctno
		from acct a WITH (NOLOCK) -- ,cmstrategy  b WITH (NOLOCK)
		where 
		EXISTS (SELECT 1 FROM TermsTypeTable t WITH (NOLOCK) WHERE t.termstype=a.termstype AND t.DeferredMonths!=0
		AND EXISTS(SELECT ag.acctno FROM agreement ag WHERE ag.acctno = a.acctno AND ag.datenextdue > @rundate))
		AND a.acctno not in (select acctno from #AcctStrategy)

		insert into CMStrategyVariablesEntry (acctno,strategy)
		select acctno ,strategy
		from #AcctStrategy a, cmstrategy  b WITH (NOLOCK)

		if OBJECT_ID('tempdb..#AcctStrategy','U') is not null 
		drop table #AcctStrategy


		--insert into CMStrategyVariablesEntry (acctno,strategy)

		--select acctno ,strategy
		--from acct a WITH (NOLOCK),cmstrategy  b WITH (NOLOCK)
		--where ----a.accttype!='T'	and (	-- #9521 not StoreCard		
		--exists (select acctno from CMStrategyAcct C WITH (NOLOCK) where C.acctno =a.acctno and c.dateto is NULL AND c.strategy !='NON' AND c.strategy !='SCNON'    -- #9521 StoreCard
		--AND NOT (C.strategy = 'PWO' AND c.currentstep = 6)
		--AND NOT (C.strategy = 'SCPWO' AND c.currentstep = 3)			-- #9521 StoreCard
		--)

		--or (a.arrears >0 and a.outstbal>0 and a.accttype not in ('C','S'))
		----OR EXISTS (SELECT * FROM SR_ServiceRequest sr WHERE a.acctno=sr.acctno)-- UAT835 jec 09/09/09 --IP - 15/12/09 - UAT(937) - Service accounts must be in arrears to enter the Service Strategy
		--OR EXISTS (SELECT 1 FROM fintrans f WITH (NOLOCK) WHERE a.acctno = f.acctno 
		--AND f.transtypecode = 'BDW'
		----AND f.datetrans > DATEADD(m, -3, GETDATE())
		--AND f.datetrans > DATEADD(m, -3, getdate())				--IP - 13/02/12 - use @rundate
		--AND (a.bdwbalance > 0 or a.bdwcharges >0) and a.accttype not in ('C','S')) --IP - 15/09/09 - UAT(857) Catch accounts written off in the last 3 months. --IP - 02/10/09 - UAT(857) Check 'bdwcharges'.
		--OR EXISTS (SELECT 1 FROM TermsTypeTable t WITH (NOLOCK) WHERE t.termstype=a.termstype AND t.DeferredMonths!=0
		----AND EXISTS(SELECT * FROM agreement ag WHERE ag.acctno = a.acctno AND ag.datenextdue > GETDATE())) --IP/JC UAT(929)- 17/12/09
		--AND EXISTS(SELECT 1 FROM agreement ag WHERE ag.acctno = a.acctno AND ag.datenextdue > GETDATE())) --IP


		----or (a.bdwbalance > 0 and a.accttype not in ('C','S')) --IP/JC - 16/07/09 - Catch written off accounts 
		
			-------------- Modified By Zensar on 08/07/2019 ------- END
  
		-- repoession flags
		insert into CMStrategyVariablesEntry     (acctno,strategy)
		select acctno,strategy
		from acct a  WITH(NoLock) ,cmstrategy  b  WITH(NoLock) 
		where exists  (select 1 from acctcode f  WITH(NoLock) where f.acctno = a.acctno and    ---Zensar   - 07/08/19  --Modified the script replace * with 1
			f.code in ('FREP','PREP') and f.datecoded > @lastrundate)
		and b.isactive = 1
		and not exists (select 1 from CMStrategyVariablesEntry v WITH(NoLock)  where v.acctno = a.acctno) ---Zensar   - 07/08/19  --Modified the script replace * with 1
		----and a.accttype!='T'		-- #9521 not StoreCard
		
		-- 3 special arrangement flags broken
    


		-- now insert those accounts where current step is lastpostwriteoffStep
		insert into CMStrategyVariablesEntry     
		(acctno,strategy)
		select a.acctno,b.strategy
		from acct a  WITH(NoLock) ,cmstrategy  b WITH(NoLock) 
			WHERE EXISTS (SELECT 1 FROM fintrans f WITH(NoLock) , CMStrategyAcct CA  WITH(NoLock)  , FinTransAccount FA   WITH(NoLock)     ---Zensar   - 07/08/19  --Modified the script replace * with 1
		WHERE ((ca.strategy = 'PWO' AND cA.currentstep = @lastPostWriteOffStep)
				or (ca.strategy = 'SCPWO' AND cA.currentstep = @lastPostWriteOffStepSC))		-- #9521 StoreCard
		AND ca.dateto IS NULL 
		AND  FA.LinkedAcctNo = A.acctno 
		AND Fa.AcctNo = F.acctno  AND ca.acctno= A.acctno 
		AND f.transvalue <0 AND f.datetrans > @lastrundate)
		and not exists (select 1 from CMStrategyVariablesEntry v WITH(NoLock)  where v.acctno = a.acctno) ---Zensar   - 07/08/19  --Modified the script replace * with 1
		----and a.accttype!='T'		-- #9521 not StoreCard
		    
		-- speed up Storecard Collection testing - tempory fix
		if exists (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SCEODTest]') AND type in (N'U'))
		BEGIN
			--select '!!! Attention - StoreCard EOD Test Only !!!'
			select * into #tempStrategyVariables from CMStrategyVariablesEntry where acctno like '___9%'
			truncate TABLE CMStrategyVariablesEntry
			insert into CMStrategyVariablesEntry select * from #tempStrategyVariables
		END
		
		-------------- Commented By Zensar on 23/08/2019 as ARB is no more in use------- Start
		--update CMStrategyVariablesEntry
		--set AR3Broke = 'Y' where exists (select acctno  from bailaction b  WITH(NoLock) 
		--where b.acctno =CMStrategyVariablesEntry.acctno and b.code='ARB'
		--group by b.acctno having count(b.acctno) >=3)
		-------------- Commented By Zensar on 23/08/2019 as ARB is no more in use------- End

		-- 3 promises to pay broken
		update CMStrategyVariablesEntry
		set PTP3Broke = 'Y' where exists 
		(select acctno from bailaction b  WITH(NoLock) 
		where b.acctno =CMStrategyVariablesEntry.acctno and b.code='PTPB'
		group by b.acctno having count(b.acctno) >=3)
		--select * from code where code like 'st%'
		update CMStrategyVariablesEntry
		set STAFF = 'Y' where exists (select acctno from acctcode b  WITH(NoLock)  
		where b.acctno =CMStrategyVariablesEntry.acctno and b.code='STAF')

		update CMStrategyVariablesEntry
		set STAFF = 'Y' where exists (select acctno from acct b  WITH(NoLock)  where b.acctno =CMStrategyVariablesEntry.acctno and b.currstatus='9')
    
		-------------- Modified By Zensar on 08/07/2019 ------- Start

		IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')


			BEGIN
				-- #9521 StoreCard account
				update CMStrategyVariablesEntry
				set StoreCard='Y' where exists 
				(select 1 from acct a   WITH(NoLock) where a.acctno = CMStrategyVariablesEntry.acctno and a.AcctType='T')





    
			 END
		-------------- Modified By Zensar on 08/07/2019 ------- END





















		-- #9521 Credit account
		update CMStrategyVariablesEntry
		set CreditAcct='Y' where exists 
		(select 1 from acct a  WITH(NoLock)  where a.acctno = CMStrategyVariablesEntry.acctno and a.AcctType!='T')
    
		-- #11761 Singer account
		update CMStrategyVariablesEntry
		set SingerAcct='Y' where exists (select 1 from SingerAcct s WITH(NoLock)  where s.acctno = CMStrategyVariablesEntry.acctno)   ---Zensar   - 07/08/19  --Modified the script replace * with 1  
    
		--IP - 27/08/09 - UAT(822) - Do not update the AOK condition if the account has been written off....
		-- UAT827 jec ... or status=6 i.e Repossession.
		update CMStrategyVariablesEntry SET Aok ='Y' 
		where exists (select 1 from acct a  WITH(NoLock)  where a.acctno = CMStrategyVariablesEntry.acctno   ---Zensar   - 07/08/19  --Modified the script replace * with 1
					and a.AcctType!='T'			-- #9518 not Storecard  
					and (a.arrears <=0 or a.outstbal <=0))    
		and not exists (select 1 from acct a1  WITH(NoLock)  where a1.acctno = CMStrategyVariablesEntry.acctno  ---Zensar   - 07/08/19  --Modified the script replace * with 1
					and a1.AcctType!='T'			-- #9518 not Storecard  
					and (a1.bdwbalance > 0 OR currstatus='6'))		-- UAT827 jec 
    
		-------------- Modified By Zensar on 08/07/2019 ------- Start

		IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')
		BEGIN
			-- #9518 StoreCard account up to date
			update CMStrategyVariablesEntry SET Aok ='Y' 
			where exists (select 1 from acct a  WITH(NoLock) , #uptodate u where a.acctno = CMStrategyVariablesEntry.acctno   ---Zensar   - 07/08/19  --Modified the script replace * with 1
			and a.acctno=u.acctno
			and (a.arrears <=0 or a.outstbal <=0 or u.totminpayment+u.payment<=0)
			)
			-- payment made since last statement for at least minimum payment
			and exists(select 1 from fintrans f  WITH(NoLock) , #uptodate u where f.acctno = CMStrategyVariablesEntry.acctno  ---Zensar   - 07/08/19  --Modified the script replace * with 1
			and f.acctno=u.acctno
			and f.transtypecode='PAY' and datetrans >= u.dateto and f.transvalue+stmtminpayment<=0)  
     
			-------Special Arrangement impact-- 


			-- has arrangement
			--update CMStrategyVariablesEntry set Arrange ='Y' where exists (select 1 from spa a where a.acctno = CMStrategyVariablesEntry.acctno     ---Zensar   - 07/08/19  --Modified the script replace * with 1
			--and dateadded >@lastrundate)
		------------------------------
		END

   	------------- Modified By Zensar on 08/07/2019 ------- END

		-- is allocated to bailiff
		update CMStrategyVariablesEntry 
		set Bailalloc ='Y' 
		where exists (select 1                                                                        ---Zensar   - 07/08/19  --Modified the script replace * with 1

				  from follupalloc a WITH(NoLock) 
				  where a.acctno = CMStrategyVariablesEntry.acctno 
				  and (datealloc is null or datealloc >@lastrundate) and datedealloc is NULL
				  AND admin.CheckPermission(a.empeeno,381) = 1)

		-- balance more than one instalment
		update CMStrategyVariablesEntry set BalMIn  ='Y' where exists (select 1 from acct a  WITH(NoLock) ,instalplan i  WITH(NoLock)    ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where i.agrmtno = 1 and a.acctno =i.acctno and a.acctno = CMStrategyVariablesEntry.acctno 
		and a.outstbal >i.instalamount)
    
    
		--'Balance Outstanding >Balance Due -Instalment Value'
		-- e.g. after first month Agreement 1000, balance due =100, instalment 100
		-- 6 months in Balance os = 400 balance due = 600, instalment value = 100
		-- Arrears = balance due + amount paid
		--balance  = delivered amount(agrmttotal) + amount paid (which is negative)
		-- amount paid = (balance -agrmttotal) 
		--balance due = arrears - (balance-agrmttotal)
		--
		update CMStrategyVariablesEntry set BalOSDue   ='Y' where 
			exists (select 1 from acct a  WITH(NoLock)  INNER JOIN instalplan i WITH(NoLock) on a.acctno = i.acctno  -- 10/12/09 jec    ---Zensar   - 07/08/19  --Modified the script replace * with 1
				where a.acctno = CMStrategyVariablesEntry.acctno 
					and as400bal >(a.arrears-a.as400bal-a.agrmttotal-i.instalamount))
		-- delivery threshhold met
		update CMStrategyVariablesEntry set DELTHRESH   ='Y' where exists (select 1 from agreement g  WITH(NoLock)    ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where  g.acctno = CMStrategyVariablesEntry.acctno and g.datedel is not null)
		-- fully delivered
		update CMStrategyVariablesEntry set FULLDEL   ='Y' where exists ((select 1 from agreement g WITH(NoLock)      ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where  g.acctno = CMStrategyVariablesEntry.acctno and isnull(g.datefullydelivered ,'1-jan-1900') > '1-jan-1999'))
    
		--select datefullydelivered,datedel,acctno from agreement where datefullydelivered !=datedel
    
		update CMStrategyVariablesEntry set FullRep    ='Y' where exists (select 1 from acctcode a WITH(NoLock) where   ---Zensar   - 07/08/19  --Modified the script replace * with 1
		a.acctno = CMStrategyVariablesEntry.acctno and a.code ='FREP' AND datedeleted is null)
    
		-------------- Modified By Zensar on 08/07/2019 ------- Start


		--'Arrangement Expires' 
    
		--update CMStrategyVariablesEntry set AGREXP    ='Y' where exists ( select 1 from instalplan i  WITH(NoLock)     ---Zensar   - 07/08/19  --Modified the script replace * with 1
		----where i.acctno = CMStrategyVariablesEntry.acctno and i.datelast <getdate() )
		-- where i.acctno = CMStrategyVariablesEntry.acctno and i.datelast <@rundate )			--IP - 13/02/12 - use @rundate
    
		-------------- Modified By Zensar on 08/07/2019 ------- END

		-- What is this attempting to do??? - commenting out for present   -- jec 21/08/09
		--update CMStrategyVariablesEntry set HOLDAREXP    ='Y' where exists ( select * from spa a
		--where a.acctno = CMStrategyVariablesEntry.acctno and dateexpiry >dateadd(day,-3, @lastrundate))
    
		--Customer has missed first instalment
		update CMStrategyVariablesEntry set MS1INS    ='Y' where exists (select 1 from acct a WITH(NoLock), instalplan i  WITH(NoLock)   ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where a.acctno = CMStrategyVariablesEntry.acctno and i.acctno = a.acctno and i.agrmtno = 1 
		and a.AcctType!='T'			-- #9521  Not StoreCard   
		--and a.arrears between .9 * i.instalamount and 1.1 * i.instalamount -- UAT764 - jec 
		--and i.datefirst between DATEADD(week,-2,@lastrundate) and getdate()) -- allowing 2 weeks as was otherwise missing.
		  and i.datefirst between DATEADD(week,-2,@lastrundate) and @rundate) -- allowing 2 weeks as was otherwise missing.		--IP - 13/02/12 - use @rundate

		-------------- Modified By Zensar on 08/07/2019 ------- Start



















		IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True')
			BEGIN
		--Customer has missed first Payment -- #9514  StoreCard		
		update CMStrategyVariablesEntry set MS1PAY    ='Y' where exists 
			(select 1 from acct a  WITH(NoLock) ,fintrans f  WITH(NoLock) INNER JOIN  StoreCardStatement s  WITH(NoLock) on f.acctno=s.acctno
				where a.acctno=CMStrategyVariablesEntry.acctno and f.acctno=a.acctno 
				and datetrans between s.datefrom  and s.datePaymentDue and transtypecode='PAY'
				and s.datefrom = (select  MIN(datefrom) from StoreCardStatement s2  WITH(NoLock) where f.acctno=s2.acctno  )
				and a.AcctType='T'
				having ISNULL(SUM(transvalue)+ sum(stmtminpayment),0)>0
				and not exists( select 1 from CMStrategyAcct s  WITH(NoLock)where s.acctno = CMStrategyVariablesEntry.acctno and s.strategy='SCNS')		-- jec 17/02/12 only set if not prev in Non starter strat
				-- no payment exists between dates
				or not exists(select 1 from acct a  WITH(NoLock) INNER JOIN  StoreCardStatement s  WITH(NoLock) on a.acctno=s.acctno,fintrans f
				where a.acctno=CMStrategyVariablesEntry.acctno and f.acctno=a.acctno and datetrans between s.datefrom  and s.datePaymentDue and transtypecode='PAY'
				and s.datefrom = (select MIN(datefrom) from StoreCardStatement s2  WITH(NoLock) where a.acctno=s2.acctno )
				and a.AcctType='T')
				and not exists( select COUNT(*) from fintrans f WITH(NoLock) where f.acctno=CMStrategyVariablesEntry.acctno and transtypecode='PAY' having COUNT(*)>1)   -- #9778 more than one payment made
				and not exists( select 1 from CMStrategyAcct s WITH(NoLock) where s.acctno = CMStrategyVariablesEntry.acctno and s.strategy='SCNS')		-- jec 17/02/12 only set if not prev in Non starter strat
			)
			 END
		---------------- Modified By Zensar on 08/07/2019 ------- END
		--

		update CMStrategyVariablesEntry set NonArrears    ='Y' where exists (select 1 from acct a WITH(NoLock) where a.acctno =    ---Zensar   - 07/08/19  --Modified the script replace * with 1
		 CMStrategyVariablesEntry.acctno and a.currstatus ='S') -- to do non arrears strat into arrears
    
		update CMStrategyVariablesEntry set PartRep    ='Y' where exists (select 1 from acctcode a WITH(NoLock) where    ---Zensar   - 07/08/19  --Modified the script replace * with 1
		a.acctno = CMStrategyVariablesEntry.acctno and a.code ='FREP' AND datedeleted is null)
    
		--update CMStrategyVariablesEntry set PTPComp    ='Y' where exists (select * from bailaction b , fintrans f
		--where b.acctno =CMStrategyVariablesEntry.acctno and b.acctno =f.acctno and f.datetrans >b.dateadded and f.transtypecode = 'PAY' and 
		--f.datetrans >@lastrundate and f.datetrans <dateadd(day,1,b.datedue))
    
		--update CMStrategyVariablesEntry set PTPComp    ='Y' where exists (select * from bailaction b , fintrans f
		--where b.acctno =CMStrategyVariablesEntry.acctno and b.acctno= f.acctno and f.datetrans >b.dateadded and f.transtypecode = 'PAY' and 
		--f.datetrans >@lastrundate and f.datetrans <dateadd(day,1,b.datedue))
    
		--IP - 02/09/09 - 5.2 UAT(828)- PTPComp was incorrectly being set as completed.
		update CMStrategyVariablesEntry set PTPComp = 'Y' where exists ( select 1 from bailactionPTP b  WITH(NoLock)  ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where b.acctno = CMStrategyVariablesEntry.acctno --and b.code = 'PTP' 
		and b.dateadded = (select max(b1.dateadded) from bailactionPTP b1 WITH(NoLock)  --Zensar   -
								where b1.acctno = b.acctno

								--and b1.code = 'PTP'
							)
		and exists (select 1 from fintrans f WITH(NoLock) where f.acctno = b.acctno     ---Zensar   - 07/08/19  --Modified the script replace * with 1
						and f.datetrans > b.dateadded

						and f.transtypecode = 'PAY'
						and f.datetrans < dateadd(d, 1, b.datedue)
						having sum(-f.transvalue) >= b.actionvalue))				
 
		--IP - 01/10/09 - UAT(855)
		--update CMStrategyVariablesEntry set PTPEnt     ='Y' where exists (select * from bailaction b 
		--where b.acctno =CMStrategyVariablesEntry.acctno and b.code ='PTP' and  b.dateadded > @lastrundate)
    
		update CMStrategyVariablesEntry set PTPEnt     ='Y' where exists (select 1 from bailactionPTP b WITH(NoLock)
		where b.acctno = CMStrategyVariablesEntry.acctno --and b.code ='PTP' 
		and  b.dateadded > @lastrundate
		and not exists (select 1 from fintrans f WITH(NoLock)   ---Zensar   - 07/08/19  --Modified the script replace * with 1

							where f.acctno = b.acctno
							and f.datetrans > b.dateadded 
							and f.transtypecode in('PAY', 'REF', 'COR')
							having sum(-f.transvalue) >= b.actionvalue))
    
		--IP - 06/10/09 - UAT(906)
		--update CMStrategyVariablesEntry set PTPPEND     ='Y' where exists (select * from bailaction b 
		--where b.acctno =CMStrategyVariablesEntry.acctno and b.code ='PTP' and  b.dateadded < @lastrundate and b.datedue >getdate())
   
		update CMStrategyVariablesEntry set PTPPEND     ='Y' where exists (select 1 from bailactionPTP b  WITH(NoLock)
		where b.acctno =CMStrategyVariablesEntry.acctno --and b.code ='PTP' 
		--and  b.dateadded < getdate() and b.datedue >getdate()
		and  b.dateadded < @rundate and b.datedue >@rundate				--IP - 13/02/12 - use @rundate
		and b.dateadded = (select  max(b1.dateadded) from bailactionPTP b1 WITH(NoLock)  --Zensar   - 07/08/19 - 
							where b1.acctno = b.acctno

							--and b1.code = b.code
							
							)
		and not exists (select 1 from fintrans f WITH(NoLock)         ---Zensar   - 07/08/19  --Modified the script replace * with 1


							where f.acctno = b.acctno
							and f.datetrans > b.dateadded

							and f.transtypecode in ('PAY', 'REF', 'COR')
							having sum(-f.transvalue) >= b.actionvalue))
    
    
		update CMStrategyVariablesEntry set REPO     ='Y' where exists (select 1 from fintrans f WITH(NoLock)   ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where f.acctno =CMStrategyVariablesEntry.acctno and f.transtypecode ='REP' 
				and  DATEADD(hh,16,f.datetrans) > @lastrundate)		-- UAT827 jec - add 16 hours to Repo date (zero time)
	
		--IP - 10/10/08 - UAT5.2 - UAT(525) condition used to exit the account to 'Write Off' strategy
		--if the 'woffnow' or 'woffever' conditions are hit meaning that a BDW has been processed on the account.
		update CMStrategyVariablesEntry set woffnow     ='Y' where exists (select 1 from fintrans f WITH(NoLock)     ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where f.acctno =CMStrategyVariablesEntry.acctno and f.transtypecode ='BDW' and  f.datetrans  > @lastrundate
		AND transvalue <=0)

		update CMStrategyVariablesEntry set woffever     ='Y' where exists (select 1 from fintrans f WITH(NoLock)   ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where f.acctno =CMStrategyVariablesEntry.acctno and f.transtypecode ='BDW' AND transvalue <=0)
    
		--IP - 10/10/08 - UAT5.2 - UAT(525) condition used to exit the account to 'Write Off' strategy
		--if the 'Levy' condition has been hit meaning that an action has been added onto the account
		--to say that levy has been processed on the Customers account.
		update CMStrategyVariablesEntry set Levy		='Y' where exists(select 1 from bailaction b WITH(NoLock)  ---Zensar   - 07/08/19  --Modified the script replace * with 1
		where b.acctno = CMStrategyVariablesEntry.acctno and b.code = 'LEVY' and b.dateadded > @lastrundate)
	
		--update CMStrategyVariablesEntry
		--set OpenSR = 'Y' where exists (SELECT * FROM SR_ServiceRequest sr 
		--WHERE sr.acctno =CMStrategyVariablesEntry.acctno AND ServiceType = 'C' AND Status <> 'C')
    
		update CMStrategyVariablesEntry																	--#10912
		set OpenSR = 'Y' where exists (SELECT 1 FROM SR_Summary sr  WITH(NoLock)                       ---Zensar   - 07/08/19  --Modified the script replace * with 1

		WHERE sr.acctno =CMStrategyVariablesEntry.acctno AND DateClosed = '01-01-1900')
    
		-- Service Request in last 30 days   jec 26/06/09 
		-- UAT733 moved to CMEOD_UpdateStrategyandStepConditionsSP  
	   -- update CMStrategyVariablesEntry
	   -- set SRA = 'Y' where exists (SELECT * FROM SR_ServiceRequest sr 
				--INNER JOIN SR_Resolution r on sr.serviceRequestNo=r.serviceRequestNo
	   -- WHERE sr.acctno =CMStrategyVariablesEntry.acctno AND ServiceType = 'C' AND Status = 'C'
				--and r.DateClosed> dateadd(d,-30,getdate()))
	
		-- Deferred Account Type   jec 26/06/09   
		update CMStrategyVariablesEntry
		set DeferAcct = 'Y' 
		where exists (SELECT 1 FROM Acct a WITH(NoLock) INNER JOIN TermsType t  WITH(NoLock) on a.termstype=t.termstype    ---Zensar   - 07/08/19  --Modified the script replace * with 1
						INNER JOIN dbo.instalplan i WITH(NoLock) ON i.acctno = a.acctno
						WHERE a.acctno =CMStrategyVariablesEntry.acctno AND t.deferredMonths!=0
						--AND i.datefirst > GETDATE())
						AND i.datefirst > @rundate)			--IP - 13/02/12 - use @rundate
						--IP/JC - 17/12/09 - UAT(929) - and not previously been in the DFA strategy 
		AND NOT EXISTS(SELECT 1 FROM cmstrategyacct a WITH(NoLock)     ---Zensar   - 07/08/19  --Modified the script replace * with 1

						WHERE a.acctno = CMStrategyVariablesEntry.acctno
						AND a.strategy = 'DFA')

		-- Written Off   jec 26/06/09    
	  --  update CMStrategyVariablesEntry
	  --  set Woff = 'Y' where exists (SELECT * FROM acct a INNER JOIN fintrans f on f.acctno = a.acctno
			--				and f.transtypecode='BDW'
			--WHERE f.acctno =CMStrategyVariablesEntry.acctno AND a.currstatus='S')
		
			--IP - 05/10/09 - UAT(902)
			update CMStrategyVariablesEntry
			set Woff = 'Y' where exists (SELECT 1 FROM acct a WITH(NoLock) INNER JOIN fintrans f WITH(NoLock)on f.acctno = a.acctno    ---Zensar   - 07/08/19  --Modified the script replace * with 1
							and f.transtypecode='BDW'
							and f.datetrans = (select max(f2.datetrans) from fintrans f2 WITH(NoLock)      --Zensar   - 07/08/19 - 
												where f2.acctno = f.acctno 
												and f2.transtypecode = f.transtypecode
												
												)
			WHERE f.acctno =CMStrategyVariablesEntry.acctno AND a.currstatus='S'
			and not exists (select 1 from fintrans f3 WITH(NoLock)                   ---Zensar   - 07/08/19  --Modified the script replace * with 1

								where f3.acctno = f.acctno
								and f3.transtypecode = 'BDR'
								and f3.datetrans > f.datetrans))
		
		--IP - 15/09/09 - UAT(862)
		UPDATE CMStrategyVariablesEntry 
		SET Loan = 'Y' WHERE EXISTS (SELECT 1 FROM acct a WITH(NoLock), TermsTypeTable t WITH(NoLock)               ---Zensar   - 07/08/19  --Modified the script replace * with 1

										WHERE a.acctno = CMStrategyVariablesEntry.acctno
										AND a.termstype = t.termstype
										AND t.isLoan = 1)
									
	
		--#13720 - CR12949
		UPDATE CMStrategyVariablesEntry
		SET ReadyAssist = 'Y' WHERE EXISTS(select 1 from vw_ReadyAssist v WITH(NoLock)        ---Zensar   - 07/08/19  --Modified the script replace * with 1
											where v.acctno = CMStrategyVariablesEntry.acctno)
										
		-- now copy Entry Conditions to Steps & Exit

		insert into CMStrategyVariablesSteps select * from CMStrategyVariablesEntry	WITH(NoLock)	
		

		insert into CMStrategyVariablesExit select * from CMStrategyVariablesEntry WITH(NoLock)
    
		--IP - 26/08/09 - UAT(816) - Moved from CNEod_UpdateandDoStepsInStrategiesSP 
		 ---- Now we are going to make sure that currentstep is 1 for new accounts where current step is 0.
		update CMStrategyAcct set currentstep =1 where currentstep = 0 and dateto is null
Return
GO




