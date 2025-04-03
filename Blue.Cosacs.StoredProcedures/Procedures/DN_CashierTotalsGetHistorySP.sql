SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsGetHistorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsGetHistorySP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsGetHistorySP
			@empeeno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	id,
			datefrom,
			dateto,
			usertotal as UserValue,
			systemtotal as SystemValue,
			difference,
			deposittotal as deposit
	FROM		CashierTotals
	WHERE	empeeno = @empeeno
	ORDER BY	datefrom DESC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

