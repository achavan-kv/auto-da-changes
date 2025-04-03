-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.StockTransfer drop column CreatedBy
ALTER TABLE Merchandising.StockTransfer add CreatedById int NOT NULL

ALTER TABLE [Merchandising].[StockTransfer]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_StockTransfer_CreatedBy] FOREIGN KEY([CreatedById])
REFERENCES [Admin].[User] ([Id])
GO

ALTER TABLE [Merchandising].[StockTransfer] CHECK CONSTRAINT [FK_Merchandising_StockTransfer_CreatedBy]
GO


