
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'StoreCardNumber' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment')) BEGIN
	ALTER TABLE Sales.OrderPayment ADD
		StoreCardNumber bigint NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'VoucherNumber' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment')) BEGIN
	ALTER TABLE Sales.OrderPayment ADD
		VoucherNumber nvarchar(32) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ChequeNumber' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment')) BEGIN
	ALTER TABLE Sales.OrderPayment ADD
		ChequeNumber nvarchar(32) NULL
END
GO