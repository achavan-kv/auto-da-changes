ALTER TABLE Warranty.WarrantySale
	ADD AgreementNumber INT NULL
GO

UPDATE Warranty.WarrantySale
SET AgreementNumber = SUBSTRING(invoicenumber, CHARINDEX(' ', invoicenumber, 0) + 1, LEN(invoicenumber) - CHARINDEX(' ', invoicenumber, 0) + 1)
GO
