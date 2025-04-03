SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BDWRejectionSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BDWRejectionSaveSP]
GO



CREATE PROCEDURE 	dbo.DN_BDWRejectionSaveSP
			@acctno varchar(12),
			@user int,
			@rejectcode varchar(4),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	BDWRejection
	SET	empeeno = @user, 
		balance = acct.outstbal, 
		rejectcode = @rejectcode,
		rejectdate = getdate()
	FROM 	acct
	WHERE 	BDWRejection.acctno = @acctno
	AND	BDWRejection.acctno = acct.acctno

	IF(@@rowcount = 0)
	BEGIN
		INSERT INTO BDWRejection(
				acctno, 
				empeeno, 
				code, 
				balance, 
				rejectcode,
				rejectdate)
	
		SELECT	@acctno,
				@user,
				b.code,
				a.outstbal,
				@rejectcode,
				getdate()
		FROM 		acct a, BDWPending b
		WHERE 	a.acctno = @acctno
		AND		b.acctno = a.acctno
   END	

	DELETE
	FROM 	BDWPending
	WHERE acctno = @acctno



	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

