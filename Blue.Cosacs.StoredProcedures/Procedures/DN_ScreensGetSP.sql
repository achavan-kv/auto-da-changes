SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScreensGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScreensGetSP]
GO



CREATE PROCEDURE 	dbo.DN_ScreensGetSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	distinct screen
	FROM		MandatoryFields

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

