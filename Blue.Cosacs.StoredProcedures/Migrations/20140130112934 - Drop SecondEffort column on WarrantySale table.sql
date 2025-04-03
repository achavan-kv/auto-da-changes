-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'WarrantySale' AND  Column_Name = 'SecondEffort'
           AND TABLE_SCHEMA = 'Warranty')
BEGIN
	ALTER TABLE Warranty.WarrantySale drop column SecondEffort
END
