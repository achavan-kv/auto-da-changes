
IF EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'VoucherNumber' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment'))BEGIN

	EXECUTE sp_rename N'Sales.OrderPayment.VoucherNumber', N'Tmp_VoucherNo_1', 'COLUMN' 

	EXECUTE sp_rename N'Sales.OrderPayment.Tmp_VoucherNo_1', N'VoucherNo', 'COLUMN' 

END
GO

IF EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'StoreCardNumber' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment'))BEGIN

	EXECUTE sp_rename N'Sales.OrderPayment.StoreCardNumber', N'Tmp_StoreCardNo', 'COLUMN'

	EXECUTE sp_rename N'Sales.OrderPayment.Tmp_StoreCardNo', N'StoreCardNo', 'COLUMN'

END
GO

IF EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'ChequeNumber' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment'))BEGIN
	ALTER TABLE Sales.OrderPayment
		DROP COLUMN ChequeNumber
END
GO