SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StaffSummaryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StaffSummaryGetSP]
GO





CREATE PROCEDURE 	dbo.DN_StaffSummaryGetSP
			@branchNo smallint ,
			@empeetype varchar(3), --IP - 03/06/08 - Credit Collections - Need to cater for (3) character Employee Types. 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	u.id as EmpeeNo,
			--u.FullName   + ' ' + convert (varchar(8), u.id) as EmployeeName,
            convert (varchar(8), u.id)  + ' : ' + u.FullName as EmployeeName,		-- id first
			branchno,
			u.FullName as name
	FROM Admin.[User] u
	
	WHERE (branchno = @branchNo OR @branchNo = 0)
	AND u.id != 0
	AND EXISTS (SELECT 1 FROM Admin.UserRole ur
	            WHERE ur.UserId = u.id)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

