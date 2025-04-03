-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

Alter TABLE WarrantyReturnCodesAudit add Deleted CHAR(1)

go

UPDATE WarrantyReturnCodesAudit set Deleted='' where Deleted is null

