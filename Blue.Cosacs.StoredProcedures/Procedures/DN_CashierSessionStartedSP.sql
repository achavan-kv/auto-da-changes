SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierSessionStartedSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierSessionStartedSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierSessionStartedSP
			@empeeno int,
			@started datetime OUT,
			@return int OUTPUT

	/* the beginning of the cashier's session is either when they take their first	*/
	/* payment transaction i.e. the first transaction in the fintrans_new_income	*/
	/* table, or when they receive the first float after they have cashed up i.e.	*/
	/* the first transaction in the cashierdeposits table for a negative amount 	*/
	/* to the safe after the cashier cashed up. 				*/

AS

	SET 	@return = 0			--initialise return code

	DECLARE 	@firstpayment datetime, 
			@firstfloat datetime

	SELECT	TOP 1 @firstpayment = datetrans
	FROM		fintrans_new_income
	WHERE	empeeno = @empeeno and empeeno != -1 -- exclude safes from this
	ORDER BY	datetrans ASC

	SELECT	@firstpayment = isnull(@firstpayment, '1/1/4000')

	SELECT	TOP 1 @firstfloat = datedeposit
	FROM	cashierdeposits CD 
	INNER JOIN courtsperson CP	ON	    CD.empeeno = CP.userid
	WHERE	CD.empeeno = @empeeno  and CD.empeeno != -1 -- exclude safes from this
	AND		CD.IsFloat = 1
	AND		CD.datedeposit > CP.datelstaudit

	SELECT	@firstfloat = isnull(@firstfloat, '1/1/4000')

	IF(@firstfloat < @firstpayment)
		SET @started = @firstfloat
	ELSE
		SET @started = @firstpayment

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

