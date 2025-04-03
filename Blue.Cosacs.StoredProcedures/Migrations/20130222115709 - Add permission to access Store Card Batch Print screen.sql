-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from admin.Permission where id =1201)
BEGIN
	DELETE FROM admin.RolePermission WHERE PermissionId = 1201
	DELETE FROM admin.Permission WHERE id = 1201
	
END

IF NOT EXISTS(select * from admin.Permission where id =1204)
BEGIN
	exec Admin.AddPermission 1204, 'Sys Config - Store Card Batch Print', 12, 'Allows access to the Store Card Batch Print screen'
END

