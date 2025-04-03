IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='view_LineDetails')
DROP VIEW view_LineDetails
GO 
CREATE VIEW view_LineDetails 
AS 
SELECT d.acctno AS AcctNo,d.AgrmtNo ,d.ItemNo,d.quantity,d.ordval AS Value, s.itemdescr1 AS Description1,s.itemdescr2 AS Description2, d.ContractNo 
, CONVERT(VARCHAR,g.empeenosale) + ' - ' + c.FullName  AS SoldBy
--FROM delivery d 
FROM lineitem d -- ON d.acctno = l.acctno AND d.agrmtno = l.agrmtno AND d.ItemID = l.ItemID
JOIN agreement g ON D.acctno = g.acctno AND d.agrmtno = g.agrmtno
LEFT JOIN Admin.[User] c ON c.Id = g.empeenosale
JOIN StockInfo s ON s.id = d.ItemID
WHERE d.quantity >0 
GO 

