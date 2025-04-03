SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerGetForDebtCollector]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerGetForDebtCollector]
GO

CREATE PROCEDURE dbo.DN_CustomerGetForDebtCollector
		 	@acctno varchar(12),
		 	@return int OUTPUT

AS

	SET @return = 0			--initialise return code

	SELECT	A.AcctNo,
			A.AcctType,
			A.DateAcctOpen,
			A.CreditDays,
			A.AgrmtTotal,
			A.DateLastPaid,
			A.AS400Bal,
			A.OutstBal,
			A.Arrears,
			A.HighstStatus,
			A.CurrStatus,
			A.HiStatusDays,
			C.custid,
			C.Hldorjnt,
			CO.Firstname,
			CO.Name,
			CO.Title,
			CO.Alias,
			CO.Title,
			IP.Instalamount,
			IP.DateFirst,
			CO.RFCreditLimit,
			CO.LimitType
	FROM		ACCT A ,
			CUSTACCT C, 
			Customer CO 
			LEFT JOIN Instalplan IP on IP.acctno = @acctno
	WHERE 	A.acctno	= @acctno
	AND 		C.acctno	= @acctno
	AND 		C.acctno	= A.acctno
	AND 		C.hldorjnt	= 'H'
	AND 		CO.custid	= C.Custid;

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

