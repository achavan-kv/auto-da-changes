
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_SaveStrategyAllocationCheckSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_SaveStrategyAllocationCheckSP]
GO
-- ====================================================================================================================
-- Author:		Ilyas Parker
-- Create date: 02/06/2009
-- Description:	Updates the 'Reference' column on the code table for category 'SS1'
--				for the strategy to determine whether this strategies accounts can be allocated
--				to Bailiff / Collectors.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/02/12 jec #9521 CR9417 - duplication of existing strategies
-- ====================================================================================================================

CREATE PROCEDURE [dbo].[CM_SaveStrategyAllocationCheckSP] 
	@strategy varchar(7),		-- #9521
    @canBeAllocated bit,
    @return int output
AS
BEGIN
    SET @return = 0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	IF(@canBeAllocated = 1)
	BEGIN
		UPDATE code
		SET reference = '1'
		WHERE code = @strategy
		AND (category = 'SS1' or category = 'SS2')		-- #9521
	END	
	ELSE	
	BEGIN
		UPDATE code
		SET reference = '0'
		WHERE code = @strategy
		AND (category = 'SS1' or category = 'SS2')		-- #9521
	END

 SET @return = @@error
END
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End