-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

ALTER TABLE [Merchandising].[Product] DROP CONSTRAINT [UQ_Product_SKU]
DROP INDEX [IX_Merchandising_Product] ON [Merchandising].[Product]

alter table merchandising.product alter column SKU varchar(20) NOT NULL
alter table merchandising.product alter column LongDescription varchar(240) NOT NULL
alter table merchandising.product alter column POSDescription varchar(240) NOT NULL
alter table merchandising.product add SKUStatus char(1) NOT NULL
alter table merchandising.product add CorporateUPC varchar(20) NOT NULL
alter table merchandising.product add VendorUPC varchar(60) NULL
alter table merchandising.product add BrandCode varchar(6) NOT NULL
alter table merchandising.product add BrandName varchar(25) NOT NULL
alter table merchandising.product add VendorStyleLong varchar(50) NOT NULL
alter table merchandising.product add CountryOfOrigin varchar(2) NOT NULL
alter table merchandising.product add VendorWarranty int NULL
alter table merchandising.product add ReplacingTo varchar(20) NULL
alter table merchandising.product add FeatureBenefit varchar(max) NULL

CREATE UNIQUE NONCLUSTERED INDEX [IX_Merchandising_Product] ON [Merchandising].[Product]([SKU] ASC)
ALTER TABLE [Merchandising].[Product] ADD  CONSTRAINT [UQ_Product_SKU] UNIQUE NONCLUSTERED(	[SKU] ASC)