


SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_GetStrategyWorkListsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_GetStrategyWorkListsSP]
GO

-- ==============================================================================================
-- Author:		Jez Hemans
-- Create date: 04/04/2007
-- Description:	Procedure that returns the worklists and the strategies linked to these worklists
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/01/10  IP  UAT(950) - Missing join onto CMStrategy table.

-- ==============================================================================================

CREATE PROCEDURE  dbo.CM_GetStrategyWorkListsSP
				@return	int	OUTPUT

AS
   SET @return = 0    --initialise return code

    SELECT w.worklist as WorkListCode, 
					w.worklist + ' ' + w.description AS 'WorkList',
					w.description,
					ISNULL(MAX(c.Strategy),'') AS Strategy
    FROM    CMWorkList w
	LEFT JOIN  CMStrategycondition c 
	INNER JOIN CMStrategy s ON c.Strategy = s.Strategy --IP - 07/01/10 - UAT(950)
	ON w.worklist = LEFT(c.ActionCode,ISNULL(CHARINDEX(' ',c.ACTIONcode),1))
	GROUP BY w.WorkList,w.description
	IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
  
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

