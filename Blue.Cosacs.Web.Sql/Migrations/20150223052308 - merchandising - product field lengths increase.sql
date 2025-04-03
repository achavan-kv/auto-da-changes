-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.GoodsReceiptDirectProduct ALTER COLUMN [Description] varchar(240) NULL
ALTER TABLE Merchandising.PurchaseOrderProduct ALTER COLUMN [Description] varchar(240) NULL