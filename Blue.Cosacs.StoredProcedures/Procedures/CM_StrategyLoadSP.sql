
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_StrategyLoadSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_StrategyLoadSP]
GO
--exec CM_StrategyLoadSP 0

-- =============================================
-- Author:		Jez Hemans
-- Create date: 19/03/2007
-- Description:	Returns the contents of the CM Strategy table
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 18/09/09  jec UAT856 return Manual column

-- =============================================
CREATE PROCEDURE CM_StrategyLoadSP
	@return INT OUTPUT
AS
SET @return = 0

--SELECT Strategy
--      ,RTRIM(Strategy) + ' ' + Description AS 'Description'
--      ,IsActive
--      ,ReadOnly
--  FROM dbo.CMStrategy
--ORDER BY Description

--IP - 02/06/09 - Credit Collection Walkthrough Changes - Allocation checkbox on 'Strategy Configuration' screen.
SELECT cms.Strategy
      ,RTRIM(cms.Strategy) + ' ' + cms.Description AS 'Description'
      ,cms.IsActive
      ,cms.ReadOnly
	  ,isnull(c.reference,0) AS 'Reference'
	  ,cms.Manual
  FROM dbo.CMStrategy cms
  inner join code c
  on cms.strategy = c.code
  and c.category = 'SS1'

ORDER BY cms.Description

SET @return = @@error
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End 
