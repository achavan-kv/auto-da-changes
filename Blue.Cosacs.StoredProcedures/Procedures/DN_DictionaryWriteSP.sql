SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DictionaryWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DictionaryWriteSP]
GO


CREATE PROCEDURE 	dbo.DN_DictionaryWriteSP
			@culture varchar(10),
			@english varchar(300),
			@translation ntext,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	/* if the translation is blank, delete any exisiting translation */
	IF(len(CAST(@translation as varchar)) = 0)
	BEGIN
		DELETE
		FROM		dictionary
		WHERE	culture = @culture
		AND		english = @english
	END
	ELSE
	BEGIN
		UPDATE	Dictionary
		SET		Translation = @translation
		WHERE	Culture = @culture
		AND		English = @english

		IF(@@rowcount=0)
		BEGIN
			INSERT
			INTO		Dictionary(Culture, English, Translation)
			VALUES	(@culture, @english, @translation)
		END
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

