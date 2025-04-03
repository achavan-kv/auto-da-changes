IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'WarrantyTypeCode' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		WarrantyTypeCode char(1) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ManualDiscountPercentage' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		ManualDiscountPercentage nvarchar(12) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ProductItemId' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		ProductItemId int NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'WarrantyLinkId' AND [object_id] = OBJECT_ID(N'Sales.OrderItem'))BEGIN
	ALTER TABLE Sales.OrderItem ADD
		WarrantyLinkId int NULL
END
GO