
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CMStrategyAcctUpdateSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CMStrategyAcctUpdateSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 05/06/2007
-- Description:	Updates CMStrategyAcct table setting a new strategy for the current strategy of the selected account where the dateto is null
-- =============================================
CREATE PROCEDURE CMStrategyAcctUpdateSP
	@acctno varchar(12),
    @sendToStrategy VARCHAR(6), --This is the strategy selected from the 'Send To Strategy' drop down on Telephone Actions screen.
	@sendToWorklist varchar(10), --NM & IP - 06/01/09 - CR976 - If the actioncode selected on the 'Telephone Action' screen is a worklist then manually send to this worklist.
	@empeeno int, --NM & IP - 06/01/09 - CR976 - Extra Telephone Actions - Send To WriteOff
	@reasonForWriteOff varchar(12),--NM & IP - 06/01/09 - CR976 - Extra Telephone Actions - Send To WriteOff
	@return int OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0			--initialise return code

	--IP - 08/06/09 - Credit Collection Walkthrough Changes
	--If the account is being sent to a different worklist
	DECLARE @supervisorWorklist bit

	--IP - 08/06/09 - Credit Collection Walkthrough Changes
	--If the account is being sent to a supervisor worklist or a worklist not linked to a strategy
	--then we do not want the account to leave the strategy that it is currently in.
	 SELECT			w.worklist,
					w.description,
					ISNULL(s.Strategy,'Sup') AS Strategy,
					ISNULL(S.Description,'Supervisor Worklist') AS WorklistDesc
	INTO #worklists
    FROM CMWorkList w
    LEFT JOIN CMStrategyCondition c 
    ON LEFT(c.ActionCode,CHARINDEX (' ',c.ActionCode)-1) = w.WorkList
	AND c.StepActiontype ='W'
	LEFT JOIN CMStrategy s ON S.Strategy = C.Strategy
    GROUP BY s.strategy,w.worklist,w.description,s.description
    order by s.strategy,w.worklist

	--Need to remove the worklist entry for 'SUP' in the instance where it appears as
	--a supervisor worklist and a worklist attached to a strategy. If this is the case then
	--the worklist cannot be a supervisor worklist.

	delete from #worklists 
	where exists(select * from #worklists w1, cmstrategy s1, cmworklist cmw1
				where w1.strategy = s1.strategy
				and w1.worklist = cmw1.worklist
				and w1.worklist = #worklists.worklist
			    and exists(select * from #worklists w2
							 where w2.worklist = w1.worklist
							 and w2.strategy = 'SUP'))
	and #worklists.strategy = 'SUP' 	

	IF EXISTS(select * from #worklists w
				where w.worklist = @sendToWorklist
				and w.strategy = 'SUP'
				and w.WorklistDesc = 'Supervisor Worklist')
	BEGIN
		SET @supervisorWorklist = 1
	END
	ELSE
	BEGIN
		SET @supervisorWorklist = 0
	END
	
	
	IF(@supervisorWorklist = 0)
	BEGIN
		UPDATE CMStrategyAcct
		SET dateto = GETDATE()  
		WHERE acctno =  @acctno AND dateto IS NULL
	END

	--IP - 21/10/08  UAT5.2 - UAT(551) - Also need to update the 'CMWorklistsacct' table
	--for the account so that it is no longer in its existing worklist.
	UPDATE CMWorklistsacct
	SET dateto = GETDATE()
	WHERE acctno = @acctno and dateto IS NULL

	----------------------------------------------------------------------------------------------
	
	--IP - 13/11/08 - UAT(548) - If an account is sent to a different strategy
	--then need to check the steps for the new strategy.
	--Need to cater for when the steps says if 'DAY = 0' and step = 1 and action is
	--to send to a worklist, and also in the instance when the 'DAYX' condition is not the first step
	--and the first step is the action to send to the new worklist.
	declare @worklist varchar(6)
	declare @nextStep int
	set @worklist = ''
	
	IF (@sendToStrategy != '')
	BEGIN

		insert into CMStrategyAcct(acctno, Strategy,datefrom,dateto,currentstep, dateincurrentstep)
		values (@acctno,@sendToStrategy,GETDATE(),NULL,0, GETDATE())
		--NM & IP - 06/01/09 - CR976 - If the action code 'STW - Send To Writeoff' has been
		--selected from the 'Telephone Actions screen then insert a record to the 'CMStrategyAcct'
		--table for the WOF (Writeoff) strategy, and an entry in the 'BDWPending' table so that 
		--the account appears in the 'WriteOff Review' screen.
		IF(@sendToStrategy = 'WOF')
		BEGIN
			insert into BDWPending (acctno, empeeno, code, runno, empeenomanual)
			values (@acctno, 0, @reasonForWriteOff, 0, @empeeno)
		END
		ELSE --IF(@sendToStrategy = 'WOF')
		BEGIN
			--If the first step is on 'Day = 0' and second step is to send to a worklist
			--then need to select the worklist that the account needs to go to.
			IF EXISTS(
			select * from cmstrategycondition c1
			where c1.savedtype = 'S'
			and c1.step = 1
			and c1.condition = 'DAYX' 
			and c1.operand = '='
			and c1.operator1 = '0'
			and c1.nextsteptrue = 2
			and exists(select * from cmstrategycondition c2
					 where c2.strategy = c1.strategy 
					 and c2.step = 2
					 and c2.stepactiontype = 'W')
			and c1.strategy = @sendToStrategy)
			BEGIN
				--Then need to select the worklist from step '2' that the account 
				--will need to go to 
				select @worklist = substring(c1.actioncode, 0, charindex(' ', c1.actioncode))from cmstrategycondition c1
									where c1.step = 2
									and c1.stepactiontype = 'W'
									and c1.condition = ''
									and c1.strategy = @sendToStrategy
				
				--Get the next step that we want to update the currentstep to be on 
				--the 'CMStrategyacct' table.
				select @nextStep = nextsteptrue from cmstrategycondition c1
									where c1.step = 2
									and c1.strategy = @sendToStrategy

				insert into cmworklistsacct(acctno, strategy, worklist, datefrom, dateto)
				values (@acctno, @sendToStrategy, @worklist, getdate(), null)

				--Update the 'Currentstep' on the 'CMStrategyacct' table to the next step
				--for the account.
				update cmstrategyacct set currentstep = @nextStep
				where acctno = @acctno
				and strategy = @sendToStrategy
				and dateto is null

			END
			--Else if the first step of the strategy is to send to a worklist
			--select the worklist from step 1.
			ELSE IF EXISTS (select * from cmstrategycondition c1
								where c1.savedtype = 'S'
								and c1.step = 1 
								and c1.stepactiontype = 'W'
								and c1.condition = ''
								and c1.strategy = @sendToStrategy)
				BEGIN
					
					select @worklist = substring(c1.actioncode, 0, charindex(' ', c1.actioncode))from cmstrategycondition c1
							where c1.savedtype = 'S'
									and c1.step = 1 
									and c1.stepactiontype = 'W'
									and c1.condition = ''
									and c1.strategy = @sendToStrategy

					--Get the next step that we want to update the currentstep to be on 
					--the 'CMStrategyacct' table.
					select @nextStep =  nextsteptrue from cmstrategycondition c1
										where c1.step = 1
										and c1.strategy = @sendToStrategy

					insert into cmworklistsacct(acctno, strategy, worklist, datefrom, dateto)
					values (@acctno, @sendToStrategy, @worklist, getdate(), null)

					--Update the 'Currentstep' on the 'CMStrategyacct' table to the next step
					--for the account.
					update cmstrategyacct set currentstep = @nextStep
					where acctno = @acctno
					and strategy = @sendToStrategy
					and dateto is null

				END
		END
	END
	--IP - 06/01/09 - CR976 - Else we are manually sending the account to the worklist selected as an 
	--action on the 'Telephone Actions' screen.
	ELSE	-- IF (@sendToStrategy != '')
	BEGIN
		
		declare @worklistStrategy varchar(6) -- NM & IP - 06/01/09 - CR976 - This will be set to
										 -- the strategy for the selected worklist to send to
										 -- if the worklist is linked to a strategy.
		
		--IP - 09/06/09 - Credit Collection Walkthrough Changes
		--Only select the strategy where it exists.
		select @worklistStrategy =  c.strategy from cmstrategycondition c
								 inner join cmstrategy cms
								 on c.strategy = cms.strategy
								 where c.stepactiontype = 'W' 
								 and substring(c.actioncode, 0,charindex(' ', c.actioncode)) = @sendToWorklist
		
		--If the worklist is one that is linked to a strategy then we need to 
		--insert a record into the 'CMStrategyAcct' table for this account
		--and also insert a record into the 'CMWorklistsAcct' table.
		--We then need to update the 'Currentstep' on the 'CMStrategyAcct' table
		--to the next step for the account.
		
		if(@worklistStrategy != '')
		BEGIN
			insert into CMStrategyAcct(acctno, Strategy,datefrom,dateto,currentstep, dateincurrentstep)
			values (@acctno,@worklistStrategy,GETDATE(),NULL,0, GETDATE())
			
			select @nextStep = nextsteptrue
							from cmstrategycondition c
							where c.strategy = @worklistStrategy
							and c.stepactiontype = 'W'
							and c.savedtype = 'S'
							and substring (c.actioncode, 0, charindex(' ', c.actioncode)) = @sendToWorklist
		
			update CMStrategyAcct set currentstep = @nextStep
			where acctno = @acctno
			and strategy = @worklistStrategy
			and dateto is null
			
			insert into cmworklistsacct(acctno, strategy, worklist, datefrom, dateto)
			values (@acctno, @worklistStrategy, @sendToWorklist, getdate(), null)
			
		END
		--Else we just need to insert a record into the 'CMWorklistsAcct' table
		--as the worklist is not linked to any strategy.
		ELSE --if(@worklistStrategy != '')
		BEGIN
			insert into cmworklistsacct(acctno, strategy, worklist, datefrom, dateto)
			values (@acctno, isnull(@worklistStrategy, ''), @sendToWorklist, getdate(), null)
		END

	END
    SET	@return = @@error
END
GO
