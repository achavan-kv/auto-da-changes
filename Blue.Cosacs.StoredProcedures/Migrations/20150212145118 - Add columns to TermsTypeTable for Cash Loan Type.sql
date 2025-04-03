-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'TermsTypeTable' AND  Column_Name = 'LoanNewCustomer')
BEGIN
	ALTER TABLE TermsTypeTable Add LoanNewCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'TermsTypeTable' AND  Column_Name = 'LoanRecentCustomer')
BEGIN
	ALTER TABLE TermsTypeTable Add LoanRecentCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'TermsTypeTable' AND  Column_Name = 'LoanExistingCustomer')
BEGIN
	ALTER TABLE TermsTypeTable Add LoanExistingCustomer bit not null default 0
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'TermsTypeTable' AND  Column_Name = 'LoanStaff')
BEGIN
	ALTER TABLE TermsTypeTable Add LoanStaff bit not null default 0
END
GO
