
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetChequeDetailsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetChequeDetailsSP]
GO

CREATE PROCEDURE 	dbo.DN_FintransGetChequeDetailsSP
			@chequeno VARCHAR(16),
			@bankcode VARCHAR(6),
			@bankacctno VARCHAR(20),
			@datefrom datetime,
			@dateto datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET		ROWCOUNT 250

	SELECT	F.acctno,
			B.bankname,
			C.bankcode,
			C.bankacctno,
			C.chequeno,
			sum(F.transvalue) as transvalue,
			F.paymethod,
			F.branchno,
			CONVERT(VARCHAR(3),'') AS TransTypeCode,
			CONVERT(DATETIME,'')    AS DateTrans,
			CONVERT(INT,0)          AS TransRefNo
	INTO		#cheque
	FROM		cheqdetail C INNER JOIN cheqfintranslnk CFL
	ON		C.bankcode = CFL.bankcode
	AND		C.bankacctno = CFL.bankacctno
	AND		C.chequeno = CFL.chequeno INNER JOIN fintrans F
	ON		CFL.acctno = F.acctno
	AND		CFL.transrefno = F.transrefno INNER JOIN bank B 
	ON		C.bankcode = B.bankcode
	WHERE	(@chequeno = CFL.chequeno OR @chequeno = '')
	AND		(@bankcode = CFL.bankcode OR @bankcode = '')
	AND		(@bankacctno = CFL.bankacctno OR @bankacctno = '')
	AND		F.datetrans BETWEEN @datefrom AND @dateto	
	AND		(F.paymethod%10) = 2
	AND		F.transtypecode in ('PAY', 'COR', 'REF', 'RET', 'DPY')
	GROUP BY	F.acctno, B.bankname, C.bankcode, C.bankacctno, C.chequeno, F.branchno,
                F.paymethod

/*
	SELECT	F.acctno,
			B.bankname, 
			F.bankcode,
			F.bankacctno,
			F.chequeno,
			sum(F.transvalue) as transvalue,
			F.branchno
	INTO 		#cheque
	FROM		fintrans F INNER JOIN bank B ON F.bankcode = B.bankcode
	WHERE	(@chequeno = F.chequeno OR @chequeno = '')
	AND		(@bankcode = F.bankcode OR @bankcode = '')
	AND		(@bankacctno = F.bankacctno OR @bankacctno = '')
	AND		F.datetrans BETWEEN @datefrom AND @dateto	
	AND		(paymethod%10) = 2
	AND		F.chequeno != ''
	AND		F.transtypecode in ('PAY', 'COR', 'REF', 'RET', 'DPY')
	GROUP BY	F.acctno, B.bankname, F.bankcode, F.bankacctno, F.chequeno, F.branchno
*/

	UPDATE	#cheque 
	SET		#cheque.transvalue = #cheque.transvalue + F.transvalue
	FROM		fintrans F 
	WHERE	F.acctno = #cheque.acctno
	AND		F.datetrans not between @datefrom and @dateto
	AND		F.chequeno = #cheque.chequeno
	AND		F.bankacctno = #cheque.bankacctno
	AND		F.transtypecode in ('PAY', 'COR', 'REF', 'RET', 'DPY')

	-- Additional info for DPY transactions. This cannot be included
	-- above because it will affect the GROUP BY
	UPDATE	#cheque 
	SET		#cheque.TransTypeCode = F.TransTypeCode,
	        #cheque.DateTrans     = F.DateTrans,
	        #cheque.TransRefNo    = F.TransRefNo
	FROM	fintrans F 
	WHERE	F.acctno = #cheque.acctno
	AND		F.chequeno = #cheque.chequeno
	AND		F.bankacctno = #cheque.bankacctno
	AND		F.transtypecode = 'DPY'

	SELECT	ch.acctno,
			ch.bankname,
			ch.bankcode,
			ch.bankacctno,
			ch.chequeno,
			-(ch.transvalue) as transvalue,
			ch.PayMethod,
			co.CodeDescript,
			ch.branchno,
			ch.TransTypeCode,
			ch.DateTrans,
			ch.TransRefNo
	FROM	#cheque ch, Code co
	WHERE	ch.transvalue < 0
	AND     co.category = 'FPM'
	AND     co.code = ch.paymethod

	SET		ROWCOUNT 0	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

