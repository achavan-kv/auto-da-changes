SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_InstalPlanAuditGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_InstalPlanAuditGetSP]
GO

CREATE PROCEDURE 	dbo.DN_InstalPlanAuditGetSP
			@acctno varchar(12),
			@rowcount int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET		ROWCOUNT @rowcount

	SELECT	IP.acctno,
			IP.agrmtno,
			IP.newinstalno,
			IP.oldinstalno,
			IP.newinstalment,
			IP.oldinstalment,
			IP.empeenochange,
			IP.datechange,
			IP.systemusername,
			isnull(u.FullName, 'Unknown') AS EmployeeName
	FROM instalplanaudit IP 
	LEFT OUTER JOIN Admin.[User] u ON u.id = IP.empeenochange
	WHERE	acctno = @acctno
	ORDER BY	IP.datechange DESC

	SET		ROWCOUNT 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

