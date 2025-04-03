SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_MandatoryFieldsSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_MandatoryFieldsSaveSP]
GO





CREATE PROCEDURE 	dbo.DN_MandatoryFieldsSaveSP
			@country varchar(2),
			@screen varchar(50),
			@control varchar(200),
			@description varchar(50),	
			@enabled smallint,
			@visible smallint,
			@mandatory smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	MandatoryFields
	SET		country = @country,
			screen = @screen,
			control = @control,
			description = @description,
			enabled = @enabled,
			visible = @visible,
			mandatory = @mandatory
	WHERE	country = @country
	AND		screen = @screen
	AND		control = @control

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

