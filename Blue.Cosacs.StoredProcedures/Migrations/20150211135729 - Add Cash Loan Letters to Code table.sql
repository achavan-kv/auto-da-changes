-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from code where category = 'LT1' and code = 'LoanE')
BEGIN
    INSERT INTO 
        Code (origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
    SELECT
        0, 'LT1', 'LoanE', 'Cash Loan Letter existing Customers', 'L', 0, 0, null, null
END
GO


IF NOT EXISTS(select * from code where category = 'LT1' and code = 'LoanR')
BEGIN
    INSERT INTO 
        Code (origbr, category, code, codedescript, statusflag, sortorder, reference, additional, Additional2)
    SELECT
        0, 'LT1', 'LoanR', 'Cash Loan Letter recent Customers', 'L', 0, 0, null, null
END
GO
