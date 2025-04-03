IF EXISTS (SELECT * 
		   FROM sysobjects
		   WHERE name = 'View_StoreCardGetProposal'
		   AND xtype = 'V')
BEGIN
DROP VIEW View_StoreCardGetProposal
END
GO

CREATE VIEW View_StoreCardGetProposal
AS
SELECT p.* FROM proposal p
WHERE p.acctno NOT LIKE '___9%'
AND p.dateprop = (SELECT MAX(dateprop) 
				    FROM proposal p2 
					WHERE p.custid = p2.custid
					AND p2.acctno NOT LIKE '___9%')