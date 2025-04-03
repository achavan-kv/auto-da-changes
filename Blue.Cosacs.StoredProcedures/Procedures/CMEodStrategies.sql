


if exists (select * from sysobjects where name ='CMEod_Strategies')
drop procedure CMEod_Strategies
go


SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================================================
-- Version	:002
-- Change Control
-- --------------
-- Date      By			Description
-- ----      --			-----------
-- 08/08/19  Zensar Strategy Job Optimization : Optimised the stored procedure for performance by putting Nolock and replacing * with 1 in all exist
-- 07/08/19  Zensar Strategy Job Optimization : Comment SPA Code as Special arrangement is no longer used in cosacs
-- 09/08/19  Zensar Strategy Job Optimization : Added Store Card functionality check based on country maintenance.
-- 12/08/19  Zensar Strategy Job Optimization : Added SMS functionality check based on country maintenance.
-- ========================================================================================================

CREATE procedure [dbo].[CMEod_Strategies]


@return int OUTPUT
as
	declare @statement sqltext ,@strategy varchar(6),@lastrundate datetime,@runno INT
	declare @rundate DATETIME				--IP - 13/02/12

	SET NOCOUNT ON 
	set @return = 0
 
	Select 'Start', 'Call1',getdate()

	select @runno= isnull(max(runno),0) from interfacecontrol i where i.interface ='COLLECTIONS'
	select @rundate= getdate()		-- jec 15/02/12 
	if @runno >1
		select @lastrundate = datestart from interfacecontrol i where i.interface ='COLLECTIONS' and runno = @runno -1
	else
		set @lastrundate = @rundate	--IP - 13/02/12 - use @rundate	-- LW72899 - Start from Day1


	  --set @lastrundate = getdate()	-- LW72899 - Start from Day1
   
	   ---Commented by Zensar------Start
	  --  Select 'CMEod_GenerateSPASummarySP', 'Call1',getdate()
	  -- --IP - 07/09/09 - UAT(809) - Procedure that populates a new SPASummary table for accounts with an SPA.
	  -- print ' Create SPA Summary'
	  -- exec CMEod_GenerateSPASummarySP @rundate = @rundate, @return = @return out	--IP - 13/02/12 - use @rundate
	  -- if @return != 0
	  -- begin
		 --Print 'failure of procedure CMEod_GenerateSPASummary - please contact support'
	  -- end
	  ---Commented by Zensar------End


	--CMClashingStrategyAssigments
	-- First we are going to get a table full of accounts with the predefined options where these accounts are in arrears
	-- Then we are going to see for each condition whether they qualify....

		--------------------------CMEod_UpdateGeneralConditionsSP------------------------------------------------------------------
		--Select 'CMEod_UpdateGeneralConditionsSP', 'Call1',getdate()
		set @rundate = GETDATE()			-- Date must be different for each SP exec
		--print ' updating general conditions'
		
		exec CMEod_UpdateGeneralConditionsSP @lastrundate = @lastrundate, @rundate = @rundate, @return = @return out-- some conditions can apply to all accounts --IP - 13/02/12 - use @rundate
	   
	   if @return !=0
	   begin
		 Print 'failure of procedure CMEod_UpdateGeneralConditionsSP - please contact support' 
	   end
   
		--------------------------CMEOD_UpdateStrategyandStepConditionsSP------------------------------------------------------------------
    	--Select 'CMEOD_UpdateStrategyandStepConditionsSP', 'Call1',getdate()
		set @rundate = GETDATE()				-- Date must be different for each SP exec
		--print ' updating general specific exit strategy and step conditions'
		
		exec CMEOD_UpdateStrategyandStepConditionsSP @lastrundate =@lastrundate, @rundate = @rundate, @loadtype ='Steps&Exit',	--IP - 13/02/12 - use @rundate
		@return =@return OUT
		
		if @return !=0
		begin
			Print 'failure of procedure CMEOD_UpdateStrategyandStepConditionsSP - please contact support' 
		end

		--------------------------CNEOD_UpdateandDoStepsInStrategiesSP------------------------------------------------------------------
		--Select 'CNEOD_UpdateandDoStepsInStrategiesSP', 'Call1',getdate()
		set @rundate = GETDATE()				-- Date must be different for each SP exec
		-- update the steps and do the steps i.e. send letters, assign to worklists, exit from and send sms' in strategies
		--print ' updating and doing steps in strategies'
		
		exec CNEOD_UpdateandDoStepsInStrategiesSP @rundate = @rundate, @return = @return OUT		--IP - 13/02/12 - use @rundate
		if @return !=0
		begin
			Print 'failure of procedure CNEOD_UpdateandDoStepsInStrategiesSP - please contact support' 
		end

		--------------------------CNEOD_UpdateandDoStepsInStrategiesSP------------------------------------------------------------------
		--Select 'CNEOD_UpdateandDoStepsInStrategiesSP', 'Call2',getdate()
		set @rundate = GETDATE()				-- Date must be different for each SP exec
		---- do this again as might generate letter then move into worklist straightaway for instance
		--print ' updating and doing steps in strategies again'
	   
		exec CNEod_UpdateandDoStepsInStrategiesSP @rundate = @rundate, @return = @return OUT		--IP - 13/02/12 - use @rundate
		if @return !=0
		begin
			Print 'failure of procedure CNEod_UpdateandDoStepsInStrategiesSP - please contact support' 
		end
		
		--------------------------CNEOD_UpdateandDoStepsInStrategiesSP------------------------------------------------------------------
	   --Select 'CNEOD_UpdateandDoStepsInStrategiesSP', 'Call3',getdate()
	   ----IP - 24/08/09 - UAT(813) - Need to execute this procedure again as account may meet an OnDay condition,
	   ----which sends to a strategy, and when in this strategy there may be an OnDay 0 step to send to a worklist.
	   set @rundate = GETDATE()				-- Date must be different for each SP exec
	   --print ' updating and doing steps in strategies again'
	   
	   exec CNEod_UpdateandDoStepsInStrategiesSP @rundate = @rundate, @return = @return OUT		--IP - 13/02/12 - use @rundate
	   
	   if @return !=0
	   begin
		 Print 'failure of procedure CNEod_UpdateandDoStepsInStrategiesSP - please contact support' 
	   end
	
		--------------------------CMEOD_UpdateStrategyandStepConditionsSP------------------------------------------------------------------
		--Select 'CMEOD_UpdateStrategyandStepConditionsSP', 'Call1',getdate()
		set @rundate = GETDATE()				-- Date must be different for each SP exec
   
		exec CMEOD_UpdateStrategyandStepConditionsSP @lastrundate =@lastrundate, @rundate = @rundate, @loadtype ='Entry',		--IP - 13/02/12 - use @rundate
		@return =@return OUT
   



	   if @return !=0
	   begin
		 Print 'failure of procedure CMEOD_UpdateStrategyandStepConditionsSP - please contact support' 
	   end

	   --------------------------CMEod_AssignAccountstoStrategiesSP------------------------------------------------------------------
		--Select 'CMEod_AssignAccountstoStrategiesSP', 'Call1',getdate()
		---- assign new accounts to strategies...
		set @rundate = GETDATE()				-- Date must be different for each SP exec

		print 'assigning new accounts to strategies '
   
		exec CMEod_AssignAccountstoStrategiesSP @rundate = @rundate, @return = @return OUT										--IP - 13/02/12 - use @rundate
   
		if @return !=0
		begin
			Print 'failure of procedure CMEod_AssignAccountstoStrategiesSP - please contact support' 
		end
   
	   --IP - 01/10/09 - UAT(891)When accounts are assigned into new strategies, the steps need to be processed
		--for these accounts as currently they are just inserted into the strategy and day=0 steps not processed. 
		--Therefore the steps conditions need to be updated for these accounts and then processed.
	 
		 --------------------------CMEOD_UpdateStrategyandStepConditionsSP------------------------------------------------------------------
		--Select 'CMEOD_UpdateStrategyandStepConditionsSP', 'Call1',getdate()


		set @rundate = GETDATE()				-- Date must be different for each SP exec
		--print ' updating general specific exit strategy and step conditions'






	

		exec CMEOD_UpdateStrategyandStepConditionsSP @lastrundate =@lastrundate, @rundate = @rundate, @loadtype ='Steps&Exit',	--IP - 13/02/12 - use @rundate
		@return =@return OUT
   
		if @return !=0
		begin
			Print 'failure of procedure CMEOD_UpdateStrategyandStepConditionsSP - please contact support' 
		end

		--------------------------CNEOD_UpdateandDoStepsInStrategiesSP------------------------------------------------------------------
		--Select 'CNEOD_UpdateandDoStepsInStrategiesSP', 'Call4',getdate()
		-- update the steps and do the steps i.e. send letters, assign to worklists, exit from and send sms' in strategies
		set @rundate = GETDATE()				-- Date must be different for each SP exec
		--print ' updating and doing steps in strategies'








   
		exec CNEOD_UpdateandDoStepsInStrategiesSP @rundate = @rundate, @return = @return OUT		--IP - 13/02/12 - use @rundate







		if @return !=0
		begin
			Print 'failure of procedure CNEOD_UpdateandDoStepsInStrategiesSP - please contact support' 
		end
 
		-- build Worklist volumes
		Truncate TABLE CMWorklistSOD

		insert into CMWorklistSOD (Worklist,NumberSOD,datefrom,acctno,outstbal,arrears)
		select w.worklist ,COUNT(*) as NumberSOD,datefrom,w.acctno,SUM(a.outstbal),SUM(a.arrears)
		from CMWorklistsAcct w with(NoLock) INNER JOIN acct a with(NoLock) on w.acctno=a.acctno
		where w.dateto is null
		group by w.acctno,w.worklist,w.datefrom
		union
		select w.worklist ,0,null,'',0,0
		from CMWorklist w with(NoLock) where not exists(select 1 from CMWorklistsAcct a with(NoLock) where w.WorkList=a.Worklist)












		-- build Strategy volumes
		Truncate TABLE CMStrategySOD









		insert into CMStrategySOD (Strategy,NumberSOD,datefrom,acctno,outstbal,arrears)
		select s.strategy ,COUNT(*) as NumberSOD,datefrom,s.acctno,SUM(a.outstbal),SUM(a.arrears)
		from CMStrategyAcct s with(NoLock) INNER JOIN acct a with(NoLock) on s.acctno=a.acctno
		where s.dateto is null
		group by s.acctno,s.strategy,s.datefrom
		union
		select s.strategy  ,0,null,'',0,0
		from CMStrategyAcct s with(NoLock) where not exists(select 1 from CMStrategyAcct a with(NoLock) where s.strategy=a.strategy)



		Print ' Doing non-arrears strategy'









		IF EXISTS(select value from CountryMaintenance where CodeName = 'StoreCardEnabled' and value= 'True') ---Added by Zensar
		BEGIN
		--IP - 21/03/12 - #9808
			select acctno,MAX(Dateto) as DateTo,MAX(datepaymentdue) as datepaymentdue,SUM(stmtminpayment) as totminpayment,CAST(0 as MONEY) as stmtminpayment,CAST(0 as MONEY) as payment,
			CAST(0 as MONEY) as currStatPayments
			into #scards
			from dbo.StoreCardStatement s with(NoLock)
			group by acctno


			alter TABLE #scards ADD  CONSTRAINT [pk_scards] PRIMARY KEY CLUSTERED ([acctno] ASC)
			-- update with payments received 
			UPDATE #scards
			set 
				payment=(select ISNULL(SUM(transvalue),0) from fintrans f with(NoLock) where f.acctno=m.acctno and transtypecode='PAY'),
				stmtminpayment=(select SUM(stmtminpayment) from StoreCardStatement s with(NoLock) where s.acctno=m.acctno and m.datepaymentdue=s.datepaymentdue), -- last min payment
				currStatPayments=(select ISNULL(SUM(transvalue),0) from fintrans f with(NoLock) where f.acctno=m.acctno and transtypecode='PAY' and f.datetrans > m.dateto)
			from #scards m
		END







	DECLARE @SMSApplicable char(1)  ---Added by Zensar
	select @SMSApplicable = convert(char(1), Value) from countrymaintenance with(NoLock) where codename = 'SMSApplicable'  ---Added by Zensar









	if(@SMSApplicable = 'Y')  ---Added by Zensar
	BEGIN
		-- now non-arrears generate SMS for all those accounts X days before SMS due
		declare @smsdays smallint, @smscode varchar(4)
		-- check there is an sms being sent for non-arrears accounts and also check if 
		-- there is a delay for them....

		select @smsdays =isnull(convert(smallint,c.Operator1),0)
		from CMStrategyCondition c with(NoLock),cmstrategy s with(NoLock)
		where s.strategy='NON'			---or s.strategy='SCNON' )			-- jec 17/02/12 storecard
		and s.strategy = c.strategy
		and c.condition = 'B4DUEDAY' and s.isactive !=0

		select @smscode=isnull(c.Actioncode,'')
		from CMStrategyCondition c with(NoLock),cmstrategy s with(NoLock)
		where s.strategy='NON' and s.strategy = c.strategy
		and c.StepActiontype ='S' and s.isactive !=0

		if @smsdays >0 and @smscode !=''
		begin
			print 'inserting into sms'
			
			IF EXISTS(select value from CountryMaintenance with(NoLock) where CodeName = 'StoreCardEnabled' and value= 'True')
			BEGIN

			--IP - 21/03/12 - #9808 - Replaces the below. Now also cater for Store Card accounts in SCNON strategy.
				insert into SMS(acctno,dateadded,code) 
				select a.acctno,@rundate,@smscode	
				from agreement g  with(NoLock) inner join acct a  with(NoLock)on a.acctno = g.acctno 
				inner join instalplan i with(NoLock) on i.acctno= g.acctno and i.agrmtno =g.agrmtno and i.agrmtno = 1
				left join #scards u on a.acctno = u.acctno			--StoreCard accounts
				where
				(
				   g.datenextdue between dateadd(day,@smsdays,@lastrundate) and dateadd(day,@smsdays,@rundate)			
				   and g.datenextdue > @rundate							
				   and a.outstbal > (.9 * i.instalamount)  -- make sure at least one instalment outstanding
				   and a.arrears between -1 and (.9 * i.instalamount)
				   and i.acctno= g.acctno and i.agrmtno =g.agrmtno and i.agrmtno = 1
				   and a.acctno like '___0%' 
				   and a.accttype !='T'		--IP - 21/03/12 - Not a Store Card
	
				)
				or (								--Cater for Store Card
				a.accttype = 'T'
				and  u.datepaymentdue > @rundate
				and @rundate > dateadd(day, -@smsdays, u.datepaymentdue)		--The minimum amount for the current statement has not been paid
				and currStatPayments + stmtminpayment >=0
				)
				and not exists (select 1 from  cmstrategyacct s with(NoLock) where s.acctno =a.acctno
				and s.strategy !='NON'
				and s.strategy !='SCNON'				
				and s.dateto is null)
			END
			ELSE
			BEGIN
				insert into SMS(acctno,dateadded,code) 
				select a.acctno,@rundate,@smscode	
				from agreement g with(NoLock) inner join acct a with(NoLock) on a.acctno = g.acctno 
				inner join instalplan i with(NoLock) on i.acctno= g.acctno and i.agrmtno =g.agrmtno and i.agrmtno = 1
				where
				(

				   g.datenextdue between dateadd(day,@smsdays,@lastrundate) and dateadd(day,@smsdays,@rundate)			
				   and g.datenextdue > @rundate							
				   and a.outstbal > (.9 * i.instalamount)  -- make sure at least one instalment outstanding
				   and a.arrears between -1 and (.9 * i.instalamount)
				   and i.acctno= g.acctno and i.agrmtno =g.agrmtno and i.agrmtno = 1
				   and a.acctno like '___0%' 
				   and a.accttype !='T'		--IP - 21/03/12 - Not a Store Card
	
				)







				and not exists (select 1 from  cmstrategyacct s  with(NoLock) where s.acctno =a.acctno
				and s.strategy !='NON'
				and s.strategy !='SCNON'				
				and s.dateto is null)
			END

		   --insert into SMS(acctno,dateadded,code) 
		   ----select a.acctno,getdate(),@smscode 
		   --select a.acctno,@rundate,@smscode					--IP - 13/02/12 - use @rundate
		   --from agreement g,acct a,instalplan i
		   --where a.acctno = g.acctno and 
		   ----g.datenextdue between dateadd(day,@smsdays,@lastrundate) and dateadd(day,@smsdays,getdate())
		   --g.datenextdue between dateadd(day,@smsdays,@lastrundate) and dateadd(day,@smsdays,@rundate)			--IP - 13/02/12 - use @rundate
		   ----and g.datenextdue > getdate()
		   --and g.datenextdue > @rundate							--IP - 13/02/12 - use @rundate
		   --and a.outstbal > (.9 * i.instalamount)  -- make sure at least one instalment outstanding
		   --and a.arrears between -1 and (.9 * i.instalamount)
		   --and i.acctno= g.acctno and i.agrmtno =g.agrmtno and i.agrmtno = 1
		   --and a.acctno like '___0%' 
		   ---- and doesn't have record in another strategy
		   --and not exists (select * from  cmstrategyacct s where s.acctno =a.acctno
		   --and s.strategy !='NON'
		   --and s.strategy !='SCNON'				-- jec 17/02/12 storecard
		   --and s.dateto is null)
   		end
	END























	--------------------------CM_ApplyZonesSP------------------------------------------------------------------
	set @rundate = GETDATE()				-- Date must be different for each SP exec
	IF @return = 0 
	 --Select 'CM_ApplyZonesSP', 'Call1',getdate()
	EXEC CM_ApplyZonesSP @zone = 'All', @return =@return OUT 

	--------------------------CM_AutoAllocateBailiffs------------------------------------------------------------------
	IF @return = 0 
	--Select 'CM_AutoAllocateBailiffs', 'Call1',getdate()
	EXEC CM_AutoAllocateBailiffs @rundate = @rundate, @return = @return OUT			--IP - 13/02/12 - use @rundate
	
	--Select 'Finish', 'Call1',getdate()
	--NM & IP - 05/01/09 - CR976 - Need to remove call reminders that are older than a month
	
	delete from cmreminder
	--where reminderdatetime < dateadd(m, -1, getdate())
	where reminderdatetime < dateadd(m, -1, @rundate)									--IP - 13/02/12 - use @rundate

GO


