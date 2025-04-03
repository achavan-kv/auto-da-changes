SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FintransGetAllRFTransactionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetAllRFTransactionsSP]
GO


CREATE PROCEDURE DN_FintransGetAllRFTransactionsSP
	@custid		varchar(20),
	@return		INTEGER OUTPUT

AS
BEGIN

	SET NOCOUNT ON

	SET @return = 0;
	
	SELECT
		F.AcctNo,
		F.TransRefNo,
		F.DateTrans,
		F.TransTypeCode,
		F.TransValue,
		C.CodeDescript AS 'PayMethod',
		u.FullName AS EmployeeName,
		F.EmpeeNo,
		F.TransPrinted
			
	FROM CustAcct CA
	INNER JOIN acct A ON A.AcctNo = CA.AcctNo
	INNER JOIN fintrans F ON f.acctno = A.acctno 
	INNER JOIN Admin.[User] u ON F.empeeno = u.id
	INNER JOIN code c ON c.code = CONVERT(varchar, F.PayMethod)
	WHERE CA.CustId	= @custid
	AND	CA.HldOrJnt	= 'H'
	AND	A.AcctType	= 'R'
	AND	C.category	= 'FPM'

	ORDER BY Convert(varchar, F.datetrans, 105) , F.transtypecode

	SET @return = @@ERROR
	
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

