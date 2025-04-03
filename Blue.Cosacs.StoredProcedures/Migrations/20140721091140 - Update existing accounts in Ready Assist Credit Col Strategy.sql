-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

declare @date datetime = getdate()

select 
	a.acctno
into 
	#acctsSettled
from 
	acct a
inner join 
	ReadyAssistDetails ra on a.acctno = ra.AcctNo
where
	a.currstatus = 'S'
	and exists(select * from CMStrategyAcct ca
				where ca.acctno = a.acctno
				and ca.strategy = 'RDYAST'
				and ca.dateto is null)


select 
	a.acctno
into 
	#RACancelled
from 
	acct a
inner join 
	ReadyAssistDetails ra on a.acctno = ra.AcctNo
where
	a.currstatus != 'S'
	and exists(select * from CMStrategyAcct ca
				where ca.acctno = a.acctno
				and ca.strategy = 'RDYAST'
				and ca.dateto is null)
	and ra.Status = 'Cancelled'

update 
	CMStrategyAcct 
set 
	dateto = @date
from 
	CMStrategyAcct ca
inner join 
	#acctsSettled a on a.acctno = ca.acctno
where 
	ca.strategy = 'RDYAST'
	and ca.dateto is null

update 
	CMStrategyAcct 
set 
	dateto = @date
from 
	CMStrategyAcct ca
inner join 
	#RACancelled a on a.acctno = ca.acctno
where 
	ca.strategy = 'RDYAST'
	and ca.dateto is null

