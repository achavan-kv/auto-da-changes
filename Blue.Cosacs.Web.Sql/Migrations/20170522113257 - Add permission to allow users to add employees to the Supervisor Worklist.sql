-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from admin.permission where id = 399)
BEGIN
    insert into admin.Permission(Id, Name, CategoryId, Description, IsDelegate)
    select 399, 'Work List Setup - add Supervisor Work List to a user ', 3, 'Allows the user to add the Supervisor Work List to an employee.', 0
END
GO

