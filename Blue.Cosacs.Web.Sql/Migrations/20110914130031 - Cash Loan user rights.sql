-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @taskid INT

select @taskid = MAX(taskid)+1 from dbo.Task

IF NOT EXISTS(select * from task where taskname = 'Cash Loan Application')
begin
insert into task (taskid,taskname)
select @taskid,'Cash Loan Application'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'MainForm','menuCashLoanApplication',1,1,'menuCustomer'
-- show menu item for application screen
insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'CashLoanApplication','AllowApplication',1,1,''
end

select @taskid = MAX(taskid)+1 from dbo.Task

IF NOT EXISTS(select * from task where taskname = 'Cash Loan Disbursement')
begin
insert into task (taskid,taskname)
select @taskid,'Cash Loan Disbursement'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'CashLoanApplication','AllowDisburseCashLoan',1,1,''
-- show menu item for application screen
insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'MainForm','menuCashLoanApplication',1,1,'menuCustomer'

end

select @taskid = MAX(taskid)+1 from dbo.Task

IF NOT EXISTS(select * from task where taskname = 'Cash Loan Overide')
begin
insert into task (taskid,taskname)
select @taskid,'Cash Loan Overide'

insert into control (taskid,screen,[control],visible,enabled,parentmenu)
select @taskid,'BasicCustomerDetails','menuCashLoanOveride',1,1,''

end

