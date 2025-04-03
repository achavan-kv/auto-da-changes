-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Table 'WarrantySale' changes
IF NOT EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'WarrantyType' AND [object_id] = OBJECT_ID(N'[Warranty].[WarrantySale]')) BEGIN
	ALTER TABLE [Warranty].[WarrantySale] ADD WarrantyType char(1) NOT NULL CONSTRAINT DF_WarrantySale_WarrantyType DEFAULT 'E'
END
GO

IF EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'WarrantyIsFree' AND [object_id] = OBJECT_ID(N'[Warranty].[WarrantySale]')) BEGIN
	UPDATE [Warranty].[WarrantySale] 
	SET WarrantyType = (CASE WHEN [WarrantyIsFree] = 1 THEN 'F' ELSE 'E' END)
	
	ALTER TABLE [Warranty].[WarrantySale] DROP COLUMN WarrantyIsFree
END
GO

-- Table 'WarrantyPotentialSale' changes
IF NOT EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'WarrantyType' AND [object_id] = OBJECT_ID(N'[Warranty].[WarrantyPotentialSale]')) BEGIN
	ALTER TABLE [Warranty].[WarrantyPotentialSale] ADD WarrantyType char(1) NOT NULL CONSTRAINT DF_WarrantyPotentialSale_WarrantyType DEFAULT 'E'
END
GO

IF EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'WarrantyIsFree' AND [object_id] = OBJECT_ID(N'[Warranty].[WarrantyPotentialSale]')) BEGIN
	UPDATE [Warranty].[WarrantyPotentialSale] 
	SET WarrantyType = (CASE WHEN [WarrantyIsFree] = 1 THEN 'F' ELSE 'E' END)
	
	ALTER TABLE [Warranty].[WarrantyPotentialSale] DROP COLUMN WarrantyIsFree
END
GO
