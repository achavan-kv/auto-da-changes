-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF (SELECT COUNT(*) FROM [Admin].PermissionCategory WHERE Name = 'Cosacs') = 0
	INSERT INTO [Admin].PermissionCategory
		(Name)
	VALUES
		('Cosacs')
GO

IF (SELECT COUNT(*) FROM [Admin].Permission WHERE id = 379) = 0
	INSERT INTO [Admin].Permission
		(id, Name, CategoryId, [Description])
	SELECT 
		379 AS id,
		'Lock User' AS Name, 
		id AS CategoryId,
		'User can lock another users' AS [Description]
	FROM 
		[Admin].PermissionCategory
	WHERE
		Name = 'System Administration'
