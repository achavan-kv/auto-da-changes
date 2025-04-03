-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT 1 FROM [$Migration] WHERE id in (20140916094851, 20140916094852))
BEGIN

update 
	lineitem
set 
	itemno = i.IUPC
from 
	lineitem l
inner join
	(
		select distinct
			sr.Account, 
			sr.ItemStockLocation, 
			si.iupc, 
			sr.ItemId
		from 
			service.Request sr
		inner join 
			stockinfo si on sr.ItemId = si.Id
		inner join 
			lineitem l on l.acctno = sr.Account
			and l.ItemID = sr.ItemId
			and l.stocklocn = sr.ItemStockLocation
			and l.itemno != sr.ItemNumber
			and l.itemno != si.IUPC
		where 
			sr.ItemNumber = si.IUPC
			and sr.CustomerId != 'PAID & TAKEN'
			and len(l.itemno) < 12
			and len(sr.ItemNumber) < 12	
	) i on l.acctno = i.Account
	and l.ItemID = i.ItemId
	and l.stocklocn = i.ItemStockLocation 


END
