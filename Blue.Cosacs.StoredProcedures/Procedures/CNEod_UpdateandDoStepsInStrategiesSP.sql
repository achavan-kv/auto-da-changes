
GO

IF EXISTS (
SELECT * 
FROM sysobjects 
WHERE name = 'CNEod_UpdateandDoStepsInStrategiesSP')  
 DROP PROCEDURE CNEod_UpdateandDoStepsInStrategiesSP
go

-- This procedure will update accounts steps and generate letters and SMS's based on what is set up in strategies.
-- Question do we a) load each strategy and loop through that one or
-- b) do it generically
-- Also what about Nikki's point
-- Step 1   Action
-- Step 2   Another action with no delay - do we wait until the next time or can we somehow do it this time.
-- Could we call this procedure twice to do this??
-- I think it would work if we called the procedure twice.
-- Now do we have to do this for each step...?? yes we have go through each strategy 
-- and then update the steps where dynamically it works

-- ============================================================================================
-- Version		:002
-- Author:		Alex Ayscough
-- Create date: ?
-- Description:	This procedure updates the steps and processes the actions for the steps for
--				the accounts in a strategy.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- Amendments: IP - 08-10 October 2008 - Exit conditions were not being catered for. Therefore
--			   the procedure will check the exit conditions for a strategy and update the accounts
--			   in a strategy if an exit condition has been hit.	This involves updating the 
--			   'CMStrategyacct', 'CMWorklistsacct' table. A new record should be inserted into
--			   the 'CMStrategyacct' table for the strategy that the account has exited to.	
--			   Also changed the date column from 'datefrom' to 'dateincurrentstep' in the query
--			   that updates the 'CMWorklistsacct' table when hitting a step where the action causes
--			   the account to leave the strategy and worklist.
-- 30/07/09  jec UAT767 New Strategy Variable tables
-- 10/08/09 jec UAT780 Datefrom incorrect for account Strategy
-- 20/08/09 - IP - UAT(799) - Step relating to 'HoldAccPTP' condition was not being returned therefore the CMStrategyAcct.Currentstep 
--			  was not being updated. SMS was not being inserted into SMS table for an account due to incorrect join. Will now correctly join
--			  on CMStrategyAcct.Dateincurrentstep. 'HoldAccPTP' and 'HoldDays' will now be set to 'Y' if the condition has been met.
--			  Once updated to 'Y' process the next step. If they are still held, then they will remain as 'R'.
-- 24/08/09 - IP - UAT(813)
-- 26/08/09 - IP - UAT(816) - Moved update statement that updates currentstep from 0 to 1 for new accounts to CMEod_UpdateGeneralConditionsSP.
-- 09/09/09 - IP - UAT(834) - Added logic to check 'PrevStrat' steps, as a sequence of multiple 'PrevStrat' steps were not being processed
--			  at the same time. If a DayX = 0 step was encountered which was not at step 1 but after a series of 'PrevStrat' steps, the step was not being processed
--			  immediately after the last 'PrevStrat' step as the condition was not set due to an account exiting a strategy and entering another.
--			  If an account was in a previous strategy, it was not exiting to this strategy. 
-- 14/09/09 - jec UAT856 check for "manual" strategy when checking previous strategies
-- 05/10/09 - IP - UAT(903) Accounts that are sent back to a previous strategy after hitting a 'PrevStrat' condition, when re-entering this strategy, did not send the 
--			  the account back into the worklist if a DayX = 0 step was hit. I have moved the sql which updates the DayX condition to the bottom of the procedure which
--			  should apply to accounts inserted into a new strategy from a step condition or from meeting a 'PrevStrat' condition.
-- 11/11/09 - IP - UAT(864) After processing 'PrevStrat' steps, if the next step was to 'Send to Strategy...' the account was not being sent to the strategy due to 
--			  a clause which needed to check if the dateincurrentstep was between @datenow - a minute and @datenow. When processing 'PrevStrat' the dateincurrentstep is
--			  @datenow - a minute therefore the account was not selected.
-- 04/12/09 - IP/JC - 04/12/09 - UAT(930) - Prevent duplicate insert into SMS.
-- 11/12/09 - IP/JC - UAT938 correct multiple inserts.
-- 27/04/10 - IP - UAT(982)UAT5.2 - Update HoldDays = 'P' once processed, so that it is not processed more than once in the same run.
-- 07/02/12 jec #9521 CR9417 - duplication of existing strategies
-- 13/02/12   IP - Replacing Getdate() with @rundate
-- 10/04/12   IP/JC - #9884 - Account in SCPWO did not enter SCWPW2 worklist
-- 08/08/19	  Zensar(SH)  Optimised the stored procedure for performance by putting Nolock and replcaing * with 1 in all exist checks.
-- 06/08/19   Zensar(SH)   Configure SMS and Letters based on values set in Country Parameter

-- ============================================================================================
create procedure [dbo].[CNEod_UpdateandDoStepsInStrategiesSP]

