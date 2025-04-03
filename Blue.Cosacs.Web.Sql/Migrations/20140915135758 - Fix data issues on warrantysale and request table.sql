-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

select distinct 
	sr.WarrantyNumber, 
	si.itemdescr1, 
	(cast(RIGHT(sr.WarrantyNumber, 1) as int) * 12) - 12 as WarrantyLength, 
	si.taxrate, 
	case 
		when sq.deleted = 'N' 
		then 0
		else 1
	end as deleted
into 
	#warrantiesToAdd
from 
	Service.Request sr
inner join 
	stockinfo si on si.IUPC = sr.WarrantyNumber
	and si.category in (12, 82)
inner join 
	StockQuantity sq on si.Id = sq.ID
	and sr.ItemStockLocation = sq.stocklocn
where 
	sr.WarrantyLength is null
	and sr.WarrantyNumber is not null
	and not exists(select * from warranty.warranty w
				where w.Number = sr.WarrantyNumber)


insert into 
	Warranty.Warranty
select 
	w.WarrantyNumber, 
	w.itemdescr1, 
	w.WarrantyLength, 
	w.taxrate, 
	max(w.deleted), 
	'E'
from 
	#warrantiesToAdd w
where 
	not exists(select * from Warranty.Warranty ww
					where ww.Number = w.WarrantyNumber)
group by 
	w.WarrantyNumber, w.itemdescr1, w.WarrantyLength, w.taxrate
union 
select 
	w.WarrantyNumber + 'M', 
	'Free ' + w.itemdescr1,
	 12,
	 0,
	 max(w.deleted),
	 'F'
from 
	#warrantiesToAdd w
where 
	not exists(select * from Warranty.Warranty ww
					where ww.Number + 'M' = w.WarrantyNumber)
group by 
	w.WarrantyNumber, w.itemdescr1, w.WarrantyLength, w.taxrate



update 
	Service.Request
set 
	WarrantyLength = w.[length]
from 
	Warranty.Warranty w
where 
	Service.Request.WarrantyLength is null and WarrantyContractNo is not null
	and w.Number = Service.Request.WarrantyNumber

