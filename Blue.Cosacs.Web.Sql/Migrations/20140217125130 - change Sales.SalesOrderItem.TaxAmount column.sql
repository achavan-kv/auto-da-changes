-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Sales.SalesOrderItem
	DROP COLUMN TaxAmount 
GO

ALTER TABLE Sales.SalesOrderItem
	ADD TaxAmount AS CONVERT(decimal(19,3), round(UnityPrice * (TaxRate/(100.00)), 3, 1), 0)