-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
alter table [Merchandising].[PurchaseOrderProduct] ADD PurchaseOrderId INT NOT NULL

ALTER TABLE [Merchandising].[PurchaseOrderProduct]  WITH CHECK ADD  CONSTRAINT [FK_PurchaseOrderProduct_PurchaseOrder] FOREIGN KEY([PurchaseOrderId])
REFERENCES [Merchandising].[PurchaseOrder] ([Id])
GO

ALTER TABLE [Merchandising].[PurchaseOrderProduct] CHECK CONSTRAINT [FK_PurchaseOrderProduct_PurchaseOrder]
GO
