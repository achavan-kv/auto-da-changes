-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update Admin.Permission
set [Name] = 'Cashier Deposits', 
	[Description] = 'Cashier Deposits - Allows user access to Cashier Deposit screen via the Transaction Menu'
where Id = 158

DELETE FROM Admin.RolePermission
WHERE PermissionId = 302

DELETE FROM Admin.Permission
WHERE Id = 302

