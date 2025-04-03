-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[GoodsOnLoan]  WITH CHECK ADD  CONSTRAINT [FK_GoodsOnLoan_ServiceRequest] FOREIGN KEY(ServiceRequestId)
REFERENCES [Service].[Request] ([Id])
GO

ALTER TABLE [Merchandising].[GoodsOnLoan] CHECK CONSTRAINT [FK_GoodsOnLoan_ServiceRequest]
GO

ALTER TABLE [Merchandising].[GoodsOnLoan]  WITH CHECK ADD  CONSTRAINT [FK_GoodsOnLoan_Location] FOREIGN KEY(StockLocationId)
REFERENCES [Merchandising].[Location] ([Id])
GO

ALTER TABLE [Merchandising].[GoodsOnLoan] CHECK CONSTRAINT [FK_GoodsOnLoan_Location]
GO
