


SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetWorkListsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetWorkListsSP]
GO

-- ============================================================================================
-- Author:		
-- Create date: 
-- Description:	
-- Changed: IP - 08/06/09 - Credit Collection Walkthrough Changes.
--			Previously was preventing supervisor worklists (worklists not linked to a strategy)
--			from being returned.     		
-- ============================================================================================

CREATE PROCEDURE  dbo.CM_GetWorkListsSP
				@return	int	OUTPUT

AS

    SET @return = 0    --initialise return code

    SELECT			w.worklist,
					w.description,
					ISNULL(s.Strategy,'Sup') AS Column1,
					ISNULL(S.Description,'Supervisor Worklist') AS Strategy
	INTO #worklists
    FROM CMWorkList w
    LEFT JOIN CMStrategyCondition c 
    ON LEFT(c.ActionCode,CHARINDEX (' ',c.ActionCode)-1) = w.WorkList
	AND c.StepActiontype ='W'
	LEFT JOIN CMStrategy s ON S.Strategy = C.Strategy
    --WHERE c.StepActiontype ='W' --IP - 08/06/09 - Credit Collection Walkthrough changes
    GROUP BY s.strategy,w.worklist,w.description,s.description
    order by s.strategy,w.worklist

	--Need to remove the worklist entry for 'SUP' in the instance where it appears as
	--a supervisor worklist and a worklist attached to a strategy. If this is the case then
	--the worklist cannot be a supervisor worklist.
	
	delete from #worklists 
	where exists(select * from #worklists w1, cmstrategy s1, cmworklist cmw1
				where w1.Column1 = s1.strategy
				and w1.worklist = cmw1.worklist
				and w1.worklist = #worklists.worklist
			    and exists(select * from #worklists w2
							 where w2.worklist = w1.worklist
							 and w2.Column1 = 'SUP'))
	and #worklists.Column1 = 'SUP' 
    and #worklists.WorkList !='SUP'
    
	select * from #worklists
	
    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

