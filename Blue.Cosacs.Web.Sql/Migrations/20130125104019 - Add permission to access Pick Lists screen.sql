-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11452

if not exists(select * from admin.Permission where id =1200)
exec Admin.AddPermission 1200, 'Sys Config - Pick Lists', 12, 'Allows access to the Pick Lists screen'

