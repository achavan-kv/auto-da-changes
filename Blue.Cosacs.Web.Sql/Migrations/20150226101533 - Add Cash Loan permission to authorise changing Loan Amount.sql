-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from admin.permission where id = 8001)
BEGIN
    insert into admin.Permission(Id, Name, CategoryId, Description)
    select 8001, 'Cash Loan Application - Change Loan Amount', 80, 'Gives the user authorisation to change Loan Amount to either Available Spend or the Maximum Loan Amount allowed for the Customer group (lesser of the two) on Cash Loan Application screen'
END
GO

IF NOT EXISTS(select * from control where TaskID = 8001 and screen = 'CashLoanApplication')
BEGIN
    insert into control (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
    select 8001, 'CashLoanApplication', 'lAuthoriseLoanAmount', 1, 1, ''
END
GO

