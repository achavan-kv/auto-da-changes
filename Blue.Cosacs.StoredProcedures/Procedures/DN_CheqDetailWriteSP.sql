SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CheqDetailWriteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CheqDetailWriteSP]
GO




CREATE PROCEDURE 	dbo.DN_CheqDetailWriteSP
			@bankCode varchar(6),
			@bankAcctNo varchar(20),
			@chequeNo varchar(14),
			@chequeVal float,
			@acctno varchar(12),
			@transrefno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	cheqdetail
	SET		bankcode = @bankCode,
			bankacctno = @bankAcctNo,
			chequeno = @chequeNo,
			cheqval =  cheqval + @chequeVal
	WHERE	bankcode = @bankCode
	AND		bankacctno = @bankAcctNo
	AND		chequeno = @chequeNo
	--AND		cheqval = @chequeVal

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		cheqdetail
				(origbr, bankcode, bankacctno, chequeno, cheqval)
		VALUES	(0, @bankCode, @bankAcctNo, @chequeNo, @chequeVal)
	END

	UPDATE	cheqfintranslnk
	SET		bankcode = @bankCode,
			bankacctno = @bankAcctNo,
			chequeno = @chequeNo,
			acctno  = @acctno,
			transrefno = @transrefno
	WHERE	bankcode = @bankCode
	AND		bankacctno = @bankAcctNo
	AND		chequeno = @chequeNo
	AND		acctno = @acctno
	AND		transrefno = @transrefno

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		cheqfintranslnk
				(bankcode, bankacctno, chequeno, acctno, transrefno)
		VALUES	(@bankCode, @bankAcctNo, @chequeNo, @acctno, @transrefno)
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

