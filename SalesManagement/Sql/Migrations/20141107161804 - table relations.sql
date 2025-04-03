-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
ALTER TABLE SalesManagement.CustomerSalesPerson ADD CONSTRAINT
	FK_CustomerSalesPerson_SalesPerson FOREIGN KEY
	(
	SalesPersonId
	) REFERENCES SalesManagement.SalesPerson
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 

ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	FK_Call_CustomerSalesPerson FOREIGN KEY
	(
	CustomerId
	) REFERENCES SalesManagement.CustomerSalesPerson
	(
	CustomerId
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
ALTER TABLE SalesManagement.Call ADD CONSTRAINT
	FK_Call_CallCloseReason FOREIGN KEY
	(
	CallClosedReasonId
	) REFERENCES SalesManagement.CallCloseReason
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
