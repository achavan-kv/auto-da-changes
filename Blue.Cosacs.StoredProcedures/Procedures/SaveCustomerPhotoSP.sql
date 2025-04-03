IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[SaveCustomerPhotoSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveCustomerPhotoSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 11/04/2007
-- Description:	Saves customer photo detalis
-- =============================================
CREATE PROCEDURE [dbo].[SaveCustomerPhotoSP] 
	@customerID VARCHAR(20),
    @filename VARCHAR(100),
    @takenby INT,
    @fileExists bit OUT,
    @return INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0

    IF NOT EXISTS(SELECT * FROM CustomerPhotos WHERE FileName = @filename)
    BEGIN
    INSERT INTO CustomerPhotos(CustomerID,FileName,DateTaken,TakenBy)
    VALUES(@customerID,@filename,GETDATE(),@takenby)
    SET @fileExists = 0
    END
    ELSE IF (DATEDIFF(DAY,(SELECT DateTaken FROM CustomerPhotos WHERE FileName = @filename),GETDATE())) = 0
    BEGIN 
    UPDATE  CustomerPhotos
    SET DateTaken = GETDATE(),TakenBy = @takenby
    WHERE [FileName] = @filename AND CustomerID = @customerID
    SET @fileExists = 0
    END
    ELSE
    BEGIN
    SET @fileExists = 1
    END

    SET @return = @@error
END

