-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taskid INT

select @taskid = MAX(taskid)+1 from dbo.Task

insert into task (taskid,taskname)
select @taskid,'Non Stock Maintenance'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'MainForm','menuNonStock',1,1,'menuSysMaint'


select @taskid = MAX(taskid)+1 from dbo.Task

insert into task (taskid,taskname)
select @taskid,'Non Stock Maintenance - Edit'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'MainForm','menuNonStock',1,1,'menuSysMaint'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'NonStock','btnSave',1,1,''

