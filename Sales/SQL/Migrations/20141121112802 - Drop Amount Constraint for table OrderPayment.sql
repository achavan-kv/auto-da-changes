
IF (OBJECT_ID(N'[Sales].[CK_OrderPayment_Amount]') IS NOT NULL) BEGIN
	ALTER TABLE [Sales].[OrderPayment] DROP CONSTRAINT [CK_OrderPayment_Amount]
END
GO