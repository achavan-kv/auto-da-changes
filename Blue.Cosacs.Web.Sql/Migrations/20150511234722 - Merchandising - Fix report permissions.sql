-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
delete from admin.RolePermission where PermissionId = 2167
delete from admin.Permission where Id = 2167
insert into admin.Permission (CategoryId, Id, Name, Description) values (20, 2041, 'Report - Top Sku', 'Grants access to view the Top Sku Report')
insert into admin.Permission (CategoryId, Id, Name, Description) values (20, 2042, 'Report - Stock Valuation', 'Grants access to view the Stock Valuation Report')
