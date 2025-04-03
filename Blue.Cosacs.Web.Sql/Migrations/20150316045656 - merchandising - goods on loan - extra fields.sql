-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.GoodsOnLoan ADD CreatedById INT NOT NULL
ALTER TABLE Merchandising.GoodsOnLoan ADD CreatedOn DateTime NOT NULL


ALTER TABLE [Merchandising].GoodsOnLoan  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_GoodsOnLoan_CreatedBy] FOREIGN KEY([CreatedById])
REFERENCES [Admin].[User] ([Id])
GO

ALTER TABLE [Merchandising].GoodsOnLoan CHECK CONSTRAINT [FK_Merchandising_GoodsOnLoan_CreatedBy]
GO

