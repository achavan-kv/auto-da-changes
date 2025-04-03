-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

update d
set parentitemno = l.parentitemno
from delivery d
inner join lineitem l on l.itemno = d.itemno
	and l.stocklocn = d.stocklocn
	and l.acctno = d.acctno
	and l.contractno = d.contractno
	and l.agrmtno = d.agrmtno
where l.itemno in (select componentno from kitproduct)