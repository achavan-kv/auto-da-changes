-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8002) BEGIN
	INSERT INTO Admin.Permission (Id, Name, CategoryId, [Description])
	VALUES (8002, N'Sales - Search Orders', 10, N'Allow user to search and print old orders')
END 
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8003) BEGIN
	INSERT INTO Admin.Permission (Id, Name, CategoryId, [Description])
	VALUES (8003, N'Sales - Edit Exchange Rate', 10, N'Allow user to edit exchange rate')
END 
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8004) BEGIN
	INSERT INTO Admin.Permission (Id, Name, CategoryId, [Description])
	VALUES (8004, N'Sales - Change Payment Setup', 10, N'Allow user to change payment setup')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8005) BEGIN
	INSERT INTO Admin.Permission (Id, Name, CategoryId, [Description])
	VALUES (8005, N'Sales - Change Discount Limit Setup', 10, N'Allow user to change discount limit setup')
END
GO