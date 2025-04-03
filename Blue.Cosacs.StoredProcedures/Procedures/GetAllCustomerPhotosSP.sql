IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetAllCustomerPhotosSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetAllCustomerPhotosSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 11/04/2007
-- Description:	Gets all customer photos for selected customer
-- =============================================
CREATE PROCEDURE [dbo].[GetAllCustomerPhotosSP] 
	@customerID VARCHAR(20),
    @return INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0

    SELECT FileName,DateTaken,TakenBy FROM CustomerPhotos 
    WHERE CustomerID = @customerID
    ORDER BY DateTaken DESC
    

    SET @return = @@error
END

