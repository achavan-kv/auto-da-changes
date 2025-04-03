-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from admin.Permission where id =57)
exec Admin.AddPermission 57, 'Scoring - Create/Edit Scoring Rules', 12, 'Customer Scoring Rules - Allows the user to Create/Edit Scoring Rules set'

