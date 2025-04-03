-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE Service.Request
	ALTER COLUMN ResolutionAdditionalCost MONEY NULL
	
ALTER TABLE Service.Request
	ALTER COLUMN ResolutionTransportCost MONEY NULL
	
ALTER TABLE Service.Request
	ALTER COLUMN DepositRequired MONEY NULL