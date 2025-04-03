SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountAddCodeToSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountAddCodeToSP]
GO






CREATE PROCEDURE 	dbo.DN_AccountAddCodeToSP
			@acctNo varchar(12),
			@code varchar(4),
			@date datetime,
			@empno int,
			@reference varchar(10),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	acctcode
	SET		acctno = @acctNo,
			code = @code,
			datecoded = @date,
			empeenocode = @empno
	WHERE	acctno = @acctNo
	AND		code = @code
	AND     reference = @reference

	IF(@@rowcount=0)
	BEGIN
		INSERT INTO acctcode
		(acctno, code, datecoded, empeenocode, reference)
		VALUES (@acctNo, @code, @date, @empno, @reference)
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

