-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from admin.permission where id = 8002)
BEGIN
    insert into admin.Permission(Id, Name, CategoryId, Description)
    select 8002, 'Cash Loan Disbursement Record Bank Transfer', 80, 'Allows the user access to the Cash Loan Disbursement Record Bank Transfer screen'
END
GO

