-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--First update WarrantyContractNo on non Cash & Go accounts

select account, itemid, itemstocklocation, warrantycontractno 
into #ContractNonCG 
from Service.Request s
where account is not null 
and WarrantyContractNo is null
and exists(select * from delivery d
			where d.acctno = s.Account
			and d.ParentItemId = s.ItemId
			and d.stocklocn = s.ItemStockLocation
			and d.contractno != ''
			and d.delorcoll = 'D'
			) 

update Service.request
set WarrantyContractNo = d.contractno
from Service.request s
	 inner join #ContractNonCG c on s.account = c.account
	 and s.itemid = c.itemid
	 and s.itemstocklocation = c.itemstocklocation
	 inner join delivery d on 
	 d.acctno = c.Account
	 and d.ParentItemId = c.ItemId
	 and d.stocklocn = c.ItemStockLocation
	 and d.contractno != ''
	 and d.delorcoll = 'D'
where s.Account is not null 	
and s.WarrantyContractNo is null

--Secondly update WarrantyContractNo on Cash & Go accounts

select Substring(s.InvoiceNumber,CHARINDEX('/', s.InvoiceNumber, 0) + 1, len(s.InvoiceNumber) - CHARINDEX('/', s.InvoiceNumber, 0)) as AgrmtNo,
	s.InvoiceNumber,
	s.itemid, 
	s.itemstocklocation, 
	s.warrantycontractno 
into #CGInvoiceNo
from Service.Request s
where s.InvoiceNumber is not null and s.account is null
and s.WarrantyContractNo is null


select c.AgrmtNo,
	   c.InvoiceNumber,
	   c.itemid, 
	   c.itemstocklocation, 
	   c.warrantycontractno 
into #ContractCG
from #CGInvoiceNo c inner join Service.Request s on s.InvoiceNumber = c.InvoiceNumber
	and s.ItemId = c.ItemId
	and s.ItemStockLocation = c.ItemStockLocation
inner join SR_ServiceRequest sr on sr.InvoiceNo = c.agrmtno
	and sr.ItemId = c.ItemId
	and sr.StockLocn = c.ItemStockLocation
	and exists(select * from delivery d
			where d.agrmtno = sr.InvoiceNo
			and d.ParentItemId = sr.ItemId
			and d.stocklocn = sr.StockLocn
			and d.contractno != ''
			and d.delorcoll = 'D')
where s.InvoiceNumber is not null and s.account is null
and s.WarrantyContractNo is null


update Service.request
set WarrantyContractNo = d.contractno
from Service.request s
 inner join #ContractCG c on s.InvoiceNumber = c.InvoiceNumber
	and s.ItemId = c.ItemId
	and s.ItemStockLocation = c.ItemStockLocation
inner join SR_ServiceRequest sr on sr.InvoiceNo = c.agrmtno
and sr.ItemId = c.ItemId
and sr.StockLocn = c.ItemStockLocation
inner join delivery d on d.agrmtno = sr.InvoiceNo
			and d.ParentItemId = sr.ItemId
			and d.stocklocn = sr.StockLocn
			and d.contractno != ''
			and d.delorcoll = 'D'
where s.InvoiceNumber is not null and s.account is null
and s.WarrantyContractNo is null


