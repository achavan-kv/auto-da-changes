SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_DeleteWorkLists]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_DeleteWorkLists]
GO


-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 22/01/2009
-- Description:	Deletes Worklists
-- =============================================

CREATE PROCEDURE [dbo].[CM_DeleteWorkLists] 
    @WorkList VARCHAR(10),
	@return INT OUTPUT

AS	
		-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	DELETE CMWorkList
	WHERE WorkList =  @WorkList 
   
	SET @return = @@ERROR

	RETURN @return

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO