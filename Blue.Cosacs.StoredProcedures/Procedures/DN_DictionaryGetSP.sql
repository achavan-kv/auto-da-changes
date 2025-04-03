SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DictionaryGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DictionaryGetSP]
GO


CREATE PROCEDURE 	dbo.DN_DictionaryGetSP
			@culture varchar(10),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	English,
			Translation 
	FROM		Dictionary
	WHERE	Culture = @culture

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

