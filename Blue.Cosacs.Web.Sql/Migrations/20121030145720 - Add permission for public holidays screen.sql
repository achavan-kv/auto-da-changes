-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT * FROM admin.PermissionCategory WHERE name = 'Configuration')
BEGIN
	INSERT INTO admin.PermissionCategory
	SELECT 15, 'Configuration'
END

IF NOT EXISTS(SELECT * FROM admin.Permission WHERE id = 15001)
BEGIN
	exec Admin.AddPermission 15001, 'Public Holidays', 15, 'Allows access to the Public Holidays screen'
END