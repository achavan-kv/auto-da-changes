-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[GoodsOnLoanProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_GoodsOnLoanProduct_Product] FOREIGN KEY([ProductId])
REFERENCES [Merchandising].[Product] ([Id])
GO

ALTER TABLE [Merchandising].[GoodsOnLoanProduct] CHECK CONSTRAINT [FK_Merchandising_GoodsOnLoanProduct_Product]
GO

ALTER TABLE [Merchandising].[GoodsOnLoanProduct]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_GoodsOnLoanProduct_GoodsOnLoan] FOREIGN KEY([GoodsOnLoanId])
REFERENCES [Merchandising].[GoodsOnLoan] ([Id])
GO

ALTER TABLE [Merchandising].[GoodsOnLoanProduct] CHECK CONSTRAINT [FK_Merchandising_GoodsOnLoanProduct_GoodsOnLoan]
GO