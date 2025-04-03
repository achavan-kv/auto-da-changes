-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update admin.[permission]
set name='View user security Audit',description='User can view security audit on the profile screen'
where id=389

