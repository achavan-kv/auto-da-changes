-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete admin.RolePermission
where permissionid in(353,354)

delete admin.Permission
where id in(353,354)


