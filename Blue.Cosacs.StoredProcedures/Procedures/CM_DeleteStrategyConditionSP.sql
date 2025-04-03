SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_DeleteStrategyConditionSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_DeleteStrategyConditionSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 20/03/2007
-- Description:	Clears the CMStrategyCondition table for passed strategy
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies
-- =============================================
CREATE PROCEDURE [dbo].[CM_DeleteStrategyConditionSP]
    @strategy varchar(7),		-- #9521
    --@type CHAR (1),
    @return INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0
    
    DELETE FROM  dbo.CMStrategyCondition
    WHERE strategy =@strategy --AND (condition IN (SELECT condition FROM CMCondition WHERE Type = @type OR Type = '') OR condition = '')
      
    SET @return = @@error
END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End