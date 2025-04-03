-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


UPDATE
    Code
SET     
    Reference = si.category
FROM 
    Stockinfo si 
INNER JOIN 
    Code c on si.itemno = c.code
WHERE 
    c.Category = 'INST'
