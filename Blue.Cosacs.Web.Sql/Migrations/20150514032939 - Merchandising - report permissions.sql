-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
insert into admin.Permission
(CategoryId, Id, Name, Description)
values
(20, 2044, 'Report - Allocated Stock', 'Grants access to view the Allocated Stock Report')