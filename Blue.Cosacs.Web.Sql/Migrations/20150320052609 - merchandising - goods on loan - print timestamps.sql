-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.GoodsOnLoan ADD DeliveryPrintedDate Datetime NULL
ALTER TABLE Merchandising.GoodsOnLoan ADD CollectionPrintedDate Datetime NULL