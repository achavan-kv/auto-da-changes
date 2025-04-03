-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[ProductAttribute] (
	Id int NOT NULL IDENTITY(1,1),
	ProductId int NOT NULL,
	Name varchar(100) NOT NULL,
	Value varchar(MAX) NULL,
	CONSTRAINT [PK_ProductAttribute] PRIMARY KEY (Id Asc))

ALTER TABLE [Merchandising].[ProductAttribute] WITH CHECK ADD CONSTRAINT [FK_ProductAttribute_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product](Id)
ALTER TABLE [Merchandising].[ProductAttribute] CHECK CONSTRAINT [FK_ProductAttribute_Product]