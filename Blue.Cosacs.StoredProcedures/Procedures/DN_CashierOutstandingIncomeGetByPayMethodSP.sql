SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierOutstandingIncomeGetByPayMethodSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierOutstandingIncomeGetByPayMethodSP]
GO

/*	JJ 27/7/2004 - the amount available for deposit to the bank for the safe cashier
	was previously given by the amount in the safe minus transfers to the safe from
	cashiers (i.e. untotalled cash) minus floats allocated to cashiers. However, 
	if $10000 is put in the safe and then -$500 is put in the safe (i.e. a float
	of $500) this tha amount available for deposit is given as $9500 - $0 -- $500 = $10000.
	This is incorrect because the $500 float should not be available to deposit to the bank
	from the safe. The solution is not to include cash which has been allocated to 
	cashiers in the amount available for deposit from the safe to the bank */

CREATE PROCEDURE 	dbo.DN_CashierOutstandingIncomeGetByPayMethodSP
			@empeeno int,
			@branchno smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code	

	DECLARE	@localchange money

	IF(@empeeno != -1)	/* a proper cashier and not the safe */
	BEGIN
		/* since we want everything broken down by the payment
		   method it was taken in, we don't have to do any
		   currency conversion and can just use the view */
		SELECT	C.codedescript,
				C.code,
				C.Reference AS NegativeRef,
				COS.depositoutstanding as fordeposit,
				COS.localchange
		INTO		#outstanding
		FROM		vw_cashieroutstandingincome COS 
				INNER JOIN code C ON COS.paymethod = C.code		
		WHERE	C.category = 'FPM'
		AND		COS.empeeno = @empeeno
		AND		COS.branchno = @branchno
		AND     C.codedescript !='StoreCard'
        AND SUBSTRING(c.Additional2,2,1) = 1
		/* 		must make sure that any local change given for foreign 
				tender transactions is subtracted from the cashier's cash
				total for deposit 							*/
		SELECT	@localchange = sum(localchange)
		FROM		#outstanding

		/* doing a union here because we need to treat cash slightly 
	  	   differently i.e. we must subtract any local change given 
		   from the cash total */
		SELECT	codedescript,
				code,
				NegativeRef,
				isnull(sum(fordeposit),0) as fordeposit
		FROM		#outstanding	
		WHERE	code != '1'
		GROUP BY	codedescript, code, NegativeRef
		UNION
		SELECT	codedescript,
				code,
				NegativeRef,
				isnull(sum(fordeposit),0) - @localchange as fordeposit
		FROM		#outstanding	
		WHERE	code = '1'
		GROUP BY	codedescript, code, NegativeRef
		
	END
	ELSE		/* if the safe cashier has been selected */
	BEGIN
		/* gets the sum of all deposits in the safe */
		SELECT	C.codedescript,
				C.code,
				C.Reference AS NegativeRef,
				CD.empeeno,
				isnull(sum(CD.depositvalue),0) as fordeposit
		INTO		#safedeposits
		FROM		cashierdeposits CD INNER JOIN code C
			ON	CD.paymethod = C.code
		WHERE	C.category = 'FPM'
		AND		CD.code = 'SAF'
		AND		CD.datevoided is null
		AND		CD.branchno = @branchno
		GROUP BY 	C.codedescript, C.code, C.Reference, CD.empeeno

		/* 	Any money in the safe which has not been totalled by the cashier 
			that put it there is not allowed to be banked and therefore must
			not be included in the amount available for deposit. Therefore
			we must go through each payment method for all the money in the safe
			and subtract the amount that has not been totalled from the 
			figure */
		DECLARE 	@tempempeeno int, 
				@code varchar(3),
				@allocatedfloat money,
				@transferredtosafe money,
				@datetotalled datetime,
				@sessionStart datetime,
				@totaltosafe money,
				@totalnewincome money

		DECLARE	cashiers_cursor CURSOR
		FOR	
		SELECT	empeeno,
				code
		FROM 		#safedeposits
		
		OPEN		cashiers_cursor
		FETCH NEXT FROM cashiers_cursor
		INTO		@tempempeeno, @code
		
		WHILE	@@FETCH_STATUS = 0
		BEGIN
			/* to find the amount that has not been totalled we find the 
			   amount that has been transferred to and from the safe in
			   current sessions for all cashiers at this branch. Then the 
			   amount available for deposit for each payment method is 
			   updated to be the amount actually in the safe for that
			   payment method less untotalled transfers */
			EXEC	DN_CashierSafeDepositsSP	@empeeno = @tempempeeno,
								@paymethod = @code,
								@branchno = @branchno,
								@safedeposits  = @transferredtosafe OUT,
								@return = @return OUT
	
			EXEC	DN_CashierSafeFloatSP		@empeeno = @tempempeeno,
								@paymethod  = @code,
								@branchno = @branchno,
								@safefloats  = @allocatedfloat OUT,
								@return = @return OUT

			UPDATE	#safedeposits
			SET		fordeposit = fordeposit - @transferredtosafe -- @allocatedfloat		see comment above JJ
			WHERE	empeeno = @tempempeeno
			AND		code = @code
		
			FETCH NEXT FROM cashiers_cursor
			INTO		@tempempeeno, @code
			END
		
		CLOSE 		cashiers_cursor
		DEALLOCATE	cashiers_cursor

		SELECT	codedescript,
				code,
				NegativeRef,
				isnull(sum(fordeposit),0) as fordeposit
		FROM		#safedeposits 	
		GROUP BY	codedescript, code, NegativeRef
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

