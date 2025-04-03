
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'AgreementIsTaxPrinted'
           AND xtype = 'P')
BEGIN 
DROP PROCEDURE AgreementIsTaxPrinted
END
GO

CREATE PROCEDURE AgreementIsTaxPrinted
@Acctno VARCHAR(12),
@return INT OUTPUT

AS
BEGIN
IF EXISTS (SELECT * FROM agreement     
     WHERE acctno = @Acctno    
     AND TaxInvoicePrinted = 'Y')    
	BEGIN
		SELECT 1
	END
ELSE
	BEGIN
		SELECT 0
	END
END



