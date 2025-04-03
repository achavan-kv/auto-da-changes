CREATE TABLE dbo.StoreCard
	(
	Id int NOT NULL,
	Number char(16) NOT NULL,
	Name varchar(26) NOT NULL,
	IssueYear smallint NOT NULL,
	IssueMonth tinyint NOT NULL,
	ExpirationYear smallint NOT NULL,
	ExpirationMonth tinyint NOT NULL
	)  ON [PRIMARY]
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
