SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountDeleteCodesFromSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountDeleteCodesFromSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountDeleteCodesFromSP
			@acctNo varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM		acctcode
	WHERE	acctno = @acctNo

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

