-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.


--Insert existing Ready Assist accounts into ReadyAssistDetails (Active)
insert into ReadyAssistDetails
select 
	l.acctno, l.agrmtno, d.datedel, c.reference, l.itemid, l.contractno, 'Active', null
from 
	lineitem l
inner join 
	stockinfo si on l.itemid = si.id
inner join
	 code c on c.code = si.iupc
inner join
	 delivery d on d.acctno = l.acctno
	 and d.agrmtno = l.agrmtno
	 and d.itemid = l.itemid 
	 and d.contractno = l.contractno	
	 and d.delorcoll = 'D'
	 and d.datetrans = (select max(datetrans)
							from delivery d1
								where d1.acctno = d.acctno
								and d1.agrmtno = d.agrmtno
								and d1.itemid = d.itemid
								and d1.contractno = d.contractno
								and d1.delorcoll = d.delorcoll)
where 
	c.category = 'RDYAST'
	and l.quantity > 0
	and not exists (select * from ReadyAssistDetails r
					where r.acctno = l.acctno
					and r.agrmtno = l.agrmtno
					and r.itemid = l.itemid
					and r.contractno = l.contractno)
			
and l.agrmtno = 1


--Insert existing Ready Assist accounts into ReadyAssistDetails (Cancelled)
insert into ReadyAssistDetails
select 
	l.acctno, l.agrmtno, del.datedel, c.reference, l.itemid, l.contractno, 'Cancelled', null
from 
	lineitem l
inner join 
	stockinfo si on l.itemid = si.id
inner join
	 code c on c.code = si.iupc
inner join
	 delivery col on col.acctno = l.acctno
	 and col.agrmtno = l.agrmtno
	 and col.itemid = l.itemid 
	 and col.contractno = l.contractno
	 and col.delorcoll = 'C'
inner join 
	delivery del on del.acctno = l.acctno
	and del.agrmtno = l.agrmtno
	and del.itemid = l.itemid
	and del.contractno = l.contractno
	and del.delorcoll = 'D'
	and del.datetrans = (select 
							max(datetrans)
						 from 
							delivery d
						  where d.acctno = del.acctno
						  and d.agrmtno = del.agrmtno
						  and d.itemid = del.itemid
						  and d.contractno = del.contractno
						  and d.delorcoll = del.delorcoll)
	
where 
	c.category = 'RDYAST'
	and l.quantity = 0
	and not exists (select * from ReadyAssistDetails r
					where r.acctno = l.acctno
					and r.agrmtno = l.agrmtno
					and r.itemid = l.itemid
					and r.contractno = l.contractno)
			
and l.agrmtno = 1


--Insert existing Ready Assist accounts into ReadyAssistDetails (Cancelled through repossession)
insert into ReadyAssistDetails
select 
	l.acctno, l.agrmtno, del.datedel, c.reference, l.itemid, l.contractno, 'Cancelled', null
from 
	lineitem l
inner join 
	stockinfo si on l.itemid = si.id
inner join
	 code c on c.code = si.iupc
inner join
	 delivery rep on rep.acctno = l.acctno
	 and rep.agrmtno = l.agrmtno
	 and rep.itemid = l.itemid 
	 and rep.contractno = l.contractno
	 and rep.delorcoll = 'R'
	 and rep.quantity < 0
inner join 
	delivery del on del.acctno = l.acctno
	and del.agrmtno = l.agrmtno
	and del.itemid = l.itemid
	and del.contractno = l.contractno
	and del.delorcoll = 'D'
	and del.datetrans = (select 
							max(datetrans)
						 from 
							delivery d
						  where d.acctno = del.acctno
						  and d.agrmtno = del.agrmtno
						  and d.itemid = del.itemid
						  and d.contractno = del.contractno
						  and d.delorcoll = del.delorcoll)
where 
	c.category = 'RDYAST'
	and l.quantity = 0
	and not exists (select * from ReadyAssistDetails r
					where r.acctno = l.acctno
					and r.agrmtno = l.agrmtno
					and r.itemid = l.itemid
					and r.contractno = l.contractno)
			
and l.agrmtno = 1

