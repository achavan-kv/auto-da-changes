-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
UPDATE Merchandising.StockTransferProduct set QuantityCancelled = 0 where QuantityCancelled IS NULL
UPDATE Merchandising.StockRequisitionProduct set QuantityCancelled = 0 where QuantityCancelled IS NULL
UPDATE Merchandising.StockAllocationProduct set QuantityCancelled = 0 where QuantityCancelled IS NULL