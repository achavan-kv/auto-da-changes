-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
--

ALTER TABLE [Warranty].[WarrantyReturnFilter]
DROP CONSTRAINT [FK_WarrantyReturnFilter_WarrantyLevel]

ALTER TABLE [Warranty].[WarrantyReturnFilter]
DROP COLUMN [LevelId]
