-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'BankCode' AND [object_id] = OBJECT_ID(N'Payments.Bank'))BEGIN
	ALTER TABLE Payments.Bank DROP COLUMN BankCode
END 
GO

TRUNCATE TABLE Payments.Bank
GO

ALTER TABLE Payments.Bank ADD BankCode varchar(6) NOT NULL
GO

-- Migrate existing Banks into Bank table
INSERT INTO Payments.Bank (BankName, BankCode, Active)
SELECT b.bankname, b.bankcode, 1
FROM Bank b
WHERE b.bankname != ''
GO