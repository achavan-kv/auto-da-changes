SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransSetPrintedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransSetPrintedSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransSetPrintedSP
			@acctno varchar(12),
			@transrefno int,
			@transdate datetime,
			@printed char(1),
			@startline int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	agreement
	SET	paymentcardline = @startline
	WHERE	acctno = @acctno

	UPDATE	fintrans
	SET		transprinted = @printed
	WHERE	acctno = @acctno
	AND		transrefno = @transrefno
	AND		datetrans = @transdate	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

