-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #18215

update service.request
set WarrantyNumber = si.IUPC
from service.request r
inner join lineitem l on r.Account = l.acctno
inner join stockinfo si on l.ItemID = si.Id
where not exists(select * from lineitem l1
					inner join stockinfo si on l1.ItemID = si.Id		
					where l1.acctno = r.Account
					and (l1.contractno = r.ManufacturerWarrantyContractNo or l1.contractno = r.WarrantyContractNo)
					and r.WarrantyNumber = si.IUPC)
and (l.contractno = r.ManufacturerWarrantyContractNo or l.contractno = r.WarrantyContractNo)