-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE dbo.StoreCardStatus ADD CONSTRAINT
	FK_StoreCardStatus_StoreCardCardStatus_Lookup FOREIGN KEY
	(
	StatusCode
	) REFERENCES dbo.StoreCardCardStatus_Lookup
	(
	Status
	) ON UPDATE  NO ACTION 
	 ON DELETE  NO ACTION 
	
GO