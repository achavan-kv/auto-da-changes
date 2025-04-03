ALTER TABLE dbo.StoreCard ADD CONSTRAINT
	FK_StoreCard_acct FOREIGN KEY
	(
	AcctNo
	) REFERENCES dbo.acct
	(
	acctno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
