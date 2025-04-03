-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE
    StockQuantity
SET
    deleted = 'N'
FROM
    StockQuantity sq
INNER JOIN 
    StockInfo si on sq.ID = si.Id
INNER JOIN 
    Warranty.Warranty w on si.itemno = w.Number
WHERE
    w.Deleted = 0
    and sq.deleted = 'Y'