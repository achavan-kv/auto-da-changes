-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'CashLoanDisbursement' AND Column_Name = 'PayMethod')
BEGIN
	
    EXEC sp_RENAME 'CashLoanDisbursement.PayMethod' , 'DisbursementType'

END
GO

