-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[CashAndGoReceipt]') AND type in (N'U'))
BEGIN
	CREATE TABLE CashAndGoReceipt
	(
		ID INT IDENTITY(1,1),
		AcctNo VARCHAR(12) NOT NULL,
		AgrmtNo INT NOT NULL,
		TaxExempt BIT NOT NULL,
		Change MONEY,
		CashierEmpeeNo INT,
		CONSTRAINT [PK_CashAndGoReceipt] PRIMARY KEY CLUSTERED
		(
			ID
		)
	)
	
	CREATE NONCLUSTERED INDEX [ix_CashAndGoReceipt] ON [dbo].[CashAndGoReceipt] 
	(
		AcctNo,
		AgrmtNo
	)
	
END

