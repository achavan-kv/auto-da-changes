-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8010 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8010, N'Sales - Reprint Order Receipts', 88, N'Allow user to reprint order receipts.')
END
GO

IF NOT EXISTS(SELECT 1 FROM  [Admin].[Permission] WHERE Id = 8011 AND CategoryId = 88) BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description])
	VALUES (8011, N'Sales - Authorise Discount Limit', 88, N'Authorise uers to pass the Discount limit.')
END
GO

Update [Admin].[Permission] 
Set [Description] = N'Allow user to search old orders'
WHERE Id = 8002 AND CategoryId = 88
