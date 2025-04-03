-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns  WHERE table_name ='CashLoan' AND column_name = 'TermsType')
BEGIN
	ALTER TABLE CashLoan ADD [TermsType] VARCHAR(2) 
END
GO 


