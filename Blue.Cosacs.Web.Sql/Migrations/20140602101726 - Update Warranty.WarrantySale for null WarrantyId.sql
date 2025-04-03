-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


--Update Warranty.WarrantySale where WarrantyNumber incorrect
update 
	Warranty.WarrantySale
set 
	WarrantyNumber = 'Manufacturer'
where
	WarrantyNumber = 'Manufact'
and WarrantyContractNo = 'Man001'

update 
	Warranty.WarrantySale
set 
	WarrantyId = w.Id
from 
	warranty.Warranty w
inner join 
	Warranty.WarrantySale ws on ws.WarrantyNumber = w.Number
where ws.WarrantyId is null
