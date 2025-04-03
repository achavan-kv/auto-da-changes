-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taskID int

select @taskID = MAX(taskid)+1
from task

IF NOT EXISTS(select * from task where taskname = 'Service - Write-Off Service Cash Account')
insert into Task
select @taskID, 'Service - Write-Off Service Cash Account'

IF NOT EXISTS(select * from [control] where screen = 'SR_Outstanding' and [control] = 'lAllowWriteOff')
insert into [control]
(TaskID, Screen, [Control], Visible, [Enabled], ParentMenu)
select @taskID, 'SR_Outstanding', 'lAllowWriteOff', 0, 1, ''