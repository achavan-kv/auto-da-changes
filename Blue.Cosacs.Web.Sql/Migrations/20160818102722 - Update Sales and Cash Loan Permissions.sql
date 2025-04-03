-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

SELECT * 
INTO #tempPermissions
FROM [Admin].RolePermission
WHERE PermissionId IN (SELECT Id FROM [Admin].Permission WHERE CategoryId = 88)

UPDATE #tempPermissions
SET PermissionId = CAST('88' + RIGHT(PermissionId, 2) AS INT)
WHERE PermissionId IN (SELECT Id FROM [Admin].Permission WHERE CategoryId = 88)

DELETE FROM [Admin].RolePermission
WHERE PermissionId IN (SELECT Id FROM [Admin].Permission WHERE CategoryId = 88)

--Update the permission Ids for Sales
UPDATE [Admin].Permission
SET Id = CAST('88' + RIGHT(Id, 2) AS INT)
WHERE CategoryId = 88

INSERT INTO [Admin].RolePermission
SELECT * FROM #tempPermissions

--Remove and re-insert Cash Loan Permissions

IF NOT EXISTS(select * from admin.PermissionCategory where id = 80)
BEGIN
    insert into admin.PermissionCategory (Id, Name)
    select 80, 'Cash Loans'
END
GO

--Remove permissions if already in there 
DELETE FROM [Admin].Permission
WHERE Id in (8000, 8001, 8002)

--Add new permissions
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

IF NOT EXISTS(select * from admin.permission where id = 8002)
BEGIN
    insert into admin.Permission(Id, Name, CategoryId, Description)
    select 8002, 'Cash Loan Record Bank Transfer', 80, 'Allows the user access to the Cash Loan Record Bank Transfer screen'
END
GO