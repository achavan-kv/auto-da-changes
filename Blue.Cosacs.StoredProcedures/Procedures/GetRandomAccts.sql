-- procedure used for testing purposes
IF  EXISTS (SELECT * FROM sysobjects WHERE NAME = 'GetRandomAccts')
DROP PROCEDURE GetRandomAccts
GO 
CREATE  PROCEDURE GetRandomAccts @count INT ,@includeSettled BIT,@accttype CHAR(1) 
AS  
 

IF @accttype = ''
	SET @accttype = '%'

SELECT TOP (@count) a.acctno, outstbal as balance , custid FROM acct a
JOIN custacct ca ON a.acctno= ca.acctno
WHERE accttype LIKE @accttype AND ca.hldorjnt = 'H'
AND (@includesettled = 1 OR (currstatus !='S' and outstbal>0))
order by NEWID()
GO 
