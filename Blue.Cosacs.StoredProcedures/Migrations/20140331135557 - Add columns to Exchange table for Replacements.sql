-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Exchange' AND  Column_Name = 'CollectionType')
BEGIN
	ALTER TABLE Exchange Add CollectionType char(1) null
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Exchange' AND  Column_Name = 'WarrantyGroupId')
BEGIN
	ALTER TABLE Exchange Add WarrantyGroupId int null
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Exchange' AND  Column_Name = 'LinkIrwId')
BEGIN
	ALTER TABLE Exchange Add LinkIrwId int null
END

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Exchange' AND  Column_Name = 'LinkIrwContractno')
BEGIN
	ALTER TABLE Exchange Add LinkIrwContractno varchar(10) null
END 

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Exchange' AND  Column_Name = 'ReplacementItemId')
BEGIN
	ALTER TABLE Exchange Add ReplacementItemId int null
END 

--IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Exchange' AND  Column_Name = 'ReplacementMonths')
--BEGIN
--	ALTER TABLE Exchange Add ReplacementMonths int null
--END

