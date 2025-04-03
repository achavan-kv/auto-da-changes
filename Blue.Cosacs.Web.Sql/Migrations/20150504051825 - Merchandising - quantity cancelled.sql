-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[StockTransferProduct] ALTER COLUMN [QuantityCancelled] INT NOT NULL
ALTER TABLE [Merchandising].[StockTransferProduct] ADD  DEFAULT ((0)) FOR [QuantityCancelled]

ALTER TABLE [Merchandising].[StockRequisitionProduct] ALTER COLUMN [QuantityCancelled] INT NOT NULL
ALTER TABLE [Merchandising].[StockRequisitionProduct] ADD  DEFAULT ((0)) FOR [QuantityCancelled]

ALTER TABLE [Merchandising].[StockAllocationProduct] ALTER COLUMN [QuantityCancelled] INT NOT NULL
ALTER TABLE [Merchandising].[StockAllocationProduct] ADD  DEFAULT ((0)) FOR [QuantityCancelled]