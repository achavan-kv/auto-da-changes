-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15183

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'Lineitem' AND  Column_Name = 'WarrantyGroupId')
BEGIN
	ALTER TABLE Lineitem ADD WarrantyGroupId int NULL
END

