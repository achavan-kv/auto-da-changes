SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountDeleteCodeFromSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountDeleteCodeFromSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountDeleteCodeFromSP
			@acctNo varchar(12),
			@code varchar(4),
			@reference varchar(10),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM		acctcode
	WHERE	acctno = @acctNo
	AND		code = @code
	AND     reference = @reference

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

