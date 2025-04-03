-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

if exists(select * from [control] where screen = 'SR_SoftScript' and [control] = 'lPreviousRepair')
delete from [control]
where screen = 'SR_SoftScript' and [control] = 'lPreviousRepair'