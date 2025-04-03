SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsBreakdownGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsBreakdownGetSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsBreakdownGetSP
			@id int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	C.codedescript,
			CTB.systemtotal,
			CTB.usertotal,
			CTB.deposit,
			CTB.difference,
			CTB.reason,
			CTB.paymethod
	FROM		CashierTotalsBreakdown CTB INNER JOIN
			code C
	ON		CTB.paymethod = C.code 
	AND		C.category = 'FPM'
	WHERE	CTB.cashiertotalid = @id

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

