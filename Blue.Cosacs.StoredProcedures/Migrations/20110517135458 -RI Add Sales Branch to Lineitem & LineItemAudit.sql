-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SalesBrnNo'
               AND OBJECT_NAME(id) = 'LineItem')
BEGIN
  ALTER TABLE LineItem ADD SalesBrnNo INT 
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SalesBrnNo'
               AND OBJECT_NAME(id) = 'LineItemAudit')
BEGIN
  ALTER TABLE LineItemAudit ADD SalesBrnNo INT 
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SalesBrnNo'
               AND OBJECT_NAME(id) = 'LineItem_Amend')
BEGIN
  ALTER TABLE LineItem_Amend ADD SalesBrnNo INT 
END
go

