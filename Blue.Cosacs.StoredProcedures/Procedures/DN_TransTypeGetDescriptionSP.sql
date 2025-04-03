SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TransTypeGetDescriptionSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TransTypeGetDescriptionSP]
GO

CREATE PROCEDURE 	dbo.DN_TransTypeGetDescriptionSP
			@transtypecode varchar(3),
			@description varchar(80) OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@description = description
	FROM		transtype
	WHERE	transtypecode = @transtypecode

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

