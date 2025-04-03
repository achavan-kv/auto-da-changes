IF EXISTS (SELECT * FROM sysobjects WHERE NAME LIKE 'ScorecardAccountGet')
DROP PROCEDURE ScorecardAccountGet
GO 
CREATE PROCEDURE ScorecardAccountGet 
@custid VARCHAR(20)
AS 
    SELECT a.acctno 
    FROM acct a 
    JOIN dbo.custacct ca ON ca.acctno= a.acctno
    WHERE ca.hldorjnt= 'H' AND ca.custid = @custid 
    AND a.accttype = 'T' -- storecard accttype
    AND a.currstatus !='S'
GO 
