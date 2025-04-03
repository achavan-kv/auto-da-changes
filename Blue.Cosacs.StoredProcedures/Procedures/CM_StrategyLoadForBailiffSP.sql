
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_StrategyLoadForBailiffSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_StrategyLoadForBailiffSP]
GO
--exec CM_StrategyLoadSP 0

-- =============================================
-- Author:		Jez Hemans
-- Create date: 19/03/2007
-- Description:	Returns the contents of the CM Strategy table
-- Changes: 
-- =============================================
CREATE PROCEDURE CM_StrategyLoadForBailiffSP
	@return INT OUTPUT
AS
SET @return = 0

SELECT Strategy
      ,RTRIM(Strategy) + ' ' + Description AS 'Description'
      ,IsActive
      ,ReadOnly
  FROM dbo.CMStrategy
  -- Strategy dropdown in Bailiff Review Screen is readonly (display purpose only)
  --UNION     
  --SELECT '' AS Strategy
  --    ,'ALL' AS 'Description'
  --    ,0
  --    ,0
ORDER BY Description

SET @return = @@error
GO
