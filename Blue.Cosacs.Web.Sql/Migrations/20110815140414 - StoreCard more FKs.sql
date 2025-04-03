-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE dbo.StoreCardAccountStatus_Lookup ADD CONSTRAINT
	PK_StoreCardAccountStatus_Lookup PRIMARY KEY CLUSTERED 
	(
	Status
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.StoreCardAccountStatus_Lookup SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.StoreCardCardStatus_Lookup ADD CONSTRAINT
	PK_StoreCardCardStatus_Lookup PRIMARY KEY CLUSTERED 
	(
	Status
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.StoreCardCardStatus_Lookup SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.StoreCardPaymentDetails ADD CONSTRAINT
	FK_StoreCardPaymentDetails_StoreCardAccountStatus_Lookup FOREIGN KEY
	(
	Status
	) REFERENCES dbo.StoreCardAccountStatus_Lookup
	(
	Status
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.StoreCardPaymentDetails SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.StoreCard SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.StoreCardStatus SET (LOCK_ESCALATION = TABLE)
GO
ALTER TABLE dbo.StoreCardStatements ADD CONSTRAINT
	FK_StoreCardStatements_StoreCardPaymentDetails FOREIGN KEY
	(
	Acctno
	) REFERENCES dbo.StoreCardPaymentDetails
	(
	acctno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.StoreCardStatements SET (LOCK_ESCALATION = TABLE)
GO
