-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Table 'StockInfo' changes
IF NOT EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'WarrantyType' AND [object_id] = OBJECT_ID(N'[dbo].[StockInfo]')) BEGIN
	ALTER TABLE [dbo].[StockInfo] ADD WarrantyType char(1) NOT NULL CONSTRAINT DF_StockInfo_WarrantyType DEFAULT 'E'
END
GO

IF EXISTS(SELECT * FROM sys.columns 
        WHERE [name] = N'WarrantyIsFree' AND [object_id] = OBJECT_ID(N'[dbo].[StockInfo]')) BEGIN
	UPDATE [dbo].[StockInfo] 
	SET WarrantyType = (CASE WHEN [WarrantyIsFree] = 1 THEN 'F' ELSE 'E' END)
	
	ALTER TABLE [dbo].[StockInfo] DROP COLUMN WarrantyIsFree
END
GO