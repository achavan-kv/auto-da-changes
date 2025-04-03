-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Admin.Permission set name='Service Request - BatchPrint' where id=309
exec Admin.AddPermission  1619, 'Service Request - Batch Print', 16, 'Allow user to batch print service requests.' 
exec Admin.AddPermission  1620, 'Service Request - Export', 16, 'Allow user to export summary details of service requests.' 
exec Admin.AddPermission  1621, 'Service Request - Summary Print', 16, 'Allow user to print summary details of service requests.'
