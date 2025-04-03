-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
/*
Some SR when they were created the fields below did not get any data because an error on the view (search view)
now they have to be updated
*/
UPDATE Service.Request
SET ManufacturerWarrantyContractNo = data.WarrantyContractNo,
ManufacturerWarrantyNumber = data.WarrantyNumber
FROM 
(
	SELECT s.CustomerAccount, s.WarrantyId, s.WarrantyContractNo, s.WarrantyNumber, s.Id as WarrantySaleId--, WarrantyType, * 
	FROM 
		Warranty.WarrantySale s 
		LEFT JOIN 
		(
				SELECT * FROM Warranty.WarrantySale w where w.WarrantyType = 'F'  
		) sf
		ON s.CustomerAccount = sf.CustomerAccount
		AND s.WarrantyId = sf.WarrantyGroupId
		AND s.WarrantyContractNo != sf.WarrantyContractNo
) Data
WHERE
	ManufacturerWarrantyContractNo IS NULL
	AND WarrantyContractId = Data.WarrantySaleId