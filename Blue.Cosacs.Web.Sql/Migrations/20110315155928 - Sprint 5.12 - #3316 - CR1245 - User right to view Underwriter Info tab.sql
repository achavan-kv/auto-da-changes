-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(select * from task where taskname = 'Authorise Delivery - View Underwriter Information')
BEGIN
	declare @taskid INT

	select @taskid = MAX(taskid)+1 from dbo.Task

	insert into task (taskid,taskname)
	select @taskid,'Authorise Delivery - View Underwriter Information'

	insert into control (taskid,screen,[control],visible,enabled,parentmenu)
	select @taskid,'ReferralSummary','ViewUwInfoTab',0,1,''
END