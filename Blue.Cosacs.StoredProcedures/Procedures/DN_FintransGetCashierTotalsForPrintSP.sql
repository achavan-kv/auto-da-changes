SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetCashierTotalsForPrintSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetCashierTotalsForPrintSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetCashierTotalsForPrintSP
			@byBranch smallint,
			@branchno smallint,
			@employeeno int,
			@datefrom datetime,
			@dateto datetime,
			@listcheques smallint,
			@total money OUT, 
			@return int OUTPUT

AS
--preventing system lock-up by reducing the number of days the fintrans query is running against

	SET 	@return = 0			--initialise return code


	IF(@byBranch = 1)
	BEGIN
		SELECT	FT.empeeno,
				FT.transtypecode,
				C.codedescript,
				FT.paymethod,
				-FT.transvalue as transvalue,
				FT.datetrans,
				isnull(FTE.foreigntender,0) as foreigntender,
				isnull(FTE.localchange,0) as localchange
		FROM		fintrans FT INNER JOIN code C	ON
				FT.paymethod = C.code AND
				C.category = 'FPM' LEFT OUTER JOIN fintransexchange FTE ON
				FT.acctno = FTE.acctno AND
				FT.datetrans = FTE.datetrans AND
				FT.transrefno = FTE.transrefno
		WHERE	FT.branchno = @branchno
		AND		FT.transtypecode in ('PAY', 'REF', 'COR', 'RET', 'DPY')
		AND		(FT.datetrans >= @datefrom AND FT.datetrans <= @dateto)
		ORDER BY	FT.datetrans, FT.empeeno	

		SELECT	@total = sum(-FT.transvalue)
		FROM		fintrans FT 
		WHERE	FT.branchno = @branchno
		AND		FT.transtypecode in ('PAY', 'REF', 'COR', 'RET','DPY')
		AND		(FT.datetrans >= @datefrom AND FT.datetrans <= @dateto)
	END
	ELSE
	BEGIN
		IF(@listcheques = 0)
		BEGIN
			SELECT	FT.empeeno,
					FT.transrefno,
					FT.transtypecode,
					C.codedescript,
					FT.paymethod,
					-FT.transvalue as transvalue,
					FT.datetrans,
					isnull(FTE.foreigntender,0) as foreigntender,
					isnull(FTE.localchange,0) as localchange
			FROM		cashiertotalsincome FT INNER JOIN code C	ON
					FT.paymethod = C.code AND
					C.category = 'FPM'  LEFT OUTER JOIN fintransexchange FTE ON
					FT.acctno = FTE.acctno AND
					FT.datetrans = FTE.datetrans AND
					FT.transrefno = FTE.transrefno
			WHERE	FT.empeeno = @employeeno
			AND		(FT.datetrans >= @datefrom AND FT.datetrans <= @dateto)
			ORDER BY	FT.datetrans
	
			SELECT	@total = sum(-FT.transvalue)
			FROM		fintrans_new_income FT
			WHERE	FT.empeeno = @employeeno
		END
		ELSE
		BEGIN
			SELECT	C.name,
					FT.acctno,					
					FT.chequeno,
					B.bankname,
					B.bankcode,
					FT.bankacctno,
					-FT.transvalue as transvalue,
					FT.datetrans,
					isnull(FTE.foreigntender,0) as foreigntender,
					isnull(FTE.localchange,0) as localchange
			FROM		cashiertotalsincome FT INNER JOIN bank B	ON
					FT.bankcode = B.bankcode INNER JOIN custacct CA ON
					FT.acctno = CA.acctno AND
					CA.hldorjnt = 'H' INNER JOIN customer C ON
					CA.custid = C.custid  LEFT OUTER JOIN fintransexchange FTE ON
					FT.acctno = FTE.acctno AND
					FT.datetrans = FTE.datetrans AND
					FT.transrefno = FTE.transrefno
			WHERE	FT.empeeno = @employeeno
			AND		(FT.paymethod%10) = 2
			ORDER BY	FT.datetrans

			SELECT	@total = sum(-FT.transvalue)
			FROM		fintrans_new_income FT
			WHERE	FT.empeeno = @employeeno
			AND		(FT.paymethod%10) = 2
		END
	END

	SET	ROWCOUNT 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

