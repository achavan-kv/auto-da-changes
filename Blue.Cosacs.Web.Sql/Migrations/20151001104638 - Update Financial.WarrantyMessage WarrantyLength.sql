-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE 
    financial.WarrantyMessage
SET
    WarrantyLength = w.Length
FROM
    Warranty.Warranty w
INNER JOIN 
    Financial.WarrantyMessage f on w.Number = f.WarrantyNo
WHERE
    f.WarrantyLength != w.Length
