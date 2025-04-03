-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (select * from sys.indexes where name ='idx_CashLoan_LoanStatus_acctno')
	DROP INDEX [idx_CashLoan_LoanStatus_acctno] ON [dbo].[CashLoan]
GO
CREATE NONCLUSTERED INDEX idx_CashLoan_LoanStatus_acctno
ON [dbo].[CashLoan] ([LoanStatus])
INCLUDE ([AcctNo])
GO