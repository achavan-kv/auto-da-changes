SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TermsTypeAccountTypesDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeAccountTypesDeleteSP]
GO


CREATE PROCEDURE 	dbo.DN_TermsTypeAccountTypesDeleteSP
			@termstype varchar(4),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE	
	FROM		TermsTypeAccountType
	WHERE	termstype = @termstype

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO


SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

