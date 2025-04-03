SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetStrategyActionsForEmployeeSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetStrategyActionsForEmployeeSP]
GO
-- ============================================================================================
-- Author:		Ilyas Parker & Nasmi Mohamed
-- Create date: 23/12/2008
-- Description:	The procedure will return all the actions for an employee for a strategy
--			    of the selected account.		
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_GetStrategyActionsForEmployeeSP] 
	            @empeeno int,
				@strategy varchar(10),
				@checkForSupervisorRight bit,
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;
	
	set @return = 0
	
	--NM & IP - 29/12/08 - Need to determine if the employee logged in has the user right 
	--'Telephone Action - View Multiple Accounts'. From this we assume that this is a supervisor.

	--declare @taskid int, @empeetype varchar(3)


	--set @empeetype = (select empeetype from courtsperson where empeeno = @empeeno)
	--set @taskid = (select taskid from task where taskname = 'Telephone Action - View Multiple Accounts')

	create table #ActionRights
	(
		ActionDescription varchar(64),
		ActionCode varchar(10),
		CycleToNextFlag bit,
		MinNotesLength int,
		IsWorklist bit --IP - CR976 - 06/01/09 - Need to check if the action is a worklist
	)
	
	--NM & IP - 24/12/2008 - If this is a Supervisor then we need to return all 
	--the actions that the employee has rights to as they should be able to 
	--apply these actions to any account in their worklist and not restricted by strategy.
	if @checkForSupervisorRight = 1 AND  Admin.CheckPermission(@empeeno,326) = 1 
	
	begin
		insert into #ActionRights
		select distinct '', ar.action,MAX(cast (ar.cycletonextflag as INT)), MAX(ar.minnoteslength), ''
		from cmactionrights ar 
		where ar.empeeno = @empeeno
		group by ar.action
	end
	else
	--If this is not a supervisor then return the actions that they are able to perform
	--against the strategy of the selected account and actions that they can perform
	--against all strategies.
	begin
		insert into #ActionRights
		select distinct '', ar.action, MAX(cast (ar.cycletonextflag as INT)), MAX(ar.minnoteslength), ''
		from cmactionrights ar left join cmstrategyactions sa
		on (ar.strategy = sa.strategy  )
		and ar.action = sa.actioncode
		where ar.empeeno = @empeeno
		and (ar.strategy = @strategy or ar.strategy = '')  
		group by ar.action
	end
    -- cycle to next flag now simply determined by code setup....	
	UPDATE #ActionRights 
	SET CycleToNextFlag = 1 
	FROM code c 
	WHERE  c.category = 'FUA' AND c.additional IN ('1','Y')
	AND c.code = #ActionRights.ActionCode
	
	--UPDATE code SET additional = 'Y' WHERE category = 'FUA'
	--IP - CR976 - 06/01/09 - Need to update the 'IsWorklist' flag to (1)
	--if the 'action' in the 'cmactionrights' table is a worklist.
	update #ActionRights set #ActionRights.IsWorklist = 1
	where exists (select * from #ActionRights ar2
					inner join cmworklist w
					on ar2.ActionCode = w.worklist
					and ar2.ActionCode = #ActionRights.ActionCode)

	--Now need to update #ActionRights 'ActionDescription' column if this is null
	--IP - CR976 - 06/01/09 - Also selecting 'IsWorklist' as need to check if the action
	--is a worklist. If so need to also update the 'ActionDescription' for the worklist.
	declare action_cursor cursor for  
			select ActionDescription, ActionCode, IsWorklist
			from #ActionRights
			for update of ActionDescription

	open action_cursor
	declare @ActionDescription varchar(64), @ActionCode varchar(10), @IsWorklist bit
	fetch next from action_cursor into @ActionDescription, @ActionCode, @IsWorklist

	while @@fetch_status = 0 
	begin
		
		--If the actioncode is not a worklist then update the 'ActionDescription'
		--from the 'cmstrategyactions' table
		if (@IsWorklist = 0)
		begin
			update #ActionRights
			set ActionDescription = @ActionCode + ' ' + (select top 1 code.codedescript from code
														 where code.category = 'FUA' and
														  code.code = @ActionCode)

			where current of action_cursor

			fetch next from action_cursor into  @ActionDescription, @ActionCode, @IsWorklist
		end
		else
		--If the actioncode is a worklist then update the 'ActionDescription'
		--from the 'cmworklist' table.
		begin
			update #ActionRights
			set ActionDescription = @ActionCode + ' ' + (select w.description
													from cmworklist w
													where w.worklist = @ActionCode)
			where current of action_cursor

			fetch next from action_cursor into  @ActionDescription, @ActionCode, @IsWorklist

		end

	end

	close action_cursor
	deallocate action_cursor
	
	--NM & IP - 29/12/08 If more than one record has been returned for an
	--action due to the user being able to perform this action against a strategy 
	--or against all strategies and the cycletonextflag is different for both then 
	--select the action where cycletonextflag = 1.

--	delete from #ActionRights
--	where #ActionRights.actioncode in(select ar1.actioncode from #ActionRights ar1 where ar1.cycletonextflag = 1)
--    and #ActionRights.cycletonextflag = 0


	--NM & IP - 06/01/09 - CR976 -
	--Need to find the worklists that are associated with the 'Strategy' passed into
	--this procedure.
	--Then need to delete the actions from the '#ActionRights' which are the same 
	--as the worklists for the strategy as we do not want the user to be able to 
	--send an account to any worklists that the account is currently in.
	
	--Select the worklists for the strategy into a temporary table.
	select substring(cmsc.actioncode, 0, charindex(' ', cmsc.actioncode)) as worklist
	into #tmpworklist
	from cmstrategycondition cmsc 
	where cmsc.strategy = @strategy
	and cmsc.condition = ''
	and	  cmsc.stepactiontype = 'w'
	
	--Delete from the '#ActionRights' table any actions that match the 
	--worklists for the strategy passed into this procedure as we do not want to 
	--display these in the 'Action' drop down on the 'Telephone Actions' screen.
	delete from #ActionRights
	where exists(select * from #tmpworklist 
				 where #tmpworklist.worklist = #ActionRights.ActionCode)

	select * from #ActionRights
							

    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO
