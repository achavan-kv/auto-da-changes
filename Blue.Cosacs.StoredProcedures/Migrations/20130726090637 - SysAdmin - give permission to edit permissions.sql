DECLARE @role INT

SELECT @role = RoleId FROM Admin.[UserRole]
WHERE UserId = 99999

IF NOT EXISTS (SELECT * FROM Admin.RolePermission
               WHERE RoleId = @role 
               AND PermissionId = 1205)
BEGIN
INSERT INTO Admin.RolePermission
        ( RoleId, PermissionId, [Deny] )
VALUES (@role, 1205, 0)
END
