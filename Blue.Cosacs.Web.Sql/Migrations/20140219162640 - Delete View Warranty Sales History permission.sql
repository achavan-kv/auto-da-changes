-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

DELETE FROM Admin.RolePermission
WHERE PermissionId = 1813

DELETE FROM Admin.Permission
WHERE Id = 1813