-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE [Admin].[PermissionCategory]
SET Name = N'Sales Client'
WHERE Id = 10
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[PermissionCategory] WHERE Id = 80) BEGIN
	INSERT INTO [Admin].[PermissionCategory] (Id, Name)
	VALUES (80, N'Sales')
END
GO

UPDATE [Admin].[Permission] SET CategoryId = 80
WHERE Id IN(8000, 8001, 8002, 8003, 8004, 8005, 8006) AND CategoryId = 10
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8000 AND CategoryId = 80) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8000, N'Sales - Sales Menu', 80, N'Allows user access to Sales Menu')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8001 AND CategoryId = 80) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8001, N'Sales - POS Menu', 80, N'Enables access to the POS screen')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8002 AND CategoryId = 80) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8002, N'Sales - Search Orders', 80, N'Allow user to search and print old orders')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8003 AND CategoryId = 80) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8003, N'Sales - Edit Exchange Rate', 80, N'Allow user to edit exchange rate')
END 
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8004 AND CategoryId = 80) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8004, N'Sales - Change Payment Setup', 80, N'Allow user to change payment setup')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8005 AND CategoryId = 80) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8005, N'Sales - Change Discount Limit Setup', 80, N'Allow user to change discount limit setup')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8006 AND CategoryId = 80) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8006, N'Sales - Contracts Setup', 80, N'Allow access to the sales Contracts Setup Screen.')
END
GO