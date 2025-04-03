-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @category INT
	select @category= id from admin.PermissionCategory where Name='System Administration'
	
exec Admin.AddPermission 391, 'Users - Search', @category, 'Search users'
exec Admin.AddPermission 392, 'Sys Config - Search', @category, 'Search roles'


INSERT INTO Admin.RolePermission
	(RoleId, PermissionId, [Deny])
SELECT 
	 r.RoleId,
	 392 AS PermissionId,
	 0 AS [Deny]
FROM 
	Admin.[User] u 
	INNER JOIN Admin.UserRole r
		ON u.Id = r.UserId
		AND u.Login = '99999'

