IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.tables WHERE table_name ='view_DeliveryDetails')
DROP VIEW view_DeliveryDetails
GO 
CREATE VIEW view_DeliveryDetails 
AS 
SELECT d.acctno AS AcctNo,d.AgrmtNo ,d.ItemNo, s.itemdescr1 AS Description1,s.itemdescr2 AS Description2, d.ContractNo 
, CONVERT(VARCHAR,g.empeenosale) + ' - ' + c.FullName AS SoldBy
FROM delivery d 
JOIN lineitem l ON d.acctno = l.acctno AND d.agrmtno = l.agrmtno AND d.ItemID = l.ItemID
JOIN agreement g ON l.acctno = g.acctno AND l.agrmtno = g.agrmtno
LEFT JOIN Admin.[User] c ON c.Id = g.empeenosale
JOIN StockInfo s ON s.id = l.ItemID
GO 

