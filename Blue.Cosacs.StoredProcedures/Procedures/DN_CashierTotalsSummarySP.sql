SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsSummarySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsSummarySP]
GO

CREATE PROCEDURE 	dbo.DN_CashierTotalsSummarySP
			@branch smallint,
			@datefrom datetime,
			@dateto datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	C.Code, 
			C.codedescript,
			sum(CTB.systemtotal) as systemtotal,
			sum(CTB.usertotal) as usertotal,
			sum(CTB.deposit) as deposit,
			sum(CTB.difference) as difference,
			sum(CTB.securitisedtotal) as securitisedtotal
	FROM		cashiertotals CT INNER JOIN 
			cashiertotalsbreakdown CTB ON
			CT.id = CTB.cashiertotalid INNER JOIN
			code C ON
			CTB.paymethod = C.code AND
			C.category = 'FPM'
	WHERE	CT.dateto >= @datefrom
	AND		CT.dateto <= @dateto
	AND		(CT.branchno = @branch OR @branch = -1)
	GROUP BY	C.code, C.codedescript
			

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

