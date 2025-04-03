SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PropResultSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PropResultSaveSP]
GO





CREATE PROCEDURE 	dbo.DN_PropResultSaveSP
			@acctno varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	propresult
	SET		acctno = @acctno
	WHERE	acctno = @acctno

	IF(@@rowcount=0)
	BEGIN
		INSERT
		INTO	propresult		--allow everything except acctno to be populated
			(acctno)			--with default value
		VALUES
			(@acctno)	
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

