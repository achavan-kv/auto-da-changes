-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoanDisbursement' AND Column_Name = 'BankTransferRefNo')
BEGIN
	ALTER TABLE CashLoanDisbursement Add BankTransferRefNo varchar(30)
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoanDisbursement' AND Column_Name = 'BankTransferDate')
BEGIN
	ALTER TABLE CashLoanDisbursement Add BankTransferDate date
END
GO