

IF EXISTS (SELECT * FROM sysobjects
           WHERE NAME = 'LoyaltyCheckAccountPeriod'
           AND xtype = 'p')
BEGIN
	DROP PROCEDURE LoyaltyCheckAccountPeriod
END
GO

CREATE PROCEDURE LoyaltyCheckAccountPeriod
@acctno VARCHAR(12),
@return INT output
AS
BEGIN

IF EXISTS (SELECT * FROM acct
		   INNER JOIN custacct ON custacct.acctno = acct.acctno
		   WHERE acct.acctno = @acctno
		   AND custacct.hldorjnt = 'H'
		   AND EXISTS (SELECT * FROM Loyalty
		               WHERE acct.dateacctopen BETWEEN DATEADD(hour,-1,Loyalty.StartDate) AND Loyalty.Enddate
		               AND loyalty.StatusAcct = 1
		               AND loyalty.custid = custacct.custid))
BEGIN
	SELECT 1
END
ELSE
BEGIN
	SELECT 0
END

END