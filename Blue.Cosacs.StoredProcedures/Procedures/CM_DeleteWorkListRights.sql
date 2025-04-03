
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


IF EXISTS (select * from dbo.sysobjects where id = object_id('[dbo].[CM_DeleteWorkListRights]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_DeleteWorkListRights]
GO

-- =============================================
-- Author:		Alex Ayscough
-- Create date: 08/01/2009
-- Description:	Deletes Worklist Rights
-- =============================================
CREATE PROCEDURE [dbo].[CM_DeleteWorkListRights] 
    @EmpeeNo INT,
	@WorkList VARCHAR(10),
	@return INT OUTPUT
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	DELETE CMWorkListRights
	WHERE EmpeeNo = @EmpeeNo AND 
	WorkList= @WorkList 

	RETURN @return



GO


