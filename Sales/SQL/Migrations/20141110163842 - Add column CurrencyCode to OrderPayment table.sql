IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'CurrencyCode' AND [object_id] = OBJECT_ID(N'Sales.OrderPayment')) BEGIN
	ALTER TABLE Sales.OrderPayment ADD
		CurrencyCode nvarchar(8) NULL
END