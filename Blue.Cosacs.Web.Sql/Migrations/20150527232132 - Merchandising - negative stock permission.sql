-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
delete from admin.RolePermission where PermissionId = 2050
delete from admin.Permission where name = 'Report - Negative Stock'
insert into admin.Permission (CategoryId, Id, Name, Description) 
values (20, 2050, 'Report - Negative Stock', 'Grants access to view the Negative Stock Report')
