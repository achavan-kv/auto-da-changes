-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Merchandising.ProductStockLevel 
set StockOnOrder = 0
where StockOnOrder < 0

ALTER TABLE Merchandising.ProductStockLevel
ADD CONSTRAINT chk_StockOnOrder CHECK (StockOnOrder >= 0)