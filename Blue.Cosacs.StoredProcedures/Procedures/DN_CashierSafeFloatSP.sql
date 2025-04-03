SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierSafeFloatSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierSafeFloatSP]
GO



CREATE PROCEDURE 	dbo.DN_CashierSafeFloatSP
			@empeeno int,
			@paymethod varchar(4),
			@branchno smallint,
			@safefloats money OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE	@sessionStart datetime

	SET	@safefloats = 0
	SET	@sessionStart = '1/1/1900'

	/* find the date which represents the begining of the cashier's current session */
	EXEC DN_CashierSessionStartedSP 	@empeeno = @empeeno,
						@started = @sessionStart OUT,
						@return = @return OUT	

	/* find the sum of all floats made to the safe since the cashier's session started */
		
	/* if the paymethod is not specified return the total in local currency, other wise 
		leave it as it is */

	IF(@paymethod = '')
	BEGIN		
		SELECT	@safefloats = isnull(sum(depositvalue), 0)
		FROM	cashierdeposits
		WHERE	empeeno = @empeeno 
		AND		datedeposit >= @sessionStart	
		AND		IsFloat = 1			/* exclude deposits */
		AND		datevoided is null
		AND		branchno = @branchno
		AND		Convert(int, paymethod) < 100			/* local currency deposits */
		AND		runno != -1				/* exclude deposits not going to FACT */
		
		SELECT	@safefloats = @safefloats + isnull(round(sum(CD.depositvalue * isnull(ER.rate, 1)),2),0)
		FROM	cashierdeposits CD INNER JOIN
				exchangerate ER ON CD.paymethod = ER.currency
				AND	ER.Status = 'C'
		WHERE	CD.empeeno = @empeeno 
		AND		CD.datedeposit >= @sessionStart	
		AND		CD.IsFloat = 1			/* exclude deposits */
		AND		CD.datevoided is null
		AND		CD.branchno = @branchno
		AND		Convert(int, CD.paymethod) >= 100		/* foreign currency deposits */	
		AND		CD.runno != -1				/* exclude deposits not going to FACT */
	END
	ELSE
	BEGIN
		SELECT	@safefloats = isnull(sum(depositvalue), 0)
		FROM	cashierdeposits
		WHERE	empeeno = @empeeno 
		AND		datedeposit >= @sessionStart	
		AND		IsFloat = 1			/* exclude deposits */
		AND		datevoided is null
		AND		branchno = @branchno
		AND		paymethod = @paymethod
		AND		runno != -1				/* exclude deposits not going to FACT */
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

