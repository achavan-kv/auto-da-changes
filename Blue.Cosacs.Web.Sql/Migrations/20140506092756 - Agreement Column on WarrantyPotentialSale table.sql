-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'AgreementNumber' AND [object_id] = OBJECT_ID(N'Warranty.WarrantyPotentialSale'))
BEGIN
	ALTER TABLE Warranty.WarrantyPotentialSale
		ADD AgreementNumber INT NULL
END
GO

UPDATE Warranty.WarrantyPotentialSale
SET AgreementNumber = SUBSTRING(invoicenumber, CHARINDEX(' ', invoicenumber, 0) + 1, LEN(invoicenumber) - CHARINDEX(' ', invoicenumber, 0) + 1)
GO
