-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15639

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'StockInfo' AND  Column_Name = 'WarrantyIsFree')
BEGIN
	ALTER TABLE StockInfo ADD WarrantyIsFree bit NULL
END
GO
