DECLARE @role INT

SELECT @role =RoleId FROM Admin.UserRole
WHERE userid = 99999

IF NOT EXISTS (SELECT * FROM Admin.RolePermission
               WHERE RoleId = @role 
               AND PermissionId = 100000)
BEGIN
INSERT INTO Admin.RolePermission
        ( RoleId, PermissionId, [Deny] )
VALUES (@role, 100000, 0)
END