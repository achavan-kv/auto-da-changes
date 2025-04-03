IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ManualDiscount' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		ManualDiscount dbo.BlueAmount NULL
END
GO
