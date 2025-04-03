-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[StockCount] ADD StockAdjustmentId int NULL

ALTER TABLE [Merchandising].[StockCount]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockCount_StockAdjustmentId] FOREIGN KEY([StockAdjustmentId])
REFERENCES [Merchandising].[StockAdjustment](Id)
GO

ALTER TABLE [Merchandising].[StockCount] CHECK CONSTRAINT [FK_Merchandising_StockCount_StockAdjustmentId]
GO