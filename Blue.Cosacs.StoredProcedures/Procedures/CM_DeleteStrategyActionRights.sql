SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_DeleteStrategyActionRights]') 
		AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_DeleteStrategyActionRights]
GO

CREATE PROCEDURE [dbo].[CM_DeleteStrategyActionRights]
-- =============================================
-- Author:		John Croft
-- Create date: 18/11/2010
-- Description:	Removes action rights for a Strategy if the action
--				does not exist on the CMStrategyActions table.
--              This procedure is executed during the Save of the Strategy Configuration
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--	
-- ============================================= 
--	Add parameters here
	@Strategy VARCHAR(10),
	@return INT OUTPUT
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	Delete from CMActionRights 
	where not exists(select * from CMStrategyActions a 
			where a.strategy=CMActionRights.strategy and a.actioncode=CMActionRights.[action])
	and CMActionRights.strategy=@strategy

	RETURN @return
	
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End

