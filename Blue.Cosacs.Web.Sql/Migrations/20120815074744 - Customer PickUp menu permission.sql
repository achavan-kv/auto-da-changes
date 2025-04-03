-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @category INT
	select @category= id from admin.PermissionCategory where Name='Warehouse'
	
exec Admin.AddPermission 1425, 'Customer Pick Up', @category, 'Allows access to the Customer Pick Up screen'
	
