SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocGetEmpeeNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocGetEmpeeNoSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocGetEmpeeNoSP
			@acctno varchar(12),
			@empeeno int OUT,
			@empeetype varchar(30) OUT,
			@empeename varchar(101) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET	@empeeno = -1

	SELECT	@empeeno = isnull(f.empeeno, 0),
			@empeename = isnull(u.fullname, ''),
			@empeetype = isnull(r.name, '')
	FROM		follupalloc f 
	inner join Admin.[User] u	ON		f.empeeno = u.id
	INNER JOIN Admin.UserRole ur ON ur.UserId = u.Id
	INNER JOIN Admin.Role r ON r.id = ur.RoleId
	WHERE	isnull(datealloc, '1/1/1900') != '1/1/1900'
	AND		isnull(datealloc, '1/1/1900') <= getdate()
	AND		(isnull(datedealloc, '1/1/1900') = '1/1/1900'
	OR		isnull(datedealloc, '1/1/1900') >= getdate())
	AND		acctno =@acctno


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


SELECT * FROM Admin.Permission
