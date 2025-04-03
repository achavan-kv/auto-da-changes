-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


declare @category INT
	select @category= id from admin.PermissionCategory where Name='Warehouse'
	
exec Admin.AddPermission 1426, 'Print Customer Pickup Note', @category, 'Allows user to print Customer PickUp Note'
	
exec Admin.AddPermission 1427, 'Reprint Customer Pickup Note', @category, 'Allows user to reprint Customer PickUp Note'


