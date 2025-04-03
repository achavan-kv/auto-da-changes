-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[ProductType] (
	Id INT NOT NULL IDENTITY(1,1),
	Name varchar(100) NOT NULL,
	Description varchar(100) NOT NULL,
	CONSTRAINT [PK_ProductType] PRIMARY KEY (Id ASC)
)

INSERT INTO  [Merchandising].[ProductType] VALUES('Regular', 'Regular')
INSERT INTO  [Merchandising].[ProductType] VALUES('Repossessed', 'Repossessed')
INSERT INTO  [Merchandising].[ProductType] VALUES('Without Stock', 'Without Stock')
INSERT INTO  [Merchandising].[ProductType] VALUES('Spare Parts', 'Spare Parts')

ALTER TABLE [Merchandising].[Product] ADD [ProductTypeId] int NOT NULL
ALTER TABLE [Merchandising].[Product] WITH CHECK ADD CONSTRAINT [FK_Product_ProductType] FOREIGN KEY ([ProductTypeId]) REFERENCES [Merchandising].[ProductType]([Id])
ALTER TABLE [Merchandising].[Product] CHECK CONSTRAINT [FK_Product_ProductType]

ALTER TABLE [Merchandising].[ProductAttribute] WITH CHECK ADD CONSTRAINT [UQ_ProductAttribute] UNIQUE ([ProductID], [Name])

ALTER TABLE [Merchandising].[Product] DROP COLUMN [Type]
