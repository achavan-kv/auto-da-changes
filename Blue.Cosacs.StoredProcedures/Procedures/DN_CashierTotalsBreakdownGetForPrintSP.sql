SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsBreakdownGetForPrintSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsBreakdownGetForPrintSP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsBreakdownGetForPrintSP
			@id int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code


	SELECT 	code,
			codedescript, 
			category,
			convert(money,0) AS systemtotal,
			convert(money,0) AS usertotal,
			convert(money,0) AS difference,
			convert(varchar(100),'') AS reason,
			convert(varchar(4),'') AS paymethod,
			convert(money,0) AS deposit
	INTO		#totals
	FROM 		code
	WHERE 	category = 'FPM'
	AND		code != 0

	UPDATE	#totals
	SET		systemtotal = CTB.systemtotal,
			usertotal = CTB.usertotal,
			difference = CTB.difference,
			reason = CTB.reason,
			paymethod = CTB.paymethod,
			deposit = CTB.deposit
	FROM		CashierTotalsBreakdown CTB INNER JOIN #totals T
	ON		CTB.paymethod = T.code 
	AND		T.category = 'FPM'
	WHERE	CTB.cashiertotalid = @id

	SELECT	codedescript,
			systemtotal,
			usertotal,
			paymethod,
			difference,
			reason,
			deposit
	FROM 		#totals

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

