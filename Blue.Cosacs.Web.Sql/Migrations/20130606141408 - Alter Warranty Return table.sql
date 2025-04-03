-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
--

ALTER TABLE [Warranty].[WarrantyReturn]
ALTER COLUMN [WarrantyLength] int null;

ALTER TABLE [Warranty].[WarrantyReturn]
ALTER COLUMN [ElapsedMonths] int not null;