-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
update [Admin].[Permission]
set description = 'Allows user to edit the list of repossessed conditions'
where id = 2122