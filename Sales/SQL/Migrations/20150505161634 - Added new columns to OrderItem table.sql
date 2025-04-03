-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- IsClaimed
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsClaimed' AND [object_id] = OBJECT_ID(N'Sales.OrderItem')) BEGIN
			  
	IF (OBJECT_ID(N'[Sales].[DF_OrderItem_IsClaimed]') IS NOT NULL) BEGIN
		ALTER TABLE [Sales].[OrderItem] DROP CONSTRAINT [DF_OrderItem_IsClaimed]
	END
	
	ALTER TABLE [Sales].[OrderItem]
	ADD IsClaimed bit NULL
	CONSTRAINT [DF_OrderItem_IsClaimed] DEFAULT 0	
END
GO

-- IsReplacement
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsReplacement' AND [object_id] = OBJECT_ID(N'Sales.OrderItem')) BEGIN
			  
	IF (OBJECT_ID(N'[Sales].[DF_OrderItem_IsReplacement]') IS NOT NULL) BEGIN
		ALTER TABLE [Sales].[OrderItem] DROP CONSTRAINT [DF_OrderItem_IsReplacement]
	END
	
	ALTER TABLE [Sales].[OrderItem]
	ADD IsReplacement bit NULL
	CONSTRAINT [DF_OrderItem_IsReplacement] DEFAULT 0
END
GO


-- SalePrice
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'SalePrice' AND [object_id] = OBJECT_ID(N'Sales.OrderItem')) BEGIN
			  
	IF (OBJECT_ID(N'[Sales].[DF_OrderItem_SalePrice]') IS NOT NULL) BEGIN
		ALTER TABLE [Sales].[OrderItem] DROP CONSTRAINT [DF_OrderItem_SalePrice]
	END
		  
	ALTER TABLE [Sales].[OrderItem]
	ADD SalePrice dbo.BlueAmount NOT NULL
	CONSTRAINT [DF_OrderItem_SalePrice] DEFAULT 0
END
GO

-- SaleTaxAmount
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'SaleTaxAmount' AND [object_id] = OBJECT_ID(N'Sales.OrderItem')) BEGIN
			  
	IF (OBJECT_ID(N'[Sales].[DF_OrderItem_SaleTaxAmount]') IS NOT NULL) BEGIN
		ALTER TABLE [Sales].[OrderItem] DROP CONSTRAINT [DF_OrderItem_SaleTaxAmount]
	END
	
	ALTER TABLE [Sales].[OrderItem]
	ADD SaleTaxAmount dbo.BlueAmount NOT NULL
	CONSTRAINT [DF_OrderItem_SaleTaxAmount] DEFAULT 0
END
GO

