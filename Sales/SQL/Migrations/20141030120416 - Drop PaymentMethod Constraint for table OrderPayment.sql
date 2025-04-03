-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF (OBJECT_ID(N'[Sales].[FK_OrderPayment_PaymentMethod]') IS NOT NULL) BEGIN
PRINT 'h'
	ALTER TABLE Sales.OrderPayment
		DROP CONSTRAINT FK_OrderPayment_PaymentMethod
END

GO