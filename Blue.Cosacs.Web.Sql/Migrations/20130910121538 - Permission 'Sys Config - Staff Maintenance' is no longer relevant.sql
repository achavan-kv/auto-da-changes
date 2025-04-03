-- transaction: true

DECLARE @PermId INT

SELECT @PermId = [Id] 
  FROM [Admin].[Permission]
 WHERE [Name] = 'Sys Config - Staff Maintenance'

IF (@PermId > 0)
BEGIN
  DELETE FROM [Admin].[RolePermission]
   WHERE [PermissionId] = @PermId

  DELETE FROM [Admin].[Permission]
   WHERE [Id] = @PermId
END
