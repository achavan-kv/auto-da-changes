IF EXISTS (SELECT * 
		   FROM sysobjects 
		   WHERE name = 'View_StoreCardQualParams'
		   AND xtype = 'V')
BEGIN
	DROP VIEW View_StoreCardQualParams
END
GO

CREATE VIEW View_StoreCardQualParams
AS
SELECT DISTINCT CA.custid, CA.acctno,p.points, customer.ScoreCardType
FROM custacct CA
INNER JOIN customer ON CA.custid = customer.custid
INNER JOIN proposal p ON CA.custid = p.custid
WHERE CA.hldorjnt = 'H'
AND p.datechange = (SELECT MAX(datechange) 
				    FROM proposal p2
				    WHERE p2.custid = p.custid)
GO
