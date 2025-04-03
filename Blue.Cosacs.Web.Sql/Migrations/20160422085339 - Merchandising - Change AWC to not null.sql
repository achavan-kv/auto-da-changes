-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Merchandising.CostPrice
SET AverageWeightedCost = 0 
WHERE AverageWeightedCost IS NULL

ALTER TABLE Merchandising.CostPrice
ALTER COLUMN AverageWeightedCost DECIMAL(19, 4) NOT NULL