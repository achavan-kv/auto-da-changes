IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[SaveCustomerSignatureSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[SaveCustomerSignatureSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 11/04/2007
-- Description:	Saves customer signature detalis
-- =============================================
CREATE PROCEDURE [dbo].[SaveCustomerSignatureSP] 
	@customerID VARCHAR(20),
    @filename VARCHAR(100),
    @fileExists bit OUT,
    @return INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0

    IF NOT EXISTS(SELECT * FROM CustomerSignatures WHERE FileName = @filename)
    BEGIN
    INSERT INTO CustomerSignatures (CustomerID,FileName,DateTaken)
    VALUES(@customerID,@filename,GETDATE())
    SET @fileExists = 0
    END
    ELSE IF (DATEDIFF(DAY,(SELECT DateTaken FROM CustomerSignatures WHERE FileName = @filename),GETDATE())) = 0
    BEGIN 
    UPDATE  CustomerSignatures
    SET DateTaken = GETDATE()
    WHERE [FileName] = @filename AND CustomerID = @customerID
    SET @fileExists = 0
    END
    ELSE
    BEGIN 
    SET @fileExists = 1
    END

    SET @return = @@error
END
