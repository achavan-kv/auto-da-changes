-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
INSERT INTO Admin.Permission
(Id, Name, CategoryId, Description)
SELECT 
	1639 AS Id,
	'Resolution Maintenance - Edit' AS Name,
	id AS CategoryId,
	'Admin - Allows user to create/maintain Service Resolutions' AS Description
FROM 
	Admin.PermissionCategory
WHERE 
	Name = 'Service'