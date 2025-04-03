-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @nid int

select @nid = MAX(taskid)
from task

insert into Task
select @nid+1, 'Authorise Instant Credit - Clear Proposal' union all
select @nid+2, 'Authorise Instant Credit' union all
select @nid+3, 'Authorise Instant Credit - Load Other Branches' union all
select @nid+4, 'View Proposal from IC' 

insert into control
(TaskID, Screen, Control, Visible, Enabled, ParentMenu)
select taskid, 'MainForm', 'menuAuthoriseIC', 1, 1, 'menuCustomer'
from task where TaskName = 'Authorise Instant Credit'

insert into control
(TaskID, Screen, Control, Visible, Enabled, ParentMenu)
select taskid, 'ICAuthorisation', 'clearProposal', 0, 1, ''
from task where TaskName = 'Authorise Instant Credit - Clear Proposal'

insert into control
(TaskID, Screen, Control, Visible, Enabled, ParentMenu)
select taskid, 'ICAuthorisation', 'viewproposal', 1, 1, ''
from task where TaskName = 'View Proposal from IC'

insert into control
(TaskID, Screen, Control, Visible, Enabled, ParentMenu)
select taskid, 'ICAuthorisation', 'loadBranches', 0, 1, ''
from task where TaskName = 'Authorise Instant Credit - Load Other Branches'
