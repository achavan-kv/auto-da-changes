-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM sys.columns 
              WHERE [name] = N'IsSalesCustomer' AND [object_id] = OBJECT_ID(N'Sales.OrderCustomer'))BEGIN
	ALTER TABLE Sales.OrderCustomer ADD
		IsSalesCustomer bit NOT NULL CONSTRAINT DF_OrderCustomer_IsSalesCustomer DEFAULT 1
END
GO