SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsIncludeDeposit]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsIncludeDeposit]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsIncludeDeposit
			@empeeno int,
			@include smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @lastdeposit datetime

	SELECT @lastdeposit = MAX(datedeposit) 
	FROM cashierdeposits 
	WHERE empeeno = @empeeno
   AND includeincashiertotals !=@include

	UPDATE cashierdeposits 
	SET includeincashiertotals = @include 
	WHERE empeeno = @empeeno 
	AND datedeposit > DATEADD(minute, -1, @lastdeposit)
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

