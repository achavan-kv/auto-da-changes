-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

INSERT INTO Merchandising.ProductTypeSchema (ProductType, FieldName, DisplayName)
SELECT 'RegularStock', 'CorporateVendorName', 'Corporate Vendor Name'
UNION
SELECT 'RepossessedStock', 'CorporateVendorName', 'Corporate Vendor Name'



UPDATE Merchandising.ProductTypeSchema
SET fieldname = 'CorporateVendorCode', DisplayName = 'Corporate Vendor Code'
WHERE fieldname = 'VendorCode'

delete from Merchandising.ProductTypeSchema
where fieldname in ('CorporateVendorName', 'CorporateVendorCode')
and ProductType not in ('RegularStock',  'RepossessedStock')