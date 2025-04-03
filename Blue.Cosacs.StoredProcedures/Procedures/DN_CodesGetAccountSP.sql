SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetAccountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetAccountSP]
GO






CREATE PROCEDURE 	dbo.DN_CodesGetAccountSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	code as Code,
			codedescript as Description
	FROM		code
	WHERE	category in ('AC1','AC2')

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

