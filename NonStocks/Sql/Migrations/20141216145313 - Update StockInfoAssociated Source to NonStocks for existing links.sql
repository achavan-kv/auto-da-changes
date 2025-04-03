-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE
    StockInfoAssociated
SET
    [Source] = 'NonStocks'
FROM 
    StockInfoAssociated sia
INNER JOIN 
    StockInfo si on sia.AssocItemId = si.Id
WHERE   
    si.itemtype = 'N' 
