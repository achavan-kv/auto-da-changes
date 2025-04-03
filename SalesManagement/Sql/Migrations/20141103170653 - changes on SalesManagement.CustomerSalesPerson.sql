-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE SalesManagement.CustomerSalesPerson
DROP COLUMN TimeFrameTempSalesPerson

ALTER TABLE SalesManagement.CustomerSalesPerson
	ADD TempSalesPersonIdBegin Date NULL

	
ALTER TABLE SalesManagement.CustomerSalesPerson
	ADD TempSalesPersonIdEnd Date NULL