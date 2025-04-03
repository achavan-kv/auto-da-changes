-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15639

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'WarrantySale' AND  Column_Name = 'ItemQuantity'
           AND TABLE_SCHEMA = 'Warranty')
BEGIN
	ALTER TABLE Warranty.WarrantySale ADD ItemQuantity int NULL
END

