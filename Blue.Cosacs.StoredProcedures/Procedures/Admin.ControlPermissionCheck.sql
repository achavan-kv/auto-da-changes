
if exists (select * from dbo.sysobjects where id = object_id('Admin.CheckControlPermission') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure Admin.CheckControlPermission
GO

CREATE PROCEDURE Admin.CheckControlPermission
			@login varchar(256),
			@screen varchar(50),
			@control VARCHAR(50)

AS

	SELECT Distinct u.Id
	FROM control c
	INNER JOIN admin.UserPermissionsView up on c.Taskid = up.PermissionId
	INNER JOIN Admin.[User] u ON u.Id = up.UserId
	WHERE u.Login = @login
	AND	 c.Screen = @screen
	AND c.control = @control
	AND Visible = 1
GO
