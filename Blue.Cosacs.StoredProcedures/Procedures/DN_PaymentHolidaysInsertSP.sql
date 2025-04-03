SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PaymentHolidaysInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PaymentHolidaysInsertSP]
GO


CREATE PROCEDURE 	dbo.DN_PaymentHolidaysInsertSP
			@acctno varchar(12),
			@agrmtno int,
			@datetaken datetime,
			@empeeno int,
			@newdatefirst datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	PaymentHolidays
	SET		empeeno = @empeeno,
			newdatefirst = @newdatefirst
	WHERE	acctno = @acctno
	AND		agrmtno = @agrmtno
	AND		datetaken = @datetaken
	
	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO	PaymentHolidays
				(acctno, agrmtno, datetaken, empeeno, newdatefirst)
		VALUES	(@acctno, @agrmtno, @datetaken, @empeeno, @newdatefirst)
	END	

	UPDATE	instalplan
	SET	datefirst = @newdatefirst
	WHERE	acctno = @acctno
	AND	agrmtno = @agrmtno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

