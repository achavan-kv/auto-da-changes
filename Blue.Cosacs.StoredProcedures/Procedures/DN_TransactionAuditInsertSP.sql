SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TransactionAuditInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TransactionAuditInsertSP]
GO

CREATE PROCEDURE 	dbo.DN_TransactionAuditInsertSP
			@acctno varchar(12),
			@empeenoauth int,
			@userempeeno int,
			@actualvalue money,
			@calcvalue money,
			@datetrans datetime,
			@transrefno int,
			@transtype varchar(3),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	transactionaudit
	SET		userempeeno = @userempeeno,
			actualvalue = @actualvalue,
			calcvalue = @calcvalue,
			transrefno = @transrefno,
			transtypecode = @transtype
	WHERE	empeenoauth = @empeenoauth
	AND		acctno = @acctno
	AND		datetrans = @datetrans

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		transactionaudit
				(empeenoauth, userempeeno, actualvalue, calcvalue, 
				acctno, datetrans, transrefno, transtypecode)
		VALUES	(@empeenoauth, @userempeeno, @actualvalue, @calcvalue, 
				@acctno, @datetrans, @transrefno, @transtype)
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

