-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Customer' AND  Column_Name = 'CashLoanNew')
BEGIN
	ALTER TABLE Customer Add CashLoanNew bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Customer' AND  Column_Name = 'CashLoanRecent')
BEGIN
	ALTER TABLE Customer Add CashLoanRecent bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Customer' AND  Column_Name = 'CashLoanExisting')
BEGIN
	ALTER TABLE Customer Add CashLoanExisting bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Customer' AND  Column_Name = 'CashLoanStaff')
BEGIN
	ALTER TABLE Customer Add CashLoanStaff bit not null default 0
END
GO