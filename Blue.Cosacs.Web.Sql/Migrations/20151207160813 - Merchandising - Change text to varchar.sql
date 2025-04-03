-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE Merchandising.Product
   ALTER COLUMN Attributes VARCHAR(MAX)

ALTER TABLE Merchandising.PurchaseOrder
   ALTER COLUMN ReferenceNumbers VARCHAR(MAX)
   
ALTER TABLE Merchandising.PurchaseOrder
   ALTER COLUMN Comments VARCHAR(MAX)