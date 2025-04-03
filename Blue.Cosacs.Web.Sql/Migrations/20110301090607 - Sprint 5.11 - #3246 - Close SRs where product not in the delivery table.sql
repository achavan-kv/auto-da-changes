-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--IP - 01/03/11 - Closing SR's that were previously raised in error against a product that was not delivered for a stock location (not existing in the delivery table)

declare @dateadded datetime
set @dateadded = getdate()

update sr_servicerequest 
set status = 'C',
comments = comments + ' ' + convert(varchar(20), @dateAdded, 103) + ' ' + convert(varchar(12), @dateAdded, 114)  + ' :' + ' Closed by migration script'
from sr_servicerequest s
where not exists(select * from delivery d
					where s.acctno = d.acctno
					and s.productcode = d.itemno 
					and s.stocklocn = d.stocklocn)
and s.status in('N', 'T')
and s.servicetype = 'C'