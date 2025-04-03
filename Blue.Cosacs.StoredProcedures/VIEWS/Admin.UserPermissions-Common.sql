-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF NOT Object_Id('Admin.UserPermissionsView') IS NULL 
	DROP VIEW Admin.UserPermissionsView
GO
/*Old view*/
IF NOT Object_Id('Admin.UserPermissions') IS NULL 
	DROP VIEW Admin.UserPermissions
GO

CREATE VIEW Admin.UserPermissionsView
AS
	SELECT 
		RP.PermissionId, 
		UR.UserId
	FROM  
		Admin.RolePermission RP 
		INNER JOIN Admin.UserRole ur 
			ON RP.RoleId = ur.RoleId AND RP.[Deny] = 0 
		LEFT JOIN 
		(
			SELECT RP2.PermissionId, UR2.UserId, RP2.[Deny]
			FROM Admin.RolePermission RP2 
			INNER JOIN Admin.UserRole ur2 ON RP2.RoleId = ur2.RoleId AND RP2.[Deny] = 1
		) as D
			ON RP.PermissionId = D.PermissionId
			AND UR.UserId = D.UserId
	WHERE 
		D.PermissionId IS  NULL
	GROUP BY 
		RP.PermissionId, 
		UR.UserId
GO




