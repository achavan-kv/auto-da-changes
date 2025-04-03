-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
UPDATE [Admin].[Permission]
SET [Description] = 'Number Generation - Enables access to the Number Generation screen via the Branch menu'
WHERE [Name] = 'Number Generation'
