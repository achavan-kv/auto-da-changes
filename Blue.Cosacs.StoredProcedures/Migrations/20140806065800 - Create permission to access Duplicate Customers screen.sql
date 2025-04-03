-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from admin.permission where id = 1005)
BEGIN

	insert into admin.permission
	select 1005, 'Duplicate Customers', 10, 'Duplicate Customers - Allows user access to the Duplicate Customers screen via the Customer Menu'

END
