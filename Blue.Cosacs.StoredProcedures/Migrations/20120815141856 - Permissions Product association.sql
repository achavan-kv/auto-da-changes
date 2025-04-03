-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @category INT
	select @category= id from admin.PermissionCategory where Name='Cosacs'
	
exec Admin.AddPermission 372, 'Product Associations', @category, 'Enable access to the ''Associations'' screen via the Warehouse menu - Warehouse and Deliveries'

