DECLARE @exrole int

SELECT @exrole = id FROM admin.role 
			 WHERE NAME = (SELECT codedescript FROM dbo.code
			  WHERE code = 'z'
			  AND category = 'et1')

UPDATE Admin.[User]
SET Locked = 1
FROM Admin.UserRole 
INNER JOIN Admin.Role R ON Admin.UserRole.RoleId = R.Id
WHERE Admin.[User].Id = Admin.UserRole.UserId
AND R.id = @exrole


DELETE FROM Admin.UserRole
WHERE RoleId = @exrole

DELETE FROM Admin.Role
WHERE Admin.Role.Id = @exrole