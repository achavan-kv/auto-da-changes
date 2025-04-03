ALTER TABLE dbo.StoreCard
	DROP CONSTRAINT FK_StoreCard_custacct
GO
ALTER TABLE dbo.StoreCard
	DROP CONSTRAINT FK_StoreCard_StoreCardRate
GO
CREATE TABLE dbo.Tmp_StoreCard
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
	Number char(16) NOT NULL,
	Name varchar(26) NOT NULL,
	IssueYear smallint NOT NULL,
	IssueMonth tinyint NOT NULL,
	ExpirationYear smallint NOT NULL,
	ExpirationMonth tinyint NOT NULL,
	FixedRate dbo.interest_rate NULL,
	RateId int NULL,
	AcctNo char(12) NOT NULL,
	CustId varchar(20) NOT NULL
	)  ON [PRIMARY]
GO
IF EXISTS(SELECT * FROM dbo.StoreCard)
	 EXEC('INSERT INTO dbo.Tmp_StoreCard (Id, Number, Name, IssueYear, IssueMonth, ExpirationYear, ExpirationMonth, FixedRate, RateId, AcctNo, CustId)
		SELECT Id, Number, Name, IssueYear, IssueMonth, ExpirationYear, ExpirationMonth, FixedRate, RateId, AcctNo, CustId FROM dbo.StoreCard WITH (HOLDLOCK TABLOCKX)')
GO
DROP TABLE dbo.StoreCard
GO
EXECUTE sp_rename N'dbo.Tmp_StoreCard', N'StoreCard', 'OBJECT' 
GO
ALTER TABLE dbo.StoreCard ADD CONSTRAINT
	PK_StoreCard PRIMARY KEY CLUSTERED 
	(
	Id
	)

GO
CREATE UNIQUE NONCLUSTERED INDEX IX_StoreCard_Number ON dbo.StoreCard
	(
	Number
	) 
GO
ALTER TABLE dbo.StoreCard ADD CONSTRAINT
	FK_StoreCard_StoreCardRate FOREIGN KEY
	(
	RateId
	) REFERENCES dbo.StoreCardRate
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE dbo.StoreCard ADD CONSTRAINT
	FK_StoreCard_custacct FOREIGN KEY
	(
	CustId,
	AcctNo
	) REFERENCES dbo.custacct
	(
	custid,
	acctno
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 	
GO
