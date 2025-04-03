-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT EXISTS (SELECT 1 FROM Admin.Permission
			   WHERE id = 8812)
BEGIN
	INSERT INTO [Admin].[Permission] (Id, Name, CategoryId, [Description], IsDelegate)
	VALUES (8812, 'Sales - Create Orders', 88, 'Allow users to create new orders (POS/Sales)', 0)
END
GO