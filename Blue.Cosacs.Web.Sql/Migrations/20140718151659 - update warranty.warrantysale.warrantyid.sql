-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
select warrantynumber, * from Warranty.WarrantySale where WarrantyId is null

UPDATE Warranty.WarrantySale
SET WarrantyId = Data.WarrantyId
FROM 
(
	SELECT w.id AS WarrantyId, ws.id
	FROM Warranty.WarrantySale ws INNER JOIN Warranty.Warranty w ON ws.WarrantyNumber = w.Number AND ws.WarrantyId IS NULL
) Data
WHERE
	Warranty.WarrantySale.Id = Data.Id
