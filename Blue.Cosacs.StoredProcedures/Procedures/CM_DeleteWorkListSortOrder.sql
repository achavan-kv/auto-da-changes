SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[CM_DeleteWorkListSortOrder]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[CM_DeleteWorkListSortOrder]
GO

-- =============================================
-- Author:		Mohamed Nasmi
-- Create date: 22/01/2009
-- Description:	Deletes Work List Sort Order
-- =============================================

CREATE PROCEDURE 	[dbo].[CM_DeleteWorkListSortOrder]
			@EmpeeType varchar(3),
			@return int OUTPUT
AS
	-- SET NOCOUNT ON added to marginally improve performance 
	SET NOCOUNT ON;
    SET @return = 0
	
	DELETE FROM CMWorkListSortOrder
	
	SET @return = @@ERROR
	RETURN @return

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO