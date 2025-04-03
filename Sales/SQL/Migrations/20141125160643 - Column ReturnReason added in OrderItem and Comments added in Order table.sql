IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'Comments' AND [object_id] = OBJECT_ID(N'Sales.Order'))BEGIN
	ALTER TABLE Sales.[Order] ADD
		Comments varchar(1024) NULL
END
GO
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ReturnReason' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.[OrderItem] ADD
		ReturnReason varchar(32) NULL
END
GO
