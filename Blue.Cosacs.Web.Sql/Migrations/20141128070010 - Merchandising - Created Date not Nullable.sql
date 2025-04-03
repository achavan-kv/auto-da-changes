-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Merchandising.GoodsReceipt
SET CreatedDate = getdate()
WHERE CreatedDate IS NULL


ALTER TABLE Merchandising.GoodsReceipt
ALTER COLUMN CreatedDate DATETIME NOT NULL