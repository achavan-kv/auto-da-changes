IF (OBJECT_ID(N'[Sales].[FK_Order_OrderCustomer]') IS NOT NULL) BEGIN
    ALTER TABLE Sales.[Order]
	DROP CONSTRAINT FK_Order_OrderCustomer
END

GO

IF (OBJECT_ID(N'[Sales].[FK_OrderCustomer_Order]') IS NOT NULL) BEGIN
	ALTER TABLE Sales.[OrderCustomer]
		DROP CONSTRAINT FK_OrderCustomer_Order
END

GO
ALTER TABLE Sales.OrderCustomer SET (LOCK_ESCALATION = TABLE)
GO

ALTER TABLE Sales.OrderCustomer ADD CONSTRAINT
	FK_OrderCustomer_Order FOREIGN KEY
	(
	OrderId
	) REFERENCES Sales.[Order]
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO
ALTER TABLE Sales.[Order] SET (LOCK_ESCALATION = TABLE)

