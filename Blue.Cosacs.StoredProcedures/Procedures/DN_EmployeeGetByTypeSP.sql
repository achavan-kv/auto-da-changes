-- **********************************************************************
-- Title:
-- Developer: David Richardson
-- Date: 2006
-- Purpose: Returns employee details for a specific employee type and branch.

-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/10/09  IP  UAT(909) Return the number of accounts that can be allocated to an employee 
-- **********************************************************************



SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmployeeGetByTypeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmployeeGetByTypeSP]
GO

CREATE PROCEDURE 	dbo.DN_EmployeeGetByTypeSP
			@employeeType varchar(3), --IP - 20/05/08 - Credit Collections requires system to cater for (3) character employee type, was originally (1)
			@branchNo smallint ,		
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	id,
			FullName AS EmployeeName,
	        u.ExternalLogin,
	        case when(maxaccounts - alloccount) < 0 then 0 else  (maxaccounts - alloccount) end as NoCanAllocate --IP - 09/10/09 - UAT(909)
	FROM Admin.[User] u
	INNER JOIN dbo.courtsperson c ON c.userid = u.id
	INNER JOIN Admin.UserRole ur ON ur.UserId = u.id
	WHERE	roleid = @employeeType
	AND		(u.branchno = @branchNo or @branchno =0)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

