-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update warranty.warrantysale
set WarrantyNumber = WarrantyNumber + 'M'
where right(WarrantyNumber, 1) != 'M'
and WarrantyType = 'F'
and WarrantyNumber != 'Manufacturer'
and exists(select * from warranty.Warranty f
				where f.Number = LTRIM(RTRIM(warranty.WarrantySale.WarrantyNumber)) + 'M')


update warranty.WarrantySale
set WarrantyId = wf.Id
from warranty.warranty wf
where warranty.WarrantySale.WarrantyNumber = wf.Number
and warranty.WarrantySale.WarrantyId != wf.Id
and warranty.WarrantySale.WarrantyType = 'F'
and warranty.WarrantySale.WarrantyNumber != 'Manufacturer'


update service.Request
set ManufacturerWarrantyNumber = ManufacturerWarrantyNumber + 'M'
where exists(select * from warranty.warranty wf
				where wf.Number = LTRIM(RTRIM(service.Request.ManufacturerWarrantyNumber)) + 'M')
and right(ManufacturerWarrantyNumber,1) != 'M'
and ManufacturerWarrantyNumber != 'Manufacturer'


update service.Request
set ManufacturerWarrantyContractNo = ManufacturerWarrantyContractNo + 'M'
where exists(select * from Warranty.WarrantySale ws
				where ws.CustomerAccount = service.Request.Account
				and ws.WarrantyType = 'F'
				and ws.WarrantyContractNo = service.Request.ManufacturerWarrantyContractNo + 'M')
and right(ManufacturerWarrantyContractNo,1) != 'M'

--Find warranties that are on a Service Request and do not exist on warranty.warranty. Need to do this first to then later update the length on the Request table.
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
	service.Request sr
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
	and sr.ManufacturerWarrantyNumber!= 'Manufacturer'


insert into warranty.warranty
select w.WarrantyNumber, w.itemdescr1, w.WarrantyLength, w.taxrate, max(w.deleted), 'E'
from #warrantiesToAdd w
where not exists(select * from warranty.warranty ww
					where ww.Number = w.WarrantyNumber)
group by w.WarrantyNumber, w.itemdescr1, w.WarrantyLength, w.taxrate
union 
select w.WarrantyNumber + 'M', 'Free ' + w.itemdescr1, 12, 0, max(w.deleted), 'F'
from #warrantiesToAdd w
where not exists(select * from warranty.warranty ww
					where ww.Number + 'M' = w.WarrantyNumber)
group by  w.WarrantyNumber, w.itemdescr1, w.WarrantyLength, w.taxrate

update service.Request
set WarrantyLength = w.[length]
from warranty.Warranty w
where service.Request.WarrantyLength is null and WarrantyContractNo is not null
and w.Number = service.request.WarrantyNumber

