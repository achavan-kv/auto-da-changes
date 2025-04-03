-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
--SET Values for WarrantySale.WarrantyID

UPDATE Warranty.WarrantySale
SET WarrantyID = Data.WarrantyID
FROM
(
	select 
		s.id,
		i.Id AS WarrantyID
	from 
		Warranty.WarrantySale s
		left join StockInfo i
			on s.WarrantyNumber = i.IUPC
	where 
		s.WarrantyID IS NULL
) Data
WHERE
	Warranty.WarrantySale.id = Data.Id
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
GO

ALTER TABLE Warranty.WarrantyPotentialSale ADD CONSTRAINT
	FK_WarrantyPotentialSale_Warranty FOREIGN KEY
	(
		WarrantyId
	) REFERENCES Warranty.Warranty
	(
		Id
	) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
	
GO

ALTER TABLE Warranty.WarrantySale ADD CONSTRAINT
	FK_WarrantySale_StockInfo FOREIGN KEY
	(
		WarrantyId
	) REFERENCES StockInfo
	(
		Id
	) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 
	
GO
