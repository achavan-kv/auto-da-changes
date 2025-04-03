SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransHasCashierTotalledSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransHasCashierTotalledSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransHasCashierTotalledSP
			@empeeno int,
			@hastotalled char(1) OUTPUT,
			@return int OUTPUT
			
AS DECLARE
	@sessionStart datetime,
	@datetotalled datetime

	SET 	@return = 0			--initialise return code

	/* Find out when the current cashier's session started */
	EXEC DN_CashierSessionStartedSP 	@empeeno = @empeeno,
						@started = @sessionStart OUT,
						@return = @return OUT	
	if @return = 0
	begin
		/* find out when the current cashier last totalled */
		SELECT	@datetotalled = datelstaudit 
		FROM		courtsperson
		WHERE	UserID = @empeeno

		IF(@sessionStart = '1/1/4000')		/* session has not started yet */
			SET	@hastotalled = 'Y'
		ELSE
			IF(@sessionStart > @datetotalled)
				SET	@hastotalled = 'N'
			ELSE
				SET	@hastotalled = 'Y'
	end

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

