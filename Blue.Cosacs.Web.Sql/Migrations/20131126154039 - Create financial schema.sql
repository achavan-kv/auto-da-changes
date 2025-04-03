-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM sys.schemas where name = 'Financial')
	exec SP_EXECUTESQL N'CREATE SCHEMA Financial'
GO

IF OBJECT_ID('Financial.[Transaction]') IS NULL
	CREATE TABLE Financial.[Transaction]
	(
		Id					Int IDENTITY(1, 1)	NOT NULL,
		RumNumber			SmallInt			NULL,
		Account				VarChar(32)			NOT NULL,
		BranchNo			SmallInt			NOT NULL,
		[Type]				Char(3)				NOT NULL,
		Amount				Decimal(12, 4)		NOT NULL,
		[Date]				Date				NOT NULL,
		Category			SmallInt			NOT NULL
	)
	GO

IF OBJECT_ID('Financial.[Transaction]') IS NULL
	ALTER TABLE Financial.[Transaction] ADD  CONSTRAINT PK_FinancialTransaction PRIMARY KEY CLUSTERED 
	(
		ID ASC
	)
	GO

IF OBJECT_ID('Financial.[Transaction]') IS NULL
	ALTER TABLE Financial.[Transaction] ADD CONSTRAINT FK_FinancialTransaction_Branch FOREIGN KEY
	(
		BranchNo
	) REFERENCES dbo.branch
	(
		branchno
	) 
		ON UPDATE  NO ACTION 
		ON DELETE  NO ACTION 
	GO

IF OBJECT_ID('Financial.[Transaction]') IS NULL
	CREATE NONCLUSTERED INDEX IX_RumNumber ON Financial.[Transaction]
	(
		RumNumber DESC
	) WITH
	( 
		STATISTICS_NORECOMPUTE = OFF, 
		IGNORE_DUP_KEY = OFF, 
		ALLOW_ROW_LOCKS = ON, 
		ALLOW_PAGE_LOCKS = ON
	) ON [PRIMARY]
	GO
