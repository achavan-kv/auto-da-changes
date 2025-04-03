-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE Merchandising.ProductTypeSchema
SET fieldname = 'Incoterm', DisplayName = 'Incoterm'
WHERE fieldname = 'IncotermName'

INSERT INTO Merchandising.ProductTypeSchema (ProductType, FieldName, DisplayName)
SELECT 'RegularStock', 'SKUStatusCode', 'SKU Status Code'
UNION
SELECT 'RepossessedStock', 'SKUStatusCode', 'SKU Status Code'
