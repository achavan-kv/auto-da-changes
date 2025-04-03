-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.


IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'OriginalId' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		OriginalId int NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'Exchanged' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		Exchanged bit NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ManualItemId' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		ManualItemId int NULL
END
GO

