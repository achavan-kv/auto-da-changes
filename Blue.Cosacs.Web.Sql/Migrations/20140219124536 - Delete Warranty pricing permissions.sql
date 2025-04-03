-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- Delete 'View Warranty Pricing (1807)' permission as they are no longer needed
-- Put your SQL code here

DELETE FROM Admin.RolePermission
WHERE PermissionId = 1807

DELETE FROM Admin.Permission
WHERE Id = 1807
