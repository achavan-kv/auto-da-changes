-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
exec sp_rename 'Merchandising.PurchaseOrder.ReceivingLocationName', 'ReceivingLocation', 'COLUMN'
exec sp_rename 'Merchandising.PurchaseOrder.VendorName', 'Vendor', 'COLUMN'