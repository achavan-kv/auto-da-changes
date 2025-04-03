-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Warranty.WarrantySale ALTER COLUMN WarrantyTaxRate DECIMAL(4,2) NULL
ALTER TABLE Warranty.WarrantySale ALTER COLUMN WarrantyCostPrice MONEY NULL
ALTER TABLE Warranty.WarrantySale ALTER COLUMN WarrantyRetailPrice MONEY NULL
ALTER TABLE Warranty.WarrantySale ALTER COLUMN WarrantySalePrice MONEY NULL
