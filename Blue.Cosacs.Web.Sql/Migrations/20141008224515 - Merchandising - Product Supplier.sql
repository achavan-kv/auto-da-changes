-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[ProductSupplier] (
	Id int NOT NULL IDENTITY(1,1),
	ProductId int NOT NULL,
	SupplierId int NOT NULL,
	CONSTRAINT [PK_ProductSupplier] PRIMARY KEY CLUSTERED (Id ASC)
)

ALTER TABLE [Merchandising].[ProductSupplier] WITH CHECK ADD CONSTRAINT [FK_ProductSupplier_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product]([Id])
ALTER TABLE [Merchandising].[ProductSupplier] CHECK CONSTRAINT [FK_ProductSupplier_Product]

ALTER TABLE [Merchandising].[ProductSupplier] WITH CHECK ADD CONSTRAINT [FK_ProductSupplier_Supplier] FOREIGN KEY ([SupplierId]) REFERENCES [Merchandising].[Supplier]([Id])
ALTER TABLE [Merchandising].[ProductSupplier] CHECK CONSTRAINT [FK_ProductSupplier_Supplier]

ALTER TABLE [Merchandising].[ProductSupplier] WITH CHECK ADD CONSTRAINT [UQ_ProductSupplier] UNIQUE ([ProductId],[SupplierId])