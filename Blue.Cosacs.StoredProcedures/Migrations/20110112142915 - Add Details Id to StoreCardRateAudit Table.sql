-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.columns WHERE table_name ='StoreCardRateAudit' AND column_name = 'DetailsId')
BEGIN
DELETE FROM  StoreCardRateAudit
ALTER TABLE dbo.StoreCardRateAudit ADD DetailsId INT  NOT NULL 
END 

