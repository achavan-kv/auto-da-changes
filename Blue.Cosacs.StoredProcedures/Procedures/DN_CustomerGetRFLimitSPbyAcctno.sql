IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'DN_CustomerGetRFLimitSPbyAcctno'
		   AND xtype = 'P')
BEGIN
	DROP PROCEDURE DN_CustomerGetRFLimitSPbyAcctno
END
GO

CREATE PROCEDURE DN_CustomerGetRFLimitSPbyAcctno
@acctno varchar(12),
@AcctList varchar(400),
@limit money OUT,
@available money OUT,
@return int OUTPUT

AS

BEGIN

	DECLARE @custid VARCHAR(20)

	SELECT @custid = custacct.custid 
	FROM custacct
	INNER JOIN storecard ON custacct.acctno = StoreCard.AcctNo
	WHERE storecard.acctno = @acctno
	AND hldorjnt = 'H'

	EXEC DN_CustomerGetRFLimitSP 
			@custid = @custid, -- varchar(20)
			@AcctList = @AcctList, -- varchar(400)
			@limit = @limit OUT, -- money
			@available = @available OUT, -- money
			@return = @return OUT -- int
END