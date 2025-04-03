SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmployeeGetCommissionDetailsByTypeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmployeeGetCommissionDetailsByTypeSP]
GO

CREATE PROCEDURE 	dbo.DN_EmployeeGetCommissionDetailsByTypeSP
			@branchno int,
			@empeetype varchar(3), --IP - 03/06/08 - Credit Collections - Need to cater for (3) character Employee Types.
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	u.branchno,
		u.id AS EmpeeNo,
		FullName AS EmployeeName,
		commndue,
		lstcommn
	FROM	courtsperson c
	INNER JOIN Admin.[User] u ON u.id = c.userid
	INNER JOIN Admin.UserRole ur ON ur.UserId = u.id
	WHERE	roleid = @empeetype
	AND	u.branchno = @branchno
    ORDER BY u.id asc --IP - 06/05/08 - UAT(330) v 5.1 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO