IF EXISTS (SELECT * FROM SYSOBJECTS WHERE NAME ='AcctCheckIfLatest')
DROP PROCEDURE AcctCheckIfLatest
GO
CREATE PROCEDURE AcctCheckIfLatest @acctno CHAR(12), @custid VARCHAR(20), @latest TINYINT OUT 
AS 

	SET @latest = 1

IF EXISTS  (SELECT * FROM acct a
JOIN custacct ca ON a.acctno = ca.acctno
WHERE ca.hldorjnt ='H'
AND a.acctno != @acctno and ca.custid = @custid
AND a.dateacctopen > 
(SELECT ISNULL(dateacctopen,GETDATE())
 FROM acct WHERE acctno= @acctno))
	SET @latest = 0
	
GO 	