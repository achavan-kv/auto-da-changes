SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BDWPendingAcceptForWOSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BDWPendingAcceptForWOSP]
GO


CREATE PROCEDURE 	dbo.DN_BDWPendingAcceptForWOSP
			@acctno varchar(12),
			@user int,
			@exists int OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	SET  	@exists = 0

	UPDATE	BDWPending
	SET		empeeno = @user
	WHERE	acctno = @acctno

	IF(@@rowcount > 0)
	BEGIN
		SET @exists = 1
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

