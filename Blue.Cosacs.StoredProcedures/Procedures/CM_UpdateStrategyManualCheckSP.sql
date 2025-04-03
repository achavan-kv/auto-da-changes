
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_UpdateStrategyManualCheckSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_UpdateStrategyManualCheckSP]
GO
-- ====================================================================================================================
-- Author:		Ilyas Parker
-- Create date: 21/09/2009
-- Description:	Updates the CMStrategy.Manual column which determines whether a strategy is manual or not.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies
-- ====================================================================================================================

CREATE PROCEDURE [dbo].[CM_UpdateStrategyManualCheckSP] 
	@strategy varchar(7),		-- #9521
    @manual bit,
    @return int output
AS
BEGIN
    SET @return = 0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	UPDATE CMStrategy
	SET manual = @manual
	WHERE strategy = @strategy
	
 SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End