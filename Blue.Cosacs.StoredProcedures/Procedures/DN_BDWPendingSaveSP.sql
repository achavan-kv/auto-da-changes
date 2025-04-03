SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BDWPendingSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BDWPendingSaveSP]
GO


CREATE PROCEDURE 	dbo.DN_BDWPendingSaveSP
			@acctno varchar(12),
			@user int,
			@code varchar(4),
			@runno int,
			@manualuser int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	BDWPending
	SET		empeeno = @user, 
			code = @code, 
			runno = @runno, 
			empeenomanual = @manualuser
	WHERE	acctno = @acctno

	IF(@@rowcount = 0)
	BEGIN
		INSERT	
		INTO		BDWPending
				(acctno, empeeno, code, runno, empeenomanual)
		VALUES	(@acctno, @user, @code, @runno, @manualuser)
	END

	IF(@@rowcount > 0)
	BEGIN
		DELETE
		FROM 	BDWRejection
		WHERE acctno = @acctno
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

