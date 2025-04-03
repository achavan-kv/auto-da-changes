-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF OBJECT_ID('Financial.[Transaction]') IS NOT NULL
	DROP TABLE Financial.[Transaction]
GO

CREATE TABLE Financial.[Transaction]
(
	Id			int IDENTITY(1,1)	NOT NULL,
	RunNo		smallint			NULL,
	Account		varchar(32)			NOT NULL,
	BranchNo	smallint			NOT NULL,
	Type		char(3)				NOT NULL,
	Amount		decimal(12, 4)		NOT NULL,
	Date		date				NOT NULL,
	MessageId	Int					NOT NULL
) 
GO
GO

ALTER TABLE Financial.[Transaction] ADD  CONSTRAINT PK_FinancialTransaction PRIMARY KEY CLUSTERED 
(
	ID ASC
)
GO

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

CREATE NONCLUSTERED INDEX IX_RumNumber ON Financial.[Transaction]
(
	RunNo DESC
) WITH
( 
	STATISTICS_NORECOMPUTE = OFF, 
	IGNORE_DUP_KEY = OFF, 
	ALLOW_ROW_LOCKS = ON, 
	ALLOW_PAGE_LOCKS = ON
) ON [PRIMARY]
GO
