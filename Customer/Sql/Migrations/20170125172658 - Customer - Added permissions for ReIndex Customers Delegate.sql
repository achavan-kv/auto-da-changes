-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Re-index customer delegate
INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description], IsDelegate)
SELECT 2601, 'xx', CategoryId, [Description], IsDelegate 
FROM [Admin].[Permission]
WHERE Id = 2600

UPDATE [Admin].[Permission]
SET IsDelegate = 1, Name = 'Re-index customer delegate' 
WHERE Id = 2600
GO

UPDATE [Admin].[Permission]
SET Name = 'Re-index customer' 
WHERE Id = 2601
GO

--  Re-index customer
INSERT INTO [Admin].[PermissionDelegate] (MainPermission, DelegatePermission)
SELECT 2601, 2600