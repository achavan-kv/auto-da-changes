-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #10970

if not exists(select * from admin.Permission where id =1629)
exec Admin.AddPermission 1629, 'View My Payments', 16, 'Allow user to view their own Technician Payments Screen'
