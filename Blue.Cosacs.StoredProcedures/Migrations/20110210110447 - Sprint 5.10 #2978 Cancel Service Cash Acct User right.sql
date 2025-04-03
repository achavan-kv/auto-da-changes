-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taskID int

select @taskID = MAX(taskid)+1
from task

IF NOT EXISTS(select * from task where taskname = 'Service - Cancel Service Cash Account')
insert into Task
select @taskID, 'Service - Cancel Service Cash Account'

IF NOT EXISTS(select * from [control] where screen = 'SR_Outstanding' and [control] = 'lAllowCancel')
insert into [control]
(TaskID, Screen, [Control], Visible, [Enabled], ParentMenu)
select @taskID, 'SR_Outstanding', 'lAllowCancel', 0, 1, ''


