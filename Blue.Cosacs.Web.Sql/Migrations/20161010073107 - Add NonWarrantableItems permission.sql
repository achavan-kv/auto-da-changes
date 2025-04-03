-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO [Admin].[Permission]
SELECT 2168, 'Non Warrantable Items', 21, 'Allows the user to manage items which do not attract warranties', 0