-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 

DELETE FROM Admin.RolePermission 
WHERE PermissionId IN (8003, 8004)

DELETE FROM Admin.Permission
WHERE id IN (8003, 8004)