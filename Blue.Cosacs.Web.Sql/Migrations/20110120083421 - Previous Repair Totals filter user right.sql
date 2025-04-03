-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taskid INT

select @taskid = MAX(taskid)+1 from dbo.Task

insert into task (taskid,taskname)
select @taskid,'Service - Previous Repair Totals'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'SR_Outstanding','lPreviousRepair',0,1,''