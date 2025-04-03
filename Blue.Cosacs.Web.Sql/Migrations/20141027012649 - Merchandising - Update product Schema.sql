-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

truncate table merchandising.ProductTypeSchema

SET IDENTITY_INSERT merchandising.ProductTypeSchema ON;
 

insert into merchandising.ProductTypeSchema ( id,ProductType,FieldName,DisplayName)
select 1,'RegularStock','CorporateUPC','Corporate UPC' union
select 2,'RegularStock','VendorUPC','Vendor UPC' union
select 3,'RegularStock','CorporateVendorCode','Vendor Code' union
select 4,'RegularStock','CorporateVendorName','Vendor Name' union
select 5,'RegularStock','BrandName','Brand Name' union
select 6,'RegularStock','VendorModelNo','Vendor Model Number' union
select 7,'RegularStock','CountryOfOrigin','Country of Origin' union
select 8,'RegularStock','CountryOfDispatch','Country of Dispatch' union
select 9,'RegularStock','CurrencyType','Currency Type' union
select 10,'RegularStock','ForeignCost','Foreign Cost' union
select 11,'RegularStock','Incoterm','Incoterm' union
select 12,'RegularStock','LeadTime','Lead Time' union
select 13,'RegularStock','Voltage','Voltage' union
select 14,'RegularStock','VendorWarranty','Vendor Warranty' union
select 15,'RegularStock','SKUStatusCode','SKU Status Code' union
select 16,'SparePart','CorporateUPC','Corporate UPC' union
select 17,'SparePart','VendorUPC','Vendor UPC' union
select 18,'SparePart','BrandName','Brand Name' union
select 19,'SparePart','VendorModelNo','Vendor Model Number' union
select 20,'SparePart','CurrencyType','Currency Type' union
select 21,'SparePart','ForeignCost','Foreign Cost' union
select 22,'SparePart','CountryOfDispatch','Country of Dispatch' union
select 23,'SparePart','CountryOfOrigin','Country of Origin' union
select 24,'SparePart','Incoterm','Incoterm' union
select 25,'SparePart','LeadTime','Lead Time' union
select 26,'SparePart','VendorWarranty','Vendor Warranty' union
select 27,'RepossessedStock','CorporateUPC','Corporate UPC' union
select 28,'RepossessedStock','VendorUPC','Vendor UPC' union
select 29,'RepossessedStock','CorporateVendorCode','Vendor Code' union
select 30,'RepossessedStock','CorporateVendorName','Vendor Name' union
select 31,'RepossessedStock','BrandName','Brand Name' union
select 32,'RepossessedStock','VendorModelNo','Vendor Model Number' union
select 33,'RepossessedStock','CountryOfOrigin','Country of Origin' union
select 34,'RepossessedStock','CountryOfDispatch','Country of Dispatch' union
select 35,'RepossessedStock','CurrencyType','Currency Type' union
select 36,'RepossessedStock','ForeignCost','Foreign Cost' union
select 37,'RepossessedStock','Incoterm','Incoterm' union
select 38,'RepossessedStock','LeadTime','Lead Time' union
select 39,'RepossessedStock','Voltage','Voltage' union
select 40,'RepossessedStock','VendorWarranty','Vendor Warranty' union
select 41,'RepossessedStock','SKUStatusCode','SKU Status Code' union
select 42,'ProductWithoutStock','CorporateUPC','Corporate UPC' union
select 43,'ProductWithoutStock','VendorUPC','Vendor UPC' union
select 44,'ProductWithoutStock','CorporateVendorCode','Vendor Code' union
select 45,'ProductWithoutStock','CorporateVendorName','Vendor Name' union
select 46,'ProductWithoutStock','BrandName','Brand Name' union
select 47,'ProductWithoutStock','VendorModelNo','Vendor Model Number' union
select 48,'ProductWithoutStock','CountryOfOrigin','Country of Origin' union
select 49,'ProductWithoutStock','CountryOfDispatch','Country of Dispatch' union
select 50,'ProductWithoutStock','CurrencyType','Currency Type' union
select 51,'ProductWithoutStock','ForeignCost','Foreign Cost' union
select 52,'ProductWithoutStock','Incoterm','Incoterm' union
select 53,'ProductWithoutStock','LeadTime','Lead Time' union
select 54,'ProductWithoutStock','Voltage','Voltage' union
select 55,'ProductWithoutStock','VendorWarranty','Vendor Warranty' union
select 56,'ProductWithoutStock','SKUStatusCode','SKU Status Code';

SET IDENTITY_INSERT merchandising.ProductTypeSchema OFF;