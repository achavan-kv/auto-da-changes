SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_GetStrategyWorklistActionsSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_GetStrategyWorklistActionsSP]
GO
-- ============================================================================================
-- Author:		Ilyas Parker
-- Create date: 25/09/2008
-- Description:	The procedure will return all the actions for worklists allocated
--			    to the employee type.			
-- ============================================================================================
CREATE PROCEDURE [dbo].[CM_GetStrategyWorklistActionsSP] 
	            @empeeno int,
				@return	int	OUTPUT
as
begin
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	set nocount on;

	declare @type varchar(3) --Store the employee type

    set @return = 0    --initialise return code

	--Temporary table to store the strategies and the worklists associated with the strategy
	create table #StratWorklists
	(
		strategy varchar(50),
		worklist varchar(4)	
	)


	--Select strategies and worklists associated with each strategy
	insert into #StratWorklists
	select cms.strategy,
		   substring(cmsc.actioncode, 0, 5)
	from cmstrategy cms inner join cmstrategycondition cmsc on
		 cms.strategy = cmsc.strategy
	where cmsc.condition = ''
	and	  cmsc.stepactiontype = 'w'
	order by cmsc.strategy

	--Delete the strategies and worklists from the table if there are not any worklists
	--assigned to the employee type for the employee passed in.
	delete from #StratWorklists 
	where not exists(select * from cmworklist cmw
					 INNER JOIN Admin.UserRole ur ON ur.RoleId = cmw.EmpeeType
					 where cmw.worklist = #StratWorklists.worklist
					 AND ur.UserId = @empeeno)


	--select the actions for the worklists assigned to the employee type	
	select sw.strategy as 'Strategy', 
		   wa.worklist as 'WorkList', 
		   wa.action as 'ActionCode',
		   wa.action + ' : ' + c.codedescript as 'ActionDescription'
	from #StratWorklists sw inner join cmworklistactions wa on
		sw.worklist = wa.worklist inner join code c on
		c.code = wa.action
	where c.category = 'FUA'


    if (@@error != 0)
    begin
        set @return = @@error
    end
end

GO
