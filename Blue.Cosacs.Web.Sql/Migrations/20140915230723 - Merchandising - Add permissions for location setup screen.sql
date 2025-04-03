-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2101, 21, 'Location View', 'Allows user to view the location setup')


INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2102, 21, 'Location Edit', 'Allows user to create new locations or edit the location setup')