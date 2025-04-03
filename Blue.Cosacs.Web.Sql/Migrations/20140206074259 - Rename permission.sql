-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update admin.Permission
	set name='Online Product Maintenance',Description='Allows user access to the Online Product Maintenance screen'
where id=394


