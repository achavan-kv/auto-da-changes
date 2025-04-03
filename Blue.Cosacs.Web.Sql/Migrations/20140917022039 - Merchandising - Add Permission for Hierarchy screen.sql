-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2103, 21, 'Hierarchy View', 'Allows user to view the hierachy setup')


INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2104, 21, 'Hierarchy Edit', 'Allows user to edit the hierarchy setup')