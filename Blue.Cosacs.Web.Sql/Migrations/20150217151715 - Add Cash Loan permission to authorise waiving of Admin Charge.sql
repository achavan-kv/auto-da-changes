-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from admin.PermissionCategory where id = 80)
BEGIN
    insert into admin.PermissionCategory (Id, Name)
    select 80, 'Cash Loans'
END
GO

IF NOT EXISTS(select * from admin.permission where id = 8000)
BEGIN
    insert into admin.Permission(Id, Name, CategoryId, Description)
    select 8000, 'Cash Loan Application - Waive / Change Admin Charge', 80, 'Gives the user authorisation to waive / change Admin Charge on Cash Loan Application screen'
END
GO

IF NOT EXISTS(select * from control where TaskID = 8000 and screen = 'CashLoanApplication')
BEGIN
    insert into control (TaskID, Screen, Control, Visible, Enabled, ParentMenu)
    select 8000, 'CashLoanApplication', 'lAuthorise', 1, 1, ''
END
GO


