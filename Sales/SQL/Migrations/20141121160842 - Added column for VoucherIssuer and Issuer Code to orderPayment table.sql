-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'VoucherIssuer' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment'))BEGIN
	ALTER TABLE Sales.OrderPayment ADD
		VoucherIssuer char(1) NULL
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'VoucherIssuerCode' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment')) BEGIN
	ALTER TABLE Sales.OrderPayment ADD
		VoucherIssuerCode char(12) NULL
END
GO