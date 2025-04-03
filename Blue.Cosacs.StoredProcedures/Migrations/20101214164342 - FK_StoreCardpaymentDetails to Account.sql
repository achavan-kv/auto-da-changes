-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE dbo.StoreCardPaymentDetails ADD CONSTRAINT
	FK_StoreCardPaymentDetails_acct FOREIGN KEY
	(
	acctno
	) REFERENCES dbo.acct
	(
	acctno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 