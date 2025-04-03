
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_DeleteActionRights]') 
		AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_DeleteActionRights]
GO
-- =============================================
-- Author:		Alex Ayscough
-- Create date: 08/01/2009
-- Description:	Removes action rights
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/06/09 jec Check for Strategy 'All'	
-- =============================================
CREATE PROCEDURE [dbo].[CM_DeleteActionRights] 
    @EmpeeNo INT,
	@Strategy VARCHAR(10),
	@Action VARCHAR(10),
	@return INT OUTPUT
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	DELETE CMActionRights
	WHERE EmpeeNo = @EmpeeNo AND 
	(Strategy = @Strategy or (Strategy=' ' and @Strategy ='ALL')) AND  
	Action = @Action 

	RETURN @return
	
-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End


