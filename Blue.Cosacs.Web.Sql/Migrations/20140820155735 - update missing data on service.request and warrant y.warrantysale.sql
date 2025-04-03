-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE service.Request
SET ManufacturerWarrantyContractNo = 'Man001',
ManufacturerWarrantyNumber = 'Manufacturer',
ManWarrantyLength = 0
WHERE
ManufacturerWarrantyContractNo IS NULL

UPDATE Warranty.WarrantySale
set WarrantyLength = 0
WHERE WarrantyContractNo='Man001'