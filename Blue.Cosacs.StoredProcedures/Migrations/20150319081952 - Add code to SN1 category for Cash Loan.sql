-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from code where category = 'SN1' and code = 'CL')
BEGIN
    INSERT INTO Code(origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
    SELECT 0, 'SN1', 'CL', 'New Cash Loan Customer', 'L', 0, 0, NULL, NULL
END
GO