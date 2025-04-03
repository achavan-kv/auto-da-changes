-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[PurchaseOrderProduct] ADD [EstimatedDeliveryDate] DATE NULL
EXEC sp_rename 'Merchandising.PurchaseOrderProduct.DeliveryDate', 'RequestedDeliveryDate', 'COLUMN';
EXEC sp_rename 'Merchandising.PurchaseOrder.DeliveryDate', 'RequestedDeliveryDate', 'COLUMN';