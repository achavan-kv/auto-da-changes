SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_EmployerGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_EmployerGetSP]
GO

CREATE PROCEDURE 	dbo.DN_EmployerGetSP
			@custid varchar(20),
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	SELECT	EmpName AS EmployeeName,
			EmpAddr1,
			EmpAddr2,
			EmpCity,
			EmpDept
	FROM 	proposal
	WHERE 	custid = @custid
	AND		acctno = @acctno

	SET	@return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

