-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
DROP TABLE [Merchandising].[ProductAttribute]
ALTER TABLE [Merchandising].[Product] DROP COLUMN InternalUPC
ALTER TABLE [Merchandising].[Product] DROP COLUMN VendorUPC
ALTER TABLE [Merchandising].[Product] DROP COLUMN PrimaryBrand
ALTER TABLE [Merchandising].[Product] DROP COLUMN SecondaryBrand
ALTER TABLE [Merchandising].[Product] DROP COLUMN ModelNumber
ALTER TABLE [Merchandising].[Product] DROP COLUMN SupplierSKU
ALTER TABLE [Merchandising].[Product] DROP COLUMN ShortDescription

ALTER TABLE [Merchandising].[Product] ADD [POSDescription] varchar(100) NOT NULL
ALTER TABLE [Merchandising].[Product] ADD [Attributes] text NULL
ALTER TABLE [Merchandising].[Product] ADD CONSTRAINT [UQ_Product_SKU] UNIQUE (SKU)
ALTER TABLE [Merchandising].[Product] ADD [CreatedDate] datetime NOT NULL DEFAULT(GETDATE())
ALTER TABLE [Merchandising].[Product] ADD [LastUpdatedDate] datetime NOT NULL DEFAULT(GETDATE())