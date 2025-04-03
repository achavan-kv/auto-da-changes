-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if not exists(select * from admin.Permission where id =1630)
exec Admin.AddPermission 1630, 'View Technician Payments', 16, 'Allow user to view payments for Technicians via the Technician Payments Screen'