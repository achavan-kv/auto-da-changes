SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_MandatoryFieldsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_MandatoryFieldsGetSP]
GO





CREATE PROCEDURE 	dbo.DN_MandatoryFieldsGetSP
			@country varchar(2),
			@screen varchar(50),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	country,
			screen,
			control,
			description,
			enabled,
			visible,
			mandatory
	FROM		MandatoryFields
	WHERE	country = @country
	AND		screen = @screen

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

