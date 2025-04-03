-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Warranty.LinkWarranty ADD CONSTRAINT FK_LinkWarranty_Warranty FOREIGN KEY
(
	WarrantyId
) REFERENCES Warranty.Warranty
(
	Id
) 
	ON UPDATE  NO ACTION 
	ON DELETE  NO ACTION 