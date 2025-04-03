-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2121, 21, 'Repossessed Conditions View', 'Allows user to view the list of repossessed conditions')


INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2122, 21, 'Repossessed Conditions Edit', 'Allows user to edit the list of repossessed conditionp')