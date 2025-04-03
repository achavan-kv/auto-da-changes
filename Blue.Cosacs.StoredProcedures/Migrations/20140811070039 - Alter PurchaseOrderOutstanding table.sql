-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PurchaseOrderOutstanding' AND  Column_Name = 'quantityonorder')
BEGIN
	ALTER TABLE PurchaseOrderOutstanding ALTER COLUMN quantityonorder int not null 
END
GO


IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE Table_Name = 'PurchaseOrderOutstanding' AND  Column_Name = 'quantityavailable')
BEGIN
	ALTER TABLE PurchaseOrderOutstanding ALTER COLUMN quantityavailable int not null 
END
