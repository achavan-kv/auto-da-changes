/*
User View for Cosacs including columns from CourtsPerson

*/


IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'V'
		   AND so.NAME = 'UserRoleView'
		   AND ss.name = 'dbo')
DROP VIEW  dbo.[UserRoleView]
GO


CREATE VIEW  dbo.[UserRoleView]
AS
SELECT u.*, c.*,ur.RoleId,r.[Name]
FROM Admin.[User] u INNER JOIN CourtsPersontable c on u.Id=c.UserId
		INNER JOIN Admin.UserRole ur on u.Id=ur.userId
		INNER JOIN Admin.Role r on ur.RoleId=r.Id
GO