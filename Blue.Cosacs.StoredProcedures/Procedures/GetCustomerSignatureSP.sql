IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('[dbo].[GetCustomerSignatureSP]') AND OBJECTPROPERTY(id,'IsProcedure') = 1)
DROP PROCEDURE [dbo].[GetCustomerSignatureSP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 11/04/2007
-- Description:	Gets customer signature for selected customer
-- =============================================
CREATE PROCEDURE [dbo].[GetCustomerSignatureSP] 
	@customerID VARCHAR(20),
    @fileName VARCHAR(100) OUT,
    @return INT OUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET @return = 0

    SET @fileName = (SELECT TOP 1 FileName FROM CustomerSignatures
    WHERE CustomerID = @customerID
    ORDER BY DateTaken DESC)
    

    SET @return = @@error
END
