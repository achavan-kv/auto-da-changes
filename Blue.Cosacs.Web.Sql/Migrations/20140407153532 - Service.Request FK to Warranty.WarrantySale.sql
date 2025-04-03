-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
UPDATE Service.Request
SET WarrantyContractId = NULL

UPDATE Service.Request
SET WarrantyContractId = Data.WarrantySaleId
FROM
(
	SELECT 
		ws.Id As WarrantySaleId, sr.Id AS ServiceRequestId
	FROM
		Warranty.Warrantysale AS Ws
		INNER JOIN Service.Request AS Sr 
			ON Sr.Itemnumber = Ws.Itemnumber 
			AND ws.WarrantyContractNo  = sr.WarrantyContractNo
			AND Sr.Account = Ws.Customeraccount
) Data
WHERE
	Id = Data.ServiceRequestId
GO

ALTER TABLE Service.Request ADD CONSTRAINT
	FK_Request_WarrantySale FOREIGN KEY
	(
		WarrantyContractId
	) REFERENCES Warranty.WarrantySale
	(
		Id
	) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 