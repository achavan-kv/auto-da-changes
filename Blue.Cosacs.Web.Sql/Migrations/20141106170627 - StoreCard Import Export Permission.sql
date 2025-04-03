-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


INSERT INTO [Admin].[PermissionCategory]
VALUES (70, 'Store Card')


INSERT INTO [Admin].[Permission]
VALUES (7000, 'Credit - Third Party Store Card Import/Export', 70, 'Allows the user to Export ACTIVE Store Cards and Import transactions processed by the Third parties')