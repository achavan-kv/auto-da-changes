-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE [Merchandising].[ProductStatus] (
	Id int NOT NULL IDENTITY(1,1),
	Name varchar(50) NOT NULL,
	IsAvailable bit NOT NULL DEFAULT(1),
	IsAutomatic bit NOT NULL DEFAULT(0),
	CONSTRAINT [PK_ProductStatus] PRIMARY KEY CLUSTERED (Id ASC)
)

INSERT INTO [Merchandising].[ProductStatus] (Name, IsAvailable, IsAutomatic) VALUES ('Unknown', 0, 1);
INSERT INTO [Merchandising].[ProductStatus] (Name, IsAvailable, IsAutomatic) VALUES ('New', 1, 0);
INSERT INTO [Merchandising].[ProductStatus] (Name, IsAvailable, IsAutomatic) VALUES ('Current', 1, 0);
INSERT INTO [Merchandising].[ProductStatus] (Name, IsAvailable, IsAutomatic) VALUES ('Discontinued', 1, 0);
INSERT INTO [Merchandising].[ProductStatus] (Name, IsAvailable, IsAutomatic) VALUES ('Deleted', 0, 0);
INSERT INTO [Merchandising].[ProductStatus] (Name, IsAvailable, IsAutomatic) VALUES ('Aged', 1, 1);

ALTER TABLE [Merchandising].[Product]
	ADD [Status] int NOT NULL DEFAULT(1),
	CONSTRAINT [FK_Product_Status] FOREIGN KEY ([Status]) REFERENCES [Merchandising].[ProductStatus](Id)