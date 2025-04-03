declare @taskid INT

select @taskid = MAX(taskid)+1 from dbo.Task

insert into task (taskid,taskname)
select @taskid,'Installation - Can Close Installation'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'InstManagement','CloseInstallation',0,1,''

UPDATE dbo.Control
SET [Control] = 'RebookTechnician'
WHERE Control = 'lbRaReason' AND Screen = 'InstManagement'