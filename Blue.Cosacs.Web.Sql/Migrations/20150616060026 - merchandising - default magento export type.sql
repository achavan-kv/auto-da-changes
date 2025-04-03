-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
update Merchandising.Product set MagentoExportType = 'Not Available Online', OnlineDateAdded = null where MagentoExportType IS NULL