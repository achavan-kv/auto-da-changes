-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS(select * from admin.permission where id = 9003)
BEGIN

	insert into admin.permission
	select 9003, 'Sales Commission Branch Enquiry', 9, 'Sales Commission Branch Enquiry - Allows user access to the Sales Commission Branch Enquiry via the Reports Menu.'

END
