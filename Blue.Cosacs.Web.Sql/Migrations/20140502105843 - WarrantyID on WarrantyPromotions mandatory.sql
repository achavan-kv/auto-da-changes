-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

TRUNCATE TABLE Warranty.WarrantyPromotion
GO

ALTER TABLE Warranty.WarrantyPromotion
	DROP CONSTRAINT FK_WarrantyPromotion_WarrantyId
GO

ALTER TABLE Warranty.WarrantyPromotion
ALTER COLUMN WarrantyId INT NOT NULL
GO 

ALTER TABLE Warranty.WarrantyPromotion ADD CONSTRAINT
	FK_WarrantyPromotion_WarrantyId FOREIGN KEY
	(
	WarrantyId
	) REFERENCES Warranty.Warranty
	(
	Id
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	