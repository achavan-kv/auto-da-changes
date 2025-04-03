-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.CintOrder ALTER COLUMN Tax decimal(18,4)
ALTER TABLE Merchandising.CintOrder ALTER COLUMN Price decimal(18,4)