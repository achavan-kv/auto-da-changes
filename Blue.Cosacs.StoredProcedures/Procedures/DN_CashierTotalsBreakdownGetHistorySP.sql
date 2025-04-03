SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsBreakdownGetHistorySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsBreakdownGetHistorySP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsBreakdownGetHistorySP
			@empeeno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	CT.id,
			C.codedescript,
			CTB.systemtotal,
			CTB.usertotal,
			CTB.deposit,
			CTB.difference,
			CTB.reason
	FROM		CashierTotals CT INNER JOIN 
			CashierTotalsBreakdown CTB 
	ON		CT.id = CTB.cashiertotalid INNER JOIN
			code C
	ON		CTB.paymethod = C.code 
	AND		C.category = 'FPM'
	WHERE	CT.empeeno = @empeeno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

