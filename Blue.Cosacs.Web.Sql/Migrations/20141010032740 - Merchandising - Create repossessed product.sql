-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE Merchandising.RepossessedProduct (
	Id int not null primary key references Merchandising.Product(Id),
	OriginalProductId int NOT NULL,
	RepossessedConditionId int NOT NULL
)

ALTER TABLE [Merchandising].[RepossessedProduct] WITH CHECK ADD CONSTRAINT [FK_RepossessedProduct_Product] FOREIGN KEY ([OriginalProductId]) REFERENCES [Merchandising].[Product]([Id])
ALTER TABLE [Merchandising].[RepossessedProduct] WITH CHECK ADD CONSTRAINT [FK_RepossessedProduct_RepossessedCondition] FOREIGN KEY ([RepossessedConditionId]) REFERENCES [Merchandising].[RepossessedCondition]([Id])
ALTER TABLE [Merchandising].[RepossessedProduct] CHECK CONSTRAINT [FK_RepossessedProduct_Product]
ALTER TABLE [Merchandising].[RepossessedProduct] CHECK CONSTRAINT [FK_RepossessedProduct_RepossessedCondition]
CREATE UNIQUE NONCLUSTERED INDEX [IX_RepossessedProduct] ON [Merchandising].[RepossessedProducts] (OriginalProductId, RepossessedConditionId)
