-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taskID int

select @taskID = taskid from [control] where screen = 'SR_Outstanding' and [control] = 'lPreviousRepair'

IF NOT EXISTS(select * from [Control] where screen = 'SR_ServiceRequest' and [Control] = 'lPreviousRepair')
insert into [control] (TaskID, Screen, [Control], Visible, [Enabled], ParentMenu)
select @taskID, 'SR_ServiceRequest', 'lPreviousRepair', 0, 1, ''

