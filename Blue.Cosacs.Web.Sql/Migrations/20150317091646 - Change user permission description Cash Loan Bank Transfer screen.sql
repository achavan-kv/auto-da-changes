-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS(select * from [admin].permission where id = 8002)
BEGIN

    UPDATE 
        [admin].permission
    SET
        Name = 'Cash Loan Record Bank Transfer',
        [Description] = 'Allows the user access to the Cash Loan Record Bank Transfer screen'
    WHERE
        id = 8002
END