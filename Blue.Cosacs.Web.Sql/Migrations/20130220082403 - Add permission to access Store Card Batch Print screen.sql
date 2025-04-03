-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from admin.Permission where id =1201)
exec Admin.AddPermission 1201, 'Sys Config - Store Card Batch Print', 12, 'Allows access to the Store Card Batch Print screen'
