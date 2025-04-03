
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'OriginalOrderId' AND [object_id] = OBJECT_ID(N'Sales.Order'))BEGIN
	ALTER TABLE Sales.[Order] ADD
		OriginalOrderId int NULL
END
GO

IF (OBJECT_ID(N'[Sales].[FK_Order_Order]') IS NULL) BEGIN
	ALTER TABLE Sales.[Order] ADD CONSTRAINT
		FK_Order_Order FOREIGN KEY
		(
		OriginalOrderId
		) REFERENCES Sales.[Order]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
END
GO