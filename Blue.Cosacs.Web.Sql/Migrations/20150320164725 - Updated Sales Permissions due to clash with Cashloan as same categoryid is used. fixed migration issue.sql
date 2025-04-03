-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
delete from [Admin].[RolePermission] where [PermissionId] IN(8000, 8001, 8002, 8003, 8004, 8005, 8006,8007,8008,8009,8010,8011) 

DELETE FROM [Admin].[Permission]
WHERE CategoryId = 80 AND Id IN(8000, 8001, 8002, 8003, 8004, 8005, 8006,8007,8008,8009,8010,8011) 
GO

IF EXISTS(SELECT 1 FROM [Admin].[PermissionCategory] WHERE Id = 80 AND Name = N'Sales') BEGIN
	UPDATE [Admin].[PermissionCategory]  
	SET Id = 88
	WHERE  Id = 80 AND Name = N'Sales'
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[PermissionCategory] WHERE Id = 88) BEGIN
	INSERT INTO [Admin].[PermissionCategory] (Id, Name)
	VALUES (88, N'Sales')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8000 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8000, N'Sales - Sales Menu', 88, N'Allows user access to Sales Menu')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8001 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8001, N'Sales - POS Menu', 88, N'Enables access to the POS screen')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8002 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8002, N'Sales - Search Orders', 88, N'Allow user to search and print old orders')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8003 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8003, N'Sales - Edit Exchange Rate', 88, N'Allow user to edit exchange rate')
END 
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8004 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8004, N'Sales - Change Payment Setup', 88, N'Allow user to change payment setup')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8005 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8005, N'Sales - Change Discount Limit Setup', 88, N'Allow user to change discount limit setup')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8006 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8006, N'Sales - Contracts Setup', 88, N'Allow access to the sales Contracts Setup Screen.')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8007 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8007, N'Sales - Authorise Exchange/Refund', 88, N'Allow user to authorise an exchange or refund on the Point of Sale.')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8008 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8008, N'Sales - Authorise Manual Exchange/Refund', 88, N'Allow user to authorise a manually entered exchange or refund on the Point of Sale.')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8009 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8009, N'Sales - Edit Store Card No', 88, N'Allow user to manually enter/edit store card numbers.')
END
GO

