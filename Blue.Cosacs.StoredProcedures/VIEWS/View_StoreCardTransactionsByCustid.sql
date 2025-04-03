IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'View_StoreCardTransactionsByCustid'
		   AND xtype = 'V')
BEGIN
	DROP VIEW View_StoreCardTransactionsByCustid
END
GO


CREATE VIEW View_StoreCardTransactionsByCustid
AS
SELECT custid,transvalue, transtypecode, a.acctno
FROM custacct ca
INNER JOIN acct a ON a.acctno = ca.acctno
LEFT OUTER JOIN fintrans f ON ca.acctno = f.acctno
WHERE ca.hldorjnt = 'H'
AND a.accttype = 'T'

