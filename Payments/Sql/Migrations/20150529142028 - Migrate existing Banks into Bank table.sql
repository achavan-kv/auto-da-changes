-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO 
    Payments.Bank (BankName, Active)
SELECT 
    b.bankname, 1
FROM
    Bank b
WHERE 
    b.bankname != ''