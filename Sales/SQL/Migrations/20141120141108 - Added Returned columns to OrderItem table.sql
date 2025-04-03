IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'Returned' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		Returned bit NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ReturnQuantity' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		ReturnQuantity smallint NULL
END
GO

IF (OBJECT_ID(N'[Sales].[DF_OrderItem_Returned]') IS NOT NULL) BEGIN
	ALTER TABLE Sales.OrderItem ADD CONSTRAINT
		DF_OrderItem_Returned DEFAULT 0 FOR Returned
END
GO

IF (OBJECT_ID(N'[Sales].[DF_OrderItem_ReturnQuantity]') IS NOT NULL) BEGIN
	ALTER TABLE Sales.OrderItem ADD CONSTRAINT
		DF_OrderItem_ReturnQuantity DEFAULT 0 FOR ReturnQuantity
END
GO