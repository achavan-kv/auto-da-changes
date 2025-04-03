-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Delete admin.rolePermission where permissionid=1419

Delete admin.Permission where id=1419

