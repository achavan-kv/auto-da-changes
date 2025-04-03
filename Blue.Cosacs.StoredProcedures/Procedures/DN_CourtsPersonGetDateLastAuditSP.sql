SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CourtsPersonGetDateLastAuditSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CourtsPersonGetDateLastAuditSP]
GO

CREATE PROCEDURE 	dbo.DN_CourtsPersonGetDateLastAuditSP
			@empeeno int,
			@dateLast datetime OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@dateLast = datelstaudit
	FROM		courtsperson 
	WHERE	userid = @empeeno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

