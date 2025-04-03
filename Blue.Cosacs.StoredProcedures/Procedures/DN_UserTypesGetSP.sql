SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UserTypesGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UserTypesGetSP]
GO





CREATE PROCEDURE 	dbo.DN_UserTypesGetSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	code,
			codedescript
	FROM		code
	WHERE	category = 'ET1'
	AND		statusflag = 'L'

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

