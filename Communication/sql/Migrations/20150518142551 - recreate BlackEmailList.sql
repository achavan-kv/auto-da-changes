-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
DROP TABLE Communication.BlackEmailList
GO

CREATE TABLE Communication.BlackEmailList
	(
	Id int NOT NULL IDENTITY (1, 1),
	Email varchar(128) NOT NULL,
	Reason varchar(32) NULL,
	Provider varchar(32) NOT NULL,
	CreatedOn smalldatetime NOT NULL
	)  ON [PRIMARY]
GO

ALTER TABLE Communication.BlackEmailList ADD CONSTRAINT
	PK_BlackEmailList PRIMARY KEY CLUSTERED 
	(
	Id
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_BlackEmailList_Email ON Communication.BlackEmailList
	(
	Email
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]