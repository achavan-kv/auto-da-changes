-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE merchandising.product DROP COLUMN BoxCount
ALTER TABLE merchandising.product ADD BoxCount int NOT NULL DEFAULT(1)
