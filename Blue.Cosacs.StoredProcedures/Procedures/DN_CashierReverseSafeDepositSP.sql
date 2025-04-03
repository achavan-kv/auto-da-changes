SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierReverseSafeDepositSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierReverseSafeDepositSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierReverseSafeDepositSP
			@empeeno int,	
			@user int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE	@depositid int, @datevoided datetime, @sessionStart datetime

	SET	@datevoided = getdate()
	SET	@sessionStart = '1/1/1900'	

	/* find the date which represents the begining of the cashier's current session */
	EXEC DN_CashierSessionStartedSP 	@empeeno = @empeeno,
						@started = @sessionStart OUT,
						@return = @return OUT

	DECLARE	cashiers_cursor3 CURSOR STATIC -- needed static this data is updated by the void procedure below - changing name as there was more than 
-- than one cursor with the same name
	FOR	
		SELECT	depositid			/* create a cursor to loop through all */
		FROM		cashierdeposits			/* transfers to safe for this session */
		WHERE	empeeno = @empeeno 
		AND		code = 'SAF'
		AND		datedeposit >= @sessionStart	
		AND		IsFloat = 0			/* exclude floats */
		AND		datevoided is null
		AND		IsReversed = 0		-- FR67933 (relates to FR67749)
	
	OPEN		cashiers_cursor3
	FETCH NEXT FROM cashiers_cursor3
	INTO		@depositid
	
	WHILE	@@FETCH_STATUS = 0
	BEGIN
		
		EXEC	DN_CashierDepositVoidSP @depositid = @depositid,	/* void each one */
			@datevoided  = @datevoided,
			@voidedby = @user,
			@reverse = 1,  -- voided changing from 0
			@return = @return OUTPUT

	
		FETCH NEXT FROM cashiers_cursor3
		INTO		@depositid
	END
	
	CLOSE 		cashiers_cursor3
	DEALLOCATE	cashiers_cursor3


	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

