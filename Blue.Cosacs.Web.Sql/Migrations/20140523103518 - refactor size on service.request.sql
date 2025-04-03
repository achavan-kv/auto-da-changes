-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Service.Request
	ALTER COLUMN ItemAmount DECIMAL(9, 2) NULL

ALTER TABLE Service.Request
	ALTER COLUMN WarrantyLength TinyInt NULL
	
ALTER TABLE Service.Request
	ALTER COLUMN ResolutionLabourCost DECIMAL(9, 2) NULL
	
ALTER TABLE Service.Request
	ALTER COLUMN LastUpdatedOn SMALLDateTime NULL