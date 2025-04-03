-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'CustomerId' AND [object_id] = OBJECT_ID(N'Sales.OrderCustomer'))
BEGIN
	ALTER TABLE [Sales].[OrderCustomer]
	ADD CustomerId VARCHAR(20)
END
GO

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'AddressLine3' AND [object_id] = OBJECT_ID(N'Sales.OrderCustomer'))
BEGIN
	ALTER TABLE [Sales].[OrderCustomer]
	ADD AddressLine3 VARCHAR(64)
END
GO

