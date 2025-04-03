
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SetStrategyActiveSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SetStrategyActiveSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 22/03/2007
-- Description:	Sets the IsActive field for the passed strategy to either active or not
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies
-- =============================================
CREATE PROCEDURE [dbo].[CM_SetStrategyActiveSP]
	@strategy varchar(7),		-- #9521
    @activeValue SMALLINT,
    @return INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0

    UPDATE dbo.CMStrategy
    SET IsActive = @activeValue
    WHERE Strategy = @strategy

    SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End