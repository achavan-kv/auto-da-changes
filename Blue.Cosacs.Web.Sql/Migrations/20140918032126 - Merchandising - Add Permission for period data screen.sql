-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2105, 21, 'Period Data View', 'Allows user to view the period data setup')


INSERT INTO [Admin].[Permission](Id, CategoryId, Name, [Description])
VALUES (2106, 21, 'Period Data Edit', 'Allows user to edit the period data setup')