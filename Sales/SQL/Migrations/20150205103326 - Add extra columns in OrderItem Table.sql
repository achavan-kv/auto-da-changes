-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ItemUPC' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN
	ALTER TABLE [Sales].[OrderItem]
	ADD ItemUPC VARCHAR(18) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ItemSupplier' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN
	ALTER TABLE [Sales].[OrderItem]
	ADD ItemSupplier VARCHAR(40) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'Level_1' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN
	ALTER TABLE [Sales].[OrderItem]
	ADD Level_1 VARCHAR(12) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'Level_2' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN
	ALTER TABLE [Sales].[OrderItem]
	ADD Level_2 smallint NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'Level_3' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN
	ALTER TABLE [Sales].[OrderItem]
	ADD Level_3 CHAR(3) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'CostPrice' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN
	ALTER TABLE [Sales].[OrderItem]
	ADD CostPrice money NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'RetailPrice' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))
BEGIN
	ALTER TABLE [Sales].[OrderItem]
	ADD RetailPrice money NULL
END
GO