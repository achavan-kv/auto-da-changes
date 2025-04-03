IF EXISTS (SELECT 1 
           FROM dbo.sysobjects 
           WHERE id = object_id('[dbo].[RolesGet]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
DROP PROCEDURE [dbo].RolesGet
GO

CREATE PROCEDURE RolesGet 
@permission INT,
@return INT OUTPUT
AS
	SELECT RoleId,Name FROM Admin.RolePermission
	INNER JOIN Admin.Role ON Admin.RolePermission.RoleId = Admin.Role.Id
	WHERE PermissionId = @permission

    SET @return = 0

GO

