-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select * from admin.permission where id = 9002 and name = 'Sales Commission Enquiry for Branch Manager')
BEGIN
	DELETE FROM 
		Admin.[Permission]
	WHERE
		id = 9002
		and name = 'Sales Commission Enquiry for Branch Manager'
END


IF EXISTS(select * from admin.permission where id = 9003 and name = 'Sales Commission Branch Enquiry')
BEGIN
	UPDATE 
		admin.Permission
	SET
		Description = 'Sales Commission Branch Enquiry - Allows user access to the Sales Commission Branch Enquiry via the Reports Menu. Allows user access to Sales Commission Enquiry with additional drop downs Employee and Branch displayed.'
	WHERE
		id = 9003
		and name = 'Sales Commission Branch Enquiry'
END


