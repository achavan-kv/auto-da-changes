-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from admin.RolePermission where permissionId in( 
	select Id from admin.permission where name = 'Report - Summary Update Control Report')
delete from admin.permission where name = 'Report - Summary Update Control Report'
insert into admin.Permission (CategoryId, Id, Name, Description) 
values (20, 2052, 'Report - Summary Update Control Report', 'Grants access to view the Summary Update Control Report')