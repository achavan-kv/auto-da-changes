-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

delete from admin.Permission where id in (2107, 2108, 2109)

INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2107, 21, 'Tag Values View', 'Allows user to view the Tag values setup')

INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2108, 21, 'Tag Condition Edit', 'Allows user to modify Tag condition values')

INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2109, 21, 'Tag FYW Edit', 'Allows user to modify Tag First Year Warranty value')