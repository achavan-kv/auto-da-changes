-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--ParentItemID is NULL for existing Schedule records. Update to the correct ParentItemID from Lineitem table

UPDATE schedule
SET ParentItemID = isnull(l.ParentItemID, 0)
FROM lineitem l INNER JOIN schedule s ON l.acctno = s.acctno
AND l.ItemID = s.ItemID
AND l.StockLocn = s.StockLocn
WHERE s.ParentItemID is null