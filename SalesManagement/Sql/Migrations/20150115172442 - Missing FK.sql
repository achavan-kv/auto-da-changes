-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT 1 FROM [$Migration] m WHERE m.Id IN (20150115172436, 20150115172438, 20150115172440))
BEGIN
	ALTER TABLE SalesManagement.SalesPersonTargets ADD CONSTRAINT
		FK_SalesPersonTargets_User FOREIGN KEY
		(
		CreatedBy
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 

	DELETE SalesManagement.Call
	WHERE SalesPersonId NOT IN (SELECT id FROM Admin.[User])

	DELETE SalesManagement.CustomerSalesPerson
	WHERE SalesPersonId NOT IN (SELECT id FROM Admin.[User])

	ALTER TABLE SalesManagement.CustomerSalesPerson ADD CONSTRAINT
		FK_CustomerSalesPerson_User FOREIGN KEY
		(
		SalesPersonId
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 

	ALTER TABLE SalesManagement.CustomerSalesPerson ADD CONSTRAINT
		FK_CustomerSalesPerson_branch FOREIGN KEY
		(
		CustomerBranch
		) REFERENCES dbo.branch
		(
		branchno
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 

	ALTER TABLE SalesManagement.CustomerSalesPerson ADD CONSTRAINT
		FK_CustomerSalesPersonTempSalesPerson_User FOREIGN KEY
		(
		SalesPersonId
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
		
	ALTER TABLE SalesManagement.CustomerSalesPerson ADD CONSTRAINT
		FK_customer_CustomerSalesPerson FOREIGN KEY
		(
			CustomerId
		) REFERENCES dbo.customer
		(
			custid
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
		
	ALTER TABLE SalesManagement.CsrUnavailable ADD CONSTRAINT
		FK_CsrUnavailable_User FOREIGN KEY
		(
		CreatedBy
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
		
	ALTER TABLE SalesManagement.Call ADD CONSTRAINT
		FK_Call_customer FOREIGN KEY
		(
		CustomerId
		) REFERENCES dbo.customer
		(
		custid
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
		
	ALTER TABLE SalesManagement.Call ADD CONSTRAINT
		FK_Call_User FOREIGN KEY
		(
		CalledBy
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
		
	ALTER TABLE SalesManagement.Call ADD CONSTRAINT
		FK_Call_UserCreatedBy FOREIGN KEY
		(
		CreatedBy
		) REFERENCES Admin.[User]
		(
		Id
		) ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
END