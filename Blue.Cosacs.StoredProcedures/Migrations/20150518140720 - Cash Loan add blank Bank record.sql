-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from bank where bankcode = '')
BEGIN
    INSERT INTO Bank(origbr, bankcode, bankname, bankaddr1, bankaddr2, bankaddr3, bankpocode)
    SELECT NULL, '', '', '', '', '', ''
END