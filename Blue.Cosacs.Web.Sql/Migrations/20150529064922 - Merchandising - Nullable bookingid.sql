-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.StockTransferMovement ALTER COLUMN BookingId int NULL

ALTER TABLE Merchandising.StockTransferMovement ADD AverageWeightedCost decimal(19,4) NULL