@rundate DATETIME, @return int OUTPUT
AS

	declare    @Type char(1) ,
		@Condition varchar(12) ,
		@strategy varchar(7),		-- #9521
		@Operand varchar(10), --- between >< = 
		@Operator1 varchar(24), -- value.....
		@Operator2 varchar(24), -- 
		@OrClause char(1),  --  can be a/b/c/1/2/3 - used to match orclauses
		@step smallint,
		@ActionCode varchar(12),
		@StepActiontype char(1),
		@statement sqltext,
		@statement2 sqltext, --IP - 08/10/08 - UAT(525)
		@newline varchar(64),
		@rowcount int			--UAT780
		
		set @newline = ''
	declare @datenow datetime, @nextsteptrue smallint, @nextstepfalse smallint,
		@ConditionType char(1)	-- UAT767 jec
	
	declare @Entrystatement sqltext,@Entrystatement2 sqltext,
		@Stepsstatement sqltext,@Stepsstatement2 sqltext,
		@Exitstatement sqltext,@Exitstatement2 sqltext,
		@StrategyVariabletable varchar(30)
		
	set @return = 0
	set @datenow = @rundate					--IP - 13/02/12

	--IP - 9/10/08 - UAT(525)- Temporary table that will hold accounts and the strategies that they will be inserted into
	--due to hitting an exit condition.
		create table #ExitToStrategy
		(

			Acctno varchar(12), 
			ActionCode varchar(12),
			Sortorder smallint,
			InsertRecord char(1)
		)


		DECLARE StrategyStep_cursor CURSOR 
  		FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,
  			C.StepActiontype ,c.nextsteptrue,c.nextstepfalse,c.savedtype
			from CMStrategyCondition C WITH(NoLock)
			LEFT JOIN CMCondition d  WITH(NoLock) ON c.condition = d.condition 
			JOIN CMStrategy S WITH(NoLock) ON c.strategy =s.strategy
			where S.isActive !=0
			and c.step is not null 
			--and s.strategy !='NON' -- non arrears strategy dealt with later.
			--and s.strategy !='SCNON' -- storecard non arrears strategy dealt with later.
			and s.strategy not in ('NON', 'SCNON','ARB','ARR') --Added by Zensar
   
		OPEN StrategyStep_cursor

		FETCH NEXT FROM StrategyStep_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
           @OrClause,@step,@ActionCode,@StepActiontype ,@nextsteptrue,@nextstepfalse,@ConditionType 
		
		WHILE (@@fetch_status <> -1)
			BEGIN
				IF (@@fetch_status <> -2)
   					begin
   						-- set table for correct table depending on condition type - UAT767	  
						set @StrategyVariabletable=case
						when @ConditionType ='N' then 'CMStrategyVariablesEntry'
						when @ConditionType ='S' then 'CMStrategyVariablesSteps'
						when @ConditionType ='X' then 'CMStrategyVariablesExit'
					end 
					

					-- update account to next step providing condition is true and account is not held
					if @nextsteptrue is not NULL AND @condition !=''
					BEGIN
						set @statement = ' update CMStrategyAcct set currentstep =' + convert(varchar,@nextsteptrue) + ',dateincurrentstep = ' +
                           '''' + convert(varchar,@datenow,109) + ''''
							+ ' from ' + @StrategyVariabletable + ' V where isnull(v.' + @condition + ',''N'')= ''Y'' ' +
							' and V.strategy = CMStrategyAcct.strategy and CMStrategyAcct.dateto is null'  +
							' and V.strategy = ' + '''' + @strategy + '''' + 
							' and CMStrategyAcct.currentstep =' + convert(varchar,@step) +
							' and V.acctno = CMStrategyAcct.acctno ' + @newline +
							' and isnull(HOLDAREXP,'''') !=''Y'' ' + 
							' and isnull(HoldDDMin,'''') !=''Y'' ' +
							' and dateincurrentstep <= ' + '''' + convert(varchar,@datenow,109) + '''' -- IP - 09/09/09 - UAT(834) - Changed to check dateincurrentstep <= @datenow as there may be two steps one after the other that are not actions such as Send to Worklist or Send Letter which would need to be processed in the same run of the procedure.
						
						execute sp_executesql @statement
						-- print @statement 
						set @return = @@error 
						
						if @return !=0
						begin
							print @statement
							return 
						end

					END


					--IP - 23/10/08 - UAT5.2 - UAT(536) - Join missing to check that the 
					if @nextsteptrue is not NULL AND @condition =''
					BEGIN
						set @statement = ' update CMStrategyAcct set currentstep =' + convert(varchar,@nextsteptrue) + ',dateincurrentstep = ' +
                           '''' + convert(varchar,@datenow,109) + '''' + 
						  ' where CMStrategyAcct.dateto is null and CMStrategyAcct.strategy=' +'''' + @strategy + ''''  +
                          ' and CMStrategyAcct.currentstep =' + convert(varchar,@step) +
                          ' and dateincurrentstep < ' + '''' + convert(varchar,@datenow) + '''' -- but don't move step if just has been moved. 
						execute sp_executesql @statement
						--print @statement 

						set @return = @@error 
						if @return !=0
						begin
							print @statement
							return 
						end

					END
					
					-- update account to next step providing condition is false and account is not held
					If @nextstepfalse is not NULL AND @Condition != 'PrevStrat' --IP - 08/09/09
					BEGIN
						set @statement =' update CMStrategyAcct set currentstep =' + convert(varchar,@nextstepfalse) + ',dateincurrentstep = ' +
							'''' + convert(varchar,@datenow,109) + ''''
							+ ' from ' + @StrategyVariabletable + ' V where isnull(v.' + @condition + ','''') != ''Y'' ' +  @newline +
							' and V.strategy = CMStrategyAcct.strategy and CMStrategyAcct.dateto is null' + @newline +
							' and CMStrategyAcct.strategy =' +'''' + @strategy + ''''  + --IP - 24/10/08
							' and V.acctno = CMStrategyAcct.acctno ' +
							' and V.strategy = ' + '''' + @strategy + '''' + 
							' and CMStrategyAcct.currentstep =' + convert(varchar,@step) +  @newline +
							' and isnull(HOLDAREXP,'''') !=''Y'' ' +  @newline +
							' and isnull(HoldDDMin,'''') !=''Y'' '  + @newline +
							' and dateincurrentstep < ' + '''' + convert(varchar,@datenow,109) + ''''   -- but don't move step if just has been moved. 
        
						execute sp_executesql @statement
						--print @statement 
						set @return = @@error 
						if @return !=0
						begin
							--print @statement
							return 
						end

					END
        
					--IP - 08/09/09 - UAT(834) If the account is on a 'PrevStrat' step and the account was NOT
					-- in a previous strategy relating to the strategy in the @ActionCode then update the currentstep to the
					--nextstepfalse. If the account was in the previous strategy then do not update the step, meaning the
					--account should then be sent to this previous strategy further down in this procedure.
					--IP - 09/09/09 - UAT(834) - Set the dateincurrentstep to @dateNow - 1 minute as if the next step is a DayX
					--when checking this step will only update if the dateincurrentstep < @dateNow.
					If @nextstepfalse is not NULL AND @Condition = 'PrevStrat' 
					BEGIN

							set @statement = ' update CMStrategyAcct set currentstep =' + convert(varchar,@nextstepfalse) + @newline
								+' ,dateincurrentstep = ' +  '''' + convert(varchar,dateadd(mi,-1,@datenow),109) + '''' + @newline --IP - 09/09/09 - UAT(834) 
								+' from ' + @StrategyVariabletable + ' V where isnull(v.' + @condition + ','''') = ''Y'' ' +  @newline 
								+' and V.acctno = CMStrategyAcct.acctno ' +
								+' and V.strategy = CMStrategyAcct.strategy ' + @newline 
								+' and CMStrategyAcct.dateto is null ' + @newline
								+' and CMStrategyAcct.strategy =' +'''' + @strategy + ''''  + 
								+' and CMStrategyAcct.currentstep =' + convert(varchar,@step) +  @newline +
								+' and not exists (select 1 from CMStrategyAcct a1 WITH(NoLock) where a1.acctno= CMStrategyAcct.acctno ' + @newline
								+' and a1.strategy = ' + '''' + @ActionCode + '''' + @newline
								+' and a1.datefrom = (select top 1 datefrom from CMStrategyAcct a3  WITH(NoLock) INNER JOIN dbo.CMStrategy s WITH(NoLock) ON a3.strategy=s.Strategy' + @newline -- UAT856 jec 14/09/09
								+' where a3.acctno = CMStrategyAcct.Acctno ' + @newline
								+' and a3.strategy !=CMStrategyAcct.strategy AND s.manual=0 order by datefrom desc)) '
                            		


							execute sp_executesql @statement
       
           
							--IP - 09/09/09 - UAT(834)
							--There may be some instances where an account has exited a strategy and entered another for e.g. 
							--was previously in 'PTP' and exited to 'PPB'. In this instance there may be steps to check 'PrevStrat'
							--first and then a 'DayX' = 0 step may be encountered as a later step and not step 1. As this account was sent to this strategy,
							--the DayX condition would not have been set previously, therefore need to set this to process the 'DayX' = 0 step immediately.
							set @statement2 = ' update CMStrategyVariablesSteps set DayX = ''Y'' ' + @newline
								+' from CMStrategyVariablesSteps cms ' + @newline
								+' where exists(select 1 from cmstrategyacct a WITH(NoLock) inner join cmstrategycondition c WITH(NoLock)' + @newline
								+' on a.strategy = c.strategy ' + @newline
								+' where c.strategy = ' + +'''' + @strategy + ''''  + @newline
								+' and a.dateto is null ' + @newline
								+' and a.dateincurrentstep = ' +  '''' + convert(varchar,dateadd(mi,-1,@datenow),109) + '''' + @newline
								+' and a.currentstep = ' +  convert(varchar,@nextstepfalse) + @newline
								+' and a.currentstep = c.step ' + @newline
								+' and c.step = ' + convert(varchar,@nextstepfalse) + @newline
								+' and c.condition = ''DayX'' ' + @newline
								+' and c.operand = ''='' ' + @newline
								+' and c.operator1 = ''0'' ' + @newline
								+' and c.savedtype = ''S'' ' + @newline
								+' and a.acctno = cms.acctno ' + @newline
								+' and a.strategy = cms.strategy) '
	
				
							execute sp_executesql @statement2		
		
			

							set @return = @@error 
							if @return !=0
							begin
								--print @statement
								return 
							end   

       
						END

        
					-------- 
          
			end
   
			FETCH NEXT FROM StrategyStep_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
				@OrClause,@step,@ActionCode,@StepActiontype,@nextsteptrue,@nextstepfalse,@ConditionType  
		END
		CLOSE StrategyStep_cursor
		DEALLOCATE StrategyStep_cursor
		
		set @return = @@error
		if @return !=0
		begin
			--print 'Error loading strategy steps'
			return 
		end

		--IP - 8/10/08 - UAT(525) - Exit Conditions - Select all the exit conditions for strategies into the 
		DECLARE StratergyExit_cursor CURSOR 
		FOR SELECT C.Strategy,C.Condition,C.ActionCode,c.savedtype
		from CMStrategyCondition C WITH(NoLock)
		LEFT JOIN CMCondition d WITH(NoLock) ON c.condition = d.condition 
		INNER JOIN CMStrategy S WITH(NoLock) ON c.strategy =s.strategy
		WHERE S.isActive !=0
		AND c.savedtype = 'X' --only for 'Exit Conditions'
		--and s.strategy !='NON' -- non arrears strategy dealt with later. 
		--and s.strategy !='SCNON' -- storecard non arrears strategy dealt with later.
		and s.strategy not in ('NON', 'SCNON','ARB','ARR') --Added by Zensar

		OPEN StratergyExit_cursor

		FETCH NEXT FROM StratergyExit_cursor INTO @Strategy,@Condition,@ActionCode,@ConditionType 

		WHILE (@@fetch_status <> -1)
		BEGIN
			-- set table for correct table depending on condition type - UAT767	  
			set @StrategyVariabletable=case
				when @ConditionType ='N' then 'CMStrategyVariablesEntry'
				when @ConditionType ='S' then 'CMStrategyVariablesSteps'
				when @ConditionType ='X' then 'CMStrategyVariablesExit'
			end 
			--Set the 'DateTo' to todays date for the account in the strategy for it to exit the strategy
			--if the exit condition for the account has been hit.
			set @statement = 'update CMStrategyAcct set dateto =' + '''' + convert(varchar,@datenow,109)+ '''' + 
					    ' from ' + @StrategyVariabletable + ' V inner join CMStrategyAcct a WITH(NoLock) on v.acctno = a.acctno' +
						' where v.' + @condition + '= ''Y'' ' +
						 ' and V.strategy = a.strategy and a.dateto is null' +
						 ' and V.strategy = ' + '''' + @strategy + '''' 

			execute sp_executesql @statement
			--print @statement
			--IP - 9/10/08 - Insert the accounts that have hit an exit rule where they have exited their current strategy
			--into a temporary table which will be used later to insert them into the new strategy that the exit
			--rule sends them to.
			set @statement2 = 'insert into #ExitToStrategy ' +
							  'select a.acctno,' + '''' + @ActionCode + '''' + ', 0, ''''' +
							  ' from ' + @StrategyVariabletable + ' V inner join CMStrategyAcct a WITH(NoLock) on v.acctno = a.acctno' +
							  ' where v.'+ @Condition + '= ''Y'' ' +
							  'and V.strategy = a.strategy ' +
							  'and a.dateto=' +
							  '''' + convert(varchar,@datenow,109)+ '''' +
							  ' and V.strategy = ' + '''' + @strategy + '''' 
 
			execute sp_executesql @statement2
			--print @statement
			set @return = @@error 
			if @return !=0
			begin
              print @statement
              return 
			end
		
			FETCH NEXT FROM StratergyExit_cursor INTO @Strategy,@Condition,@ActionCode,@ConditionType 

		END
		CLOSE StratergyExit_cursor
		DEALLOCATE StratergyExit_cursor
		set @return = @@error
		
		if @return !=0
		begin
			return 
		end

		--IP - 13/10/08 - UAT(525)- Select the sortorder from the 'code' table for the strategies and 
		--update the #ExitToStrategy as this will be used later to determine which records to insert
		--into the 'CMStrategyAcct' table if an account has hit more than one strategy (unlikely to occurr).

		update #ExitToStrategy set sortorder =

		(select c.sortorder from code c with(NoLock) where #ExitToStrategy.actioncode = c.code
			and c.category = 'SS1')

			--IP - 13/10/08 - UAT(525)- Update the 'InsertRecord' column on the #ExitToStrategy table.
			--This will determine which records to insert into the 'CMStrategyAcct' table based on
			--the sort order of the strategies.

		update #ExitToStrategy set InsertRecord = 'Y'
			where exists(select e2.acctno, min(e2.sortorder) 
			from #ExitToStrategy e2
			group by e2.acctno
			having min(e2.sortorder) = #ExitToStrategy.sortorder
			and e2.acctno = #ExitToStrategy.acctno
			)


		--IP - 26/08/09 - UAT(816) - Moved update to CMEod_UpdateGeneralConditionsSP
		---- Now we are going to make sure that currentstep is 1 for new accounts where current step is 0.
		--update CMStrategyAcct set currentstep =1 where currentstep = 0 and dateto is null

		-- Now we are going to put an action on the account. Either add a letter or sms or assign to a worklist
		-- to do this we query the stepaction type on the CMStrategycondition - S for SMS, L for Letter, W for worklist
		-- X for exit to another strategy and P for exit to a previous strategy i.e. not ptp	
   
		-- first do letter
		DECLARE @runno smallint
		SELECT @runno =ISNULL(MAX(runno),0) FROM interfacecontrol WITH(NoLock) WHERE interface = 'collections'

	   --Start Configure Sending Letters based on values set in Country Parameter: Dt : 06 Aug 2019 : Zensar(SH)

	   DECLARE @lettersApplicable char(1)
	   select @lettersApplicable = convert(char(1), Value) from countrymaintenance WITH(NoLock) where codename = 'LettersApplicable'

	   if(@lettersApplicable = 'Y')
	   BEGIN	

			declare @datedueletterdays smallint,@dateletterdue datetime
			select @datedueletterdays = convert(smallint,value)  from countrymaintenance WITH(NoLock) where codename ='letterdays'
			set @dateletterdue = dateadd(day,@datedueletterdays,@datenow)   

			--IP - 04/06/09 - Credit Collection Walkthrough Changes - apply individual letter costs to each letter.
			if @return = 0
			BEGIN
				--IP - 04/06/09 - Create temporary table to hold accounts and letters with values
				create table #letters
				(

					acctNo varchar(12),
					letterCode varchar(10),
					letterVal money,
					transrefNo int,
					letterID integer identity 
				)


				declare @hoBranchMaxRefNo int

				INSERT INTO #letters 
				SELECT a.acctno, 
				CASE isnull(CHARINDEX  (' ',d.actioncode) ,0)
				WHEN 0
					then d.actioncode
					else left (d.actioncode, CHARINDEX (' ',d.actioncode) -1)
				END,
				0,
				0


				FROM CMStrategycondition d WITH(NoLock), CMStrategyAcct a WITH(NoLock)
				WHERE d.strategy =a.strategy
				AND a.dateincurrentstep =  @datenow
				AND d.step = a.currentstep
				AND d.stepactiontype ='L' 
				ORDER BY a.acctno 
		
				--Update the value of the letter
				UPDATE #letters
				SET letterVal = (select isnull(c.reference, 0)
									from code c	 with(Nolock)

									where c.code = l.letterCode
									and c.category = 'LTA')
				FROM #letters l

				--Update the transrefno on the #Letters table
				SELECT @hoBranchMaxRefNo = hirefno from branch b, country c
											where b.branchno = c.hobranchno
		
				UPDATE #letters
				SET transrefno = @hoBranchMaxRefNo + letterID

				PRINT 'doing letters for ' + CONVERT(VARCHAR,@rundate)		--IP - 13/02/12
		
				INSERT INTO letter (runno,acctno,dateacctlttr, datedue, addtovalue, excelgen, lettercode)
				SELECT @runno, l.acctNo, @datenow, @dateletterdue, 0, 0, l.letterCode
				FROM #letters l

				INSERT INTO fintrans (origbr, branchno, acctno, transrefno, datetrans, transtypecode, empeeno,
										transupdated, transprinted, transvalue, bankcode, bankacctno, chequeno,
										ftnotes, paymethod, runno, source)
				SELECT 0, substring(l.acctNo, 1, 3), l.acctNo, l.transrefNo, @datenow, 'ADM', -99999,
								'N', 'N', l.letterVal, '', '', '',
								'', 0, 0, 'COSACS'
				FROM #letters l
				WHERE l.letterVal > 0
		
				--Update the outstanding balance 
				UPDATE acct
				SET outstbal = (select sum(transvalue) from fintrans f, #letters l2
								 where f.acctno = l2.acctno 
								 and l2.letterVal > 0
								 and l2.acctno = a.acctno)
				FROM acct a WITH(NoLock)











				inner join #letters l1
				on a.acctno = l1.acctno
				and l1.letterVal > 0

				--Update the hirefno on the branch table to the correct value
				UPDATE branch 
				SET hirefno = ISNULL((select max(transrefNo) from #letters),hirefno )
				FROM country c WITH(NoLock)
				INNER JOIN branch b WITH(NoLock)
				ON b.branchno = c.hobranchno

				SET @return = @@error
				IF @return !=0
				BEGIN
					PRINT 'Error generating letters'
					return
				END
	
			END






		END

		--End Configure Sending Letters based on values set in Country Parameter : Zensar(SH)

		 --Start Configure SMS Sending based on values set in Country Parameter : Zensar(SH)



		DECLARE @SMSApplicable char(1)
		select @SMSApplicable = convert(char(1), Value) from countrymaintenance with(NoLock) where codename = 'SMSApplicable'

		if(@SMSApplicable = 'Y')
		BEGIN	






















			if @return = 0
			begin
				 insert into sms (acctno,dateadded,code) 
				 select a.acctno, @datenow ,d.actioncode
				  from CMStrategycondition d WITH(NoLock), CMStrategyAcct a WITH(NoLock), custacct cu WITH(NoLock), custtel ct WITH(NoLock)
				  where 
				  d.strategy =a.strategy and a.dateincurrentstep = @datenow --IP - 20/08/09 - UAT(799) - should join on dateincurrentstep
				  and d.step = a.currentstep and d.stepactiontype ='S' 
				  and cu.acctno= a.acctno and cu.hldorjnt ='H' and 
				  ct.custid = cu.custid and ct.tellocn = 'M' 
				  AND ct.datediscon IS NULL --IP/JC - 04/12/09 - UAT(930)
				  AND NOT EXISTS (SELECT 1 FROM sms s WHERE s.acctno = a.acctno AND s.code = d.ActionCode
				  AND s.dateadded = @datenow)
				  GROUP BY a.acctno, d.ActionCode
			 
				 set @return = @@error
			 
				 if @return !=0
					begin
					   print 'Error generating sms'
					   return 
					end
			 end
		END

		 --End Configure SMS Sending based on values set in Country Parameter : Zensar(SH)

















		if @return = 0
		begin  
		   -- now update worklist -- set dateto first where worklist changing or strategy exiting then insert
		   update CMWorklistsAcct set dateto = @datenow from 







		   CMStrategycondition d WITH(NoLock), CMStrategyAcct A WITH(NoLock)
		   where a.acctno = CMWorklistsAcct.acctno and CMWorklistsAcct.dateto is null
		   and d.strategy = a.strategy and a.dateincurrentstep =@datenow --IP - 08/10/08 - Changed to 'dateincurrentstep' from 'datefrom'  
		   and d.step =a.currentstep and d.stepactiontype in('X','W')-- x for exit w for worklist 




		   and CMWorklistsAcct.worklist !=d.actioncode
		   set @return = @@error
		   if @return !=0
		   begin
			  print 'Error updating CMWorklistsAcct'
			  return 
		   end
		end

		if @return = 0
		begin -- insert new worklists if actiontype worklist - but only if don't exist arleady in there
		   insert into CMWorklistsAcct ( acctno,strategy,datefrom,dateto,worklist)
		   select Distinct a.acctno,a.strategy,@datenow,NULL,
			 CASE isnull(CHARINDEX  (' ',d.actioncode) ,0) 
			 WHEN 0 
				THEN D.ACTIONCODE
			 ELSE -- this is being saved with the description so need to extract it to write the letter
				LEFT(d.actioncode, CHARINDEX (' ',d.actioncode) -1)
			 END
		   from CMStrategycondition d WITH(NoLock), CMStrategyAcct A WITH(NoLock)
		   where   d.strategy =a.strategy and a.dateto is null
		   and d.step =a.currentstep and d.stepactiontype ='W' --worklist
		   and not exists (select 1 from CMWorklistsAcct W  WITH(NoLock) where w.acctno =a.acctno and w.worklist = d.actioncode and w.dateto is null)
		   and not exists (select 1 from CMWorklistsAcct W  WITH(NoLock)
		   where w.acctno =a.acctno and w.worklist = LEFT(d.actioncode, CHARINDEX (' ',d.actioncode) -1) and w.dateto is null)







		   set @return = @@error
		   if @return !=0
		   begin
			  print 'Error adding worklist'
			  return 
		   end
		end
		--Charudatt
		--IP - 9/10/08 - UAT(525) - Update cmworklistsacct so that the account is no longer in the worklist if it has
		--exited the strategy after hitting an exit condition.
		if @return = 0
		begin
			update CMWorklistsAcct set dateto = @datenow from
			CMStrategyAcct a
			where a.acctno = CMWorklistsAcct.acctno and CMWorklistsAcct.dateto is null
			and a.strategy = CMWorklistsAcct.strategy 
			and a.dateto = @datenow --where the account has just exited the strategy.
		
			set @return = @@error
			if @return !=0
			begin
				print 'Error exiting the worklist'
				return 
			end
		end


		--IP - 08/09/09 - UAT(834) - Incorrect strategy (current strategy) was being selected, and join was missing.
		--IP - 09/09/09 - Changed where clause to check a.dateincurrentstep = dateadd(mi, -1,@datenow) as when processing 'PrevStrat'
		--step, dateincurrentstep is added as dateadd(mi, -1,@datenow).
		create table #previousstrategy (acctno char(12), previousstrategy varchar(7))		-- #9521
    
		if @return = 0
		begin --now check if there is a previous strategy to go back to if PTP broken
		   insert into #previousstrategy (acctno,previousstrategy)
		   select a.acctno, d.ActionCode --IP - 08/09/09 - UAT(834)
		   from CMStrategyAcct A WITH(NoLock),CMStrategycondition D WITH(NoLock)
			where  d.strategy =a.strategy and a.dateincurrentstep between dateadd(mi, -1,@datenow) AND @datenow and a.dateto is null --IP - 09/09/09 - UAT(834)
		   and d.step =a.currentstep and d.stepactiontype ='P'
		   -- check whether the previous strategy was a previous strategy to go backto                                               
		   and exists (select 1 from CMStrategyAcct PS with(Nolock)  where PS.acctno= a.acctno
		   and PS.strategy = d.actioncode and PS.datefrom =
		   (select top 1 datefrom from CMStrategyAcct P WITH(NoLock) INNER JOIN CMStrategy s WITH(NoLock) ON p.strategy=s.Strategy -- UAT856 jec 14/09/09
			where p.acctno = a.acctno and p.strategy !=a.strategy AND s.manual=0 order by datefrom desc)) -- IP - 08/09/09 - UAT(834)

		   update a set dateto = @datenow
		   from CMStrategycondition d, CMStrategyAcct A 
			where  d.strategy =a.strategy and a.dateincurrentstep BETWEEN dateadd(mi, -1,@datenow) AND @datenow  and a.dateto is null --IP - 09/09/09 - UAT(834)
		   and d.step =a.currentstep and d.stepactiontype ='P'
		   -- check whether the previous strategy was a previous strategy to go backto                                               
		   and exists (select 1 from CMStrategyAcct PS WITH(NoLock) where PS.acctno= a.acctno
		   and PS.strategy = d.actioncode and PS.datefrom =
		   (select top 1 datefrom from CMStrategyAcct P WITH(NoLock) INNER JOIN CMStrategy s WITH(NoLock) ON p.strategy=s.Strategy		-- UAT856 jec 14/09/09
		   where p.acctno = a.acctno and p.strategy !=a.strategy AND s.manual=0 order by datefrom desc)) --IP - 08/09/09 - UAT(834)
       
		   if exists (select * from #previousstrategy)
			begin
				insert into CMStrategyAcct 
				(acctno,strategy,datefrom,dateto,currentstep,dateincurrentstep)
				select 
				acctno,previousstrategy,@datenow,null,1,@datenow --IP - 05/10/09 - UAT(903) - Changed inserting step 0, to 1.
				from #previousstrategy
           end



       
			set @return = @@error
			
			if @return !=0
			begin
				print 'Error inserting previous strategy'
				return 
			 end
       
       
		end
	
		if @return = 0
		begin
			-- now for exits to another strategy - this sets the dateto for current strategy
			update CMStrategyAcct set dateto=@datenow		-- jec 10/08/09 UAT780 strategy = d.actioncode
			from CMStrategycondition d WITH(NoLock), CMStrategyAcct A WITH(NoLock)
			where   d.strategy =a.strategy and a.dateincurrentstep BETWEEN dateadd(mi, -1,@datenow) AND @datenow --IP - 9/10/08 - changed to 'dateincurrentstep' from 'datefrom'. --IP - 11/11/09 - UAT(864)
			and d.step =a.currentstep and d.stepactiontype ='X'
			and a.dateto is NULL			-- jec/IP UAT938 11/12/09
			
			set @rowcount=@@ROWCOUNT
			set @return = @@error
			
			if @return !=0
			begin
				 print 'Error exiting to another strategy'
				return 
			end
			
			if @rowcount>0
			begin
				-- insert into new strategy from current step
				insert into CMStrategyAcct
				select a.acctno,c.ActionCode,a.dateto,null,1,a.dateto
				from CMStrategyAcct a WITH(NoLock) inner join CMStrategyCondition c WITH(NoLock) on a.strategy=c.strategy and a.currentstep=c.Step
				where a.dateto=@datenow and c.StepActiontype='X'
	       
				--IP - 08/09/09 - UAT(834) - If an account hits a step that sends the account 
				--to another strategy, and the first step in that strategy happens to be a check on 
				--'PrevStrat', then we need to update the 'PrevStrat' condition so that this step is processed the next time
				--this procedure is called.
				update CMStrategyVariablessteps set PrevStrat = 'Y'
				from CMStrategyVariablessteps cms
				where exists (select 1 from CMStrategyAcct a WITH(NoLock)
							inner join CMStrategyCondition c WITH(NoLock)
							on a.strategy = c.Strategy
							where a.datefrom = @datenow
							and a.currentstep = c.step
							and c.step = 1
							and c.condition = 'PrevStrat'
							and a.strategy = cms.strategy
							and a.acctno = cms.acctno)
		   end
       
		end
    
		--IP -9/10/08- UAT(525) - When an exit condition has been hit and the exit condition
		--sends the account to another strategy, we need to insert the 
		--account into the new strategy
		--select the distinct account as #ExitToStrategy may have duplicates for accounts
		--if an exit condition sends the account to the same strategy.

		if @return = 0
		begin			
        
			--update where exit strategy already inserted from exit condition
			UPDATE e
			SET e.InsertRecord = ''
			FROM #ExitToStrategy e
			WHERE EXISTS (SELECT 'a' FROM CMStrategyAcct WITH(NoLock)
						  WHERE acctno = e.Acctno
							AND strategy = e.ActionCode
							AND datefrom = @datenow
							AND dateto IS NULL)
				AND e.InsertRecord = 'Y'
        

			insert into CMStrategyAcct
				(acctno,strategy,datefrom,dateto,currentstep,dateincurrentstep)
			select distinct e.acctno, e.actioncode, @datenow, null, 0, @datenow
			from #ExitToStrategy e
			where e.insertrecord = 'Y'  --IP - 13/10/08 - UAT(525) 
			

		   set @return = @@error
		   if @return !=0
		   begin
			  print 'Error inserting into new strategy'
			  return 
		   end
		end

		--Ilyas Changes------------------------------------------
 
	   --IP - 23/10/08 - UAT(536)- now need to update the CMStrategyAcct.Currentstep to the
	   --CMStrategyCondition.Nextsteptrue if the CMStrategyacct.currentstep = CMStrategyConditionStep
	   --and the CMStrategyCondition.StepActionType != null
	   DECLARE StrategyStep_cursor CURSOR 
	   FOR SELECT C.Strategy,C.Condition,C.Operand,C.Operator1,C.Operator2,C.OrClause,C.step,C.ActionCode,
				C.StepActiontype ,c.nextsteptrue,c.nextstepfalse,c.savedtype
	   from CMStrategyCondition C WITH(NoLock)
	   LEFT JOIN CMCondition d WITH(NoLock) ON c.condition = d.condition 
	   JOIN CMStrategy S WITH(NoLock) ON c.strategy =s.strategy
	   where S.isActive !=0 
	   and c.step is not null 
	   --and s.strategy !='NON' -- non arrears strategy dealt with later. 
	   --and s.strategy !='SCNON' -- storecard non arrears strategy dealt with later. 
	   and s.strategy not in ('NON', 'SCNON','ARB','ARR') --Added by Zensar
	   OPEN StrategyStep_cursor

		FETCH NEXT FROM StrategyStep_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
           @OrClause,@step,@ActionCode,@StepActiontype ,@nextsteptrue,@nextstepfalse,@ConditionType 
		WHILE (@@fetch_status <> -1)
		BEGIN
			
			
			-----commented by Zensar on 31/08/2018----------------start
			-- set table for correct table depending on condition type - UAT767	  
			--set @StrategyVariabletable=case
			--when @ConditionType ='N' then 'CMStrategyVariablesEntry'
			--when @ConditionType ='S' then 'CMStrategyVariablesSteps'
			--when @ConditionType ='X' then 'CMStrategyVariablesExit'
			--end 
			-----commented by Zensar on 31/08/2018----------------end


			--IP - 24/10/08 - UAT(536) - Needed to convert the 'dateincurrentstep' to a varchar
			--in order to compare this to @datenow, otherwise was not finding a match and 
			--updating the step correctly to the next step.
        
			if @nextsteptrue is not NULL AND @StepActiontype is not NULL AND @StepActiontype != 'X'
			BEGIN
				set @statement = ' update CMStrategyAcct set currentstep =' + convert(varchar,@nextsteptrue) + ',dateincurrentstep = ' +
                           '''' + convert(varchar,@datenow,109) + '''' + 
						  ' where CMStrategyAcct.dateto is null and CMStrategyAcct.strategy=' +'''' + @strategy + ''''  +
                          ' and CMStrategyAcct.currentstep =' + convert(varchar,@step) +
                          ' and convert(varchar,dateincurrentstep) = ' + '''' + convert(varchar,@datenow) + ''''
				execute sp_executesql @statement
				--print @statement 

				set @return = @@error 
				if @return !=0
				begin
					print @statement
					return 
				end  

			END
   
			FETCH NEXT FROM StrategyStep_cursor INTO @Strategy,@Condition,@Operand,@Operator1,@Operator2,
			@OrClause,@step,@ActionCode,@StepActiontype,@nextsteptrue,@nextstepfalse,@ConditionType  

		END

		CLOSE StrategyStep_cursor
		DEALLOCATE StrategyStep_cursor
  
		set @return = @@error
		if @return !=0
		begin
			--print 'Error loading strategy steps'
			return 
		end

		--IP/JC - 10/04/12 - #9884 
		if @return = 0
		begin -- insert new worklists if actiontype worklist - but only if don't exist arleady in there
			insert into CMWorklistsAcct ( acctno,strategy,datefrom,dateto,worklist)
			select Distinct a.acctno,a.strategy,@datenow,NULL,
			CASE isnull(CHARINDEX  (' ',d.actioncode) ,0) 
			WHEN 0 
				THEN D.ACTIONCODE
			ELSE -- this is being saved with the description so need to extract it to write the letter
				LEFT(d.actioncode, CHARINDEX (' ',d.actioncode) -1)
			END
			from CMStrategycondition d WITH(NoLock), CMStrategyAcct A WITH(NoLock)
			where d.strategy =a.strategy and a.dateto is null
			and d.step =a.currentstep and d.stepactiontype ='W' --worklist
	   		and not exists (select 1 from CMWorklistsAcct W WITH(NoLock) where w.acctno =a.acctno and w.worklist = d.actioncode and w.dateto is null)
			and not exists (select 1 from CMWorklistsAcct W WITH(NoLock) 
			where w.acctno =a.acctno and w.worklist = LEFT(d.actioncode, CHARINDEX (' ',d.actioncode) -1) and w.dateto is null)

			set @return = @@error
			if @return !=0
			begin
				print 'Error adding worklist'
				return 
			end
		end


		--Ilyas Changes-------------------------------------
		--IP - 05/10/09 - UAT(903) - The code below has been moved to here from 
		--above where an account is inserted into a new strategy from a step. Moved to 
		--here as this can then be applied to accounts that have recently been inserted into a strategy either from a step
		--or from hitting a previous strategy step.
		--If the first step in the new strategy is a DayX = 0 step then this must be processed. This code also applies to UAT(813)








		update CMStrategyVariablessteps set dayx = 'Y'
		from CMStrategyVariablessteps cms WITH(NoLock)
		where exists(select 1 from CMStrategyAcct a WITH(NoLock)
		inner join CMStrategyCondition c WITH(NoLock)
			on a.strategy = c.Strategy
			where a.datefrom = @datenow
			and a.currentstep = c.step
			and c.step = 1
			and c.condition = 'DayX'
			and c.savedtype = 'S'
			and c.operand = '='
			and c.operator1 ='0'
			and a.strategy = cms.strategy
			and a.acctno = cms.acctno)
	
		--IP - 25/08/09 - UAT(813) - Do not update the DayX to 'P' if the account has just entered a new strategy
		--where the first step is a DayX = 0 condition as this step will still need to be processed.
		--IP - 09/09/09 - UAT(834) - commented out line as DayX = 0 may not always be the first step, for e.g. as in the 'PPB' strategy. 
		--Changed where clause to include check where a.dateincurrentstep = dateadd(mi, -1,@datenow) as when processing 'PrevStrat' condition
		--dateincurrentstep is set as dateadd(mi, -1,@datenow).
			Update CMStrategyVariablesSteps
			set DayX='P' 
			from CMStrategyVariablesSteps cms WITH(NoLock)
			where ISNULL(DayX,'N')='Y'
			and not exists(select 1 from CMStrategyCondition c WITH(NoLock)
							inner join CMStrategyAcct a WITH(NoLock)
							on a.strategy = c.Strategy
							where (a.datefrom = @datenow or a.dateincurrentstep = dateadd(mi, -1,@datenow))
							and a.currentstep = c.step
							and c.condition = 'DayX'
							and c.savedtype = 'S'
							and c.operand = '='
							and c.operator1 ='0'
							and a.strategy = cms.strategy
							and a.acctno = cms.acctno)
						
			--IP - 27/04/10 - UAT(982)UAT5.2 - Update HoldDays = P so that this condition is not processed again when the 
			--procedure is called more than once within the same run. 
			UPDATE CMStrategyVariablesSteps
			SET HoldDays = 'P'
			WHERE ISNULL(HoldDays,'N')='Y'
			
GO

