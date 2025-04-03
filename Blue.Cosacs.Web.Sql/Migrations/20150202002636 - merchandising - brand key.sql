-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[Product]  WITH CHECK ADD  CONSTRAINT [FK_Product_Brand] FOREIGN KEY([BrandId])
REFERENCES [Merchandising].[Brand] ([Id])
GO

ALTER TABLE [Merchandising].[Product] CHECK CONSTRAINT [FK_Product_Brand]
GO
