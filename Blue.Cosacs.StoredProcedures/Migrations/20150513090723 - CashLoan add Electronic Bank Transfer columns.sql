-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND  Column_Name = 'Bank')
BEGIN
	ALTER TABLE CashLoan Add Bank varchar(6) null
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND  Column_Name = 'BankAccountType')
BEGIN
	ALTER TABLE CashLoan Add BankAccountType char(1) null
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND  Column_Name = 'BankBranch')
BEGIN
	ALTER TABLE CashLoan Add BankBranch varchar(20) null
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND  Column_Name = 'BankAcctNo')
BEGIN
	ALTER TABLE CashLoan Add BankAcctNo varchar(20) null
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND  Column_Name = 'Notes')
BEGIN
	ALTER TABLE CashLoan Add Notes varchar(200) null
END