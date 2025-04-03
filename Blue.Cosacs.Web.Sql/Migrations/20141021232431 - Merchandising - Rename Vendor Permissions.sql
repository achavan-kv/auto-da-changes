-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
select * from Admin.Permission

update Admin.Permission
set Name = 'Vendor View',
	Description = 'Allows user to view the vendor screen'
where id = 2110

update Admin.Permission
set Name = 'Vendor Edit',
	Description = 'Allows user to create and edit vendors'
where id = 2111
