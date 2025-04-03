-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE Config.DecisionTableType
	(
	[Key] varchar(50) NOT NULL
	)  ON [PRIMARY]
GO
ALTER TABLE Config.DecisionTableType ADD CONSTRAINT
	PK_DecisionTableType PRIMARY KEY CLUSTERED 
	(
	[Key]
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
ALTER TABLE Config.DecisionTableType SET (LOCK_ESCALATION = TABLE)
GO
CREATE TABLE Config.DecisionTable
	(
	Id int NOT NULL IDENTITY (1, 1),
	[Key] varchar(50) NOT NULL,
	CreatedUtc datetime NOT NULL,
	Value varchar(MAX) NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE Config.DecisionTable ADD CONSTRAINT
	PK_DecisionTable PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE NONCLUSTERED INDEX IX_DecisionTable ON Config.DecisionTable
	(
	[Key],
	CreatedUtc DESC
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE Config.DecisionTable ADD CONSTRAINT
	FK_DecisionTable_DecisionTableType FOREIGN KEY
	(
	[Key]
	) REFERENCES Config.DecisionTableType
	(
	[Key]
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Config.DecisionTable SET (LOCK_ESCALATION = TABLE)
GO