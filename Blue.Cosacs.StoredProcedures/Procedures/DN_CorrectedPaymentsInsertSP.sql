SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CorrectedPaymentsInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CorrectedPaymentsInsertSP]
GO

CREATE PROCEDURE 	dbo.DN_CorrectedPaymentsInsertSP
			@acctno varchar(12),
			@paymentref int,
			@correctionref int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	CorrectedPayments
	SET		correctionref = @correctionref
	WHERE	acctno = @acctno 
	AND		paymentref = @paymentref

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		CorrectedPayments
				(acctno, paymentref, correctionref)
		VALUES	(@acctno, @paymentref, @correctionref)
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

