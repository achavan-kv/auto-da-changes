-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


update Warranty.WarrantySale
set WarrantyContractNo= replace(WarrantyContractNo,'M','')
where WarrantyIsFree=0
and WarrantyContractNo is not null

--select min(Id) as ID,InvoiceNumber,itemid,salebranch,WarrantyIsFree,count(*) as nbr
--into #delete
--from Warranty.WarrantySale
--where WarrantyIsFree=1
--group by InvoiceNumber,itemid,salebranch,WarrantyIsFree
--having count(*)>1

--delete Warranty.WarrantySale
--from Warranty.WarrantySale w, #delete d
--where w.id = d.id

select DENSE_RANK() OVER (PARTITION BY InvoiceNumber ORDER BY InvoiceNumber,ItemId,WarrantyGroupId) as GroupId,id 
into #groupId
from Warranty.WarrantySale 
where ItemQuantity is null
order by InvoiceNumber,ItemId,WarrantyGroupId

update Warranty.WarrantySale
set WarrantyGroupId=GroupId
from Warranty.WarrantySale w,#groupId g
where w.id=g.id

update  Warranty.WarrantySale
set ItemQuantity=1
where ItemQuantity is null

update Warranty.WarrantySale
set SoldById= empeenosale
from Warranty.WarrantySale ws, agreement a
where ws.CustomerAccount= a.acctno
and a.agrmtno = cast(SUBSTRING(ws.InvoiceNumber, CHARINDEX(' ',ws.InvoiceNumber, 1) + 1, len(ws.InvoiceNumber) - CHARINDEX(' ',ws.InvoiceNumber, 1)) as int)

update Warranty.WarrantySale
set WarrantyNumber='Manufact', WarrantyContractNo='Man001'
where WarrantyIsFree=1
and WarrantyNumber is null

go

DISABLE TRIGGER trig_lineiteminsertupdate ON dbo.lineitem

update lineitem
set WarrantyGroupId=w.WarrantyGroupId
from Warranty.WarrantySale w,lineitem l
where w.CustomerAccount=l.acctno
and w.WarrantyContractNo=l.contractno
and w.WarrantyIsFree=0
and l.quantity>0

go

Enable TRIGGER trig_lineiteminsertupdate ON dbo.lineitem


