-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
UPDATE Merchandising.StockTransferMovement Set AverageWeightedCost = 0 where AverageWeightedCost IS NULL

ALTER TABLE Merchandising.StockTransferMovement ALTER COLUMN  AverageWeightedCost decimal(19,4) NOT NULL