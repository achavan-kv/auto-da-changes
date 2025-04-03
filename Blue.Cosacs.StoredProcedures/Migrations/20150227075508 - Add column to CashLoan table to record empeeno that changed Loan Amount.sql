-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoan' AND Column_Name = 'EmpeenoLoanAmountChanged')
BEGIN
	ALTER TABLE CashLoan Add EmpeenoLoanAmountChanged int null
END
GO