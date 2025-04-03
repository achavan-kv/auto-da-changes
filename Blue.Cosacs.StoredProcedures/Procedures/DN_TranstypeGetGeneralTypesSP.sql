SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TranstypeGetGeneralTypesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TranstypeGetGeneralTypesSP]
GO

CREATE PROCEDURE 	dbo.DN_TranstypeGetGeneralTypesSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT 	transtypecode as code,
			description as codedescript
	FROM	transtype
	WHERE	IncludeInGFT = 1

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

