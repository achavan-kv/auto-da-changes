-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #11989

if not exists(select * from admin.Permission where id =1100)
exec Admin.AddPermission 1100, 'Service Request - View BER Replacements screen', 11, 'Allow user to view the BER Replacements screen'