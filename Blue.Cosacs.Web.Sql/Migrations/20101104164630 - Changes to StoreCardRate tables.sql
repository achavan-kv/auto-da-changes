
CREATE TABLE dbo.Tmp_StoreCardRate
	(
	Id int NOT NULL,
	[$Version] int NOT NULL,
	[$IsDeleted] bit NOT NULL,
	[$DeletedBy] varchar(50) NULL,
	[$DeletedOn] datetime NULL,
	[$CreatedBy] varchar(50) NULL,
	[$CreatedOn] datetime NULL,
	[$LastUpdatedBy] varchar(50) NULL,
	[$LastUpdatedOn] datetime NULL,
	Name varchar(50) NOT NULL,
	ScoreFrom int NOT NULL,
	ScoreTo int NOT NULL,
	RetailRateFixed float(53) NULL,
	RetailRateVariable float(53) NULL
	)  ON [PRIMARY]
GO
IF EXISTS(SELECT * FROM dbo.StoreCardRates)
	 EXEC('INSERT INTO dbo.Tmp_StoreCardRate (Id, Name, ScoreFrom, ScoreTo, RetailRateFixed)
		SELECT Id, RateName, ScoreFrom, ScoreTo, Retail FROM dbo.StoreCardRates WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.StoreCardRates
GO
EXECUTE sp_rename N'dbo.Tmp_StoreCardRate', N'StoreCardRate', 'OBJECT' 
GO
ALTER TABLE dbo.StoreCardRate ADD CONSTRAINT
	PK_StoreCardRate PRIMARY KEY NONCLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE TABLE dbo.Tmp_StoreCardRateAudit
	(
	[$AuditId] int NOT NULL,
	Id int NOT NULL,
	[$Action] char(1) NOT NULL,
	[$CreatedOn] datetime NULL,
	[$CreatedBy] varchar(50) NULL,
	Name varchar(50) NOT NULL,
	ScoreFrom int NOT NULL,
	ScoreTo int NOT NULL,
	RetailRateFixed float(53) NOT NULL,
	RetailRateVariable float(53) NOT NULL
	)  ON [PRIMARY]
GO
IF EXISTS(SELECT * FROM dbo.StoreCardRatesAudit)
	 EXEC('INSERT INTO dbo.Tmp_StoreCardRateAudit ([$AuditId], Name, ScoreFrom, ScoreTo, RetailRateFixed, RetailRateVariable)
		SELECT Id, RateName, ScoreFrom, ScoreTo, Retail, Cash FROM dbo.StoreCardRatesAudit WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.StoreCardRatesAudit
GO
EXECUTE sp_rename N'dbo.Tmp_StoreCardRateAudit', N'StoreCardRateAudit', 'OBJECT' 
GO
ALTER TABLE dbo.StoreCardRateAudit ADD CONSTRAINT
	PK_StoreCardRateAudit PRIMARY KEY NONCLUSTERED 
	(
	[$AuditId]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE dbo.StoreCardRateAudit ADD CONSTRAINT
	FK_StoreCardRateAudit_StoreCardRate FOREIGN KEY
	(
	Id
	) REFERENCES dbo.StoreCardRate
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
