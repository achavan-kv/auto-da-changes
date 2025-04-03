-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
declare @taskid INT

select @taskid = MAX(taskid)+1 from dbo.Task

insert into task (taskid,taskname)
select @taskid,'Service - Reassign Technician'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'SR_ServiceRequest','lbRaReason',0,1,''

select @taskid = MAX(taskid)+1 from dbo.Task

insert into task (taskid,taskname)
select @taskid,'Installation - Reassign Technician'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'InstManagement','lbRaReason',0,1,''


select @taskid = MAX(taskid)+1 from dbo.Task

insert into task (taskid,taskname)
select @taskid,'Service - Food Loss'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'SR_ServiceRequest','lbFoodLoss',0,1,''