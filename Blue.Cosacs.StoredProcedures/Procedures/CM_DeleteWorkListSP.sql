
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[CM_DeleteWorkListSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[CM_DeleteWorkListSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 04/04/2007
-- Description:	Permanently deletes the work list from the database
-- =============================================
CREATE PROCEDURE [dbo].[CM_DeleteWorkListSP] 
	            @worklist varchar(10),
				@return	int	OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0    --initialise return code

    DELETE FROM dbo.CMWorkList
    WHERE WorkList = @worklist

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END
END

GO
