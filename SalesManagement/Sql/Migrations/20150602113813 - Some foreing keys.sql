-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
DELETE SalesManagement.CustomerSalesPerson
WHERE CustomerId IN 
(
	select c.CustomerId 
	from SalesManagement.CustomerSalesPerson c left join Admin.[User] a on c.SalesPersonId = a.Id
	where a.id is null
)

IF NOT EXISTS (SELECT 1
               FROM sys.foreign_keys 
               WHERE name = N'FK_CsrUnavailable_CreatedBy' AND parent_object_id = OBJECT_ID(N'SalesManagement.CsrUnavailable'))
	ALTER TABLE SalesManagement.CsrUnavailable ADD CONSTRAINT
		FK_CsrUnavailable_CreatedBy FOREIGN KEY
		(
			CreatedBy
		) REFERENCES Admin.[User]
		(
			Id
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
	

IF NOT EXISTS (SELECT 1
               FROM sys.foreign_keys 
               WHERE name = N'FK_CustomerSalesPerson_customer' AND parent_object_id = OBJECT_ID(N'SalesManagement.CustomerSalesPerson'))
	ALTER TABLE SalesManagement.CustomerSalesPerson ADD CONSTRAINT
		FK_CustomerSalesPerson_customer FOREIGN KEY
		(
			CustomerId
		) REFERENCES dbo.customer
		(
			custid
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 