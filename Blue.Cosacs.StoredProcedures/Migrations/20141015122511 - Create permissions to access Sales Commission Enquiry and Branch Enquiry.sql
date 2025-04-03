-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from admin.permission where id = 9001)
BEGIN

	insert into admin.permission
	select 9001, 'Sales Commission Enquiry for CSR', 9, 'Sales Commission Enquiry for CSR - Allows user access to the Sales Commission Enquiry via the Reports Menu. Users will not have access to Branch and Employee drop downs'

END

IF NOT EXISTS(select * from admin.permission where id = 9002)
BEGIN

	insert into admin.permission
	select 9002, 'Sales Commission Enquiry for Branch Manager', 9, 'Sales Commission Enquiry for Branch Manager - Allows user access to the Sales Commission Enquiry via the Reports Menu. Users will have access to Branch and Employee drop downs'

END
