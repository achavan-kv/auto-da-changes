-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsFixedDiscount' AND [object_id] = OBJECT_ID(N'Sales.OrderItem')) BEGIN
			  
	IF (OBJECT_ID(N'[Sales].[DF_OrderItem_IsFixedDiscount]') IS NOT NULL) BEGIN
		ALTER TABLE [Sales].[OrderItem] DROP CONSTRAINT [DF_OrderItem_IsFixedDiscount]
	END
		  
	ALTER TABLE [Sales].[OrderItem]
	ADD IsFixedDiscount bit NOT NULL
	CONSTRAINT [DF_OrderItem_IsFixedDiscount] DEFAULT 0
END
GO