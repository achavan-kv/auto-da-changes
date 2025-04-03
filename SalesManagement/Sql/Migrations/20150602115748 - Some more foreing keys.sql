-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
DELETE SalesManagement.[Call]
WHERE ID IN 
(
	SELECT c.id 
	FROM SalesManagement.[Call] c left join Admin.[User] a on c.SalesPersonId = a.Id
	WHERE a.id is null
)

IF NOT EXISTS (SELECT 1
               FROM sys.foreign_keys 
               WHERE name = N'FK_Call_customer' AND parent_object_id = OBJECT_ID(N'SalesManagement.Call'))
	ALTER TABLE SalesManagement.Call ADD CONSTRAINT
		FK_Call_customer FOREIGN KEY
		(
			CustomerId
		) REFERENCES dbo.customer
		(
			custid
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 

IF NOT EXISTS (SELECT 1
               FROM sys.foreign_keys 
               WHERE name = N'FK_Call_User' AND parent_object_id = OBJECT_ID(N'SalesManagement.Call'))
	ALTER TABLE SalesManagement.Call ADD CONSTRAINT
		FK_Call_User FOREIGN KEY
		(
			SalesPersonId
		) REFERENCES Admin.[User]
		(
			Id
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 
IF NOT EXISTS (SELECT 1
               FROM sys.foreign_keys 
               WHERE name = N'FK_Call_branch' AND parent_object_id = OBJECT_ID(N'SalesManagement.Call'))
	ALTER TABLE SalesManagement.Call ADD CONSTRAINT
		FK_Call_branch FOREIGN KEY
		(
			Branch
		) REFERENCES dbo.branch
		(
			branchno
		) 
		 ON UPDATE  NO ACTION 
		 ON DELETE  NO ACTION 