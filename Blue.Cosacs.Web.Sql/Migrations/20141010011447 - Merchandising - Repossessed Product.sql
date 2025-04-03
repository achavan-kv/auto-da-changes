-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE Merchandising.RepossessedProducts (
	Id int not null primary key references Merchandising.Product(Id),
	OriginalProductId int NOT NULL,
	RepossessedConditionId int NOT NULL
)

ALTER TABLE [Merchandising].[RepossessedProducts] WITH CHECK ADD CONSTRAINT [FK_RepossessedProducts_Product] FOREIGN KEY ([OriginalProductId]) REFERENCES [Merchandising].[Product]([Id])
ALTER TABLE [Merchandising].[RepossessedProducts] WITH CHECK ADD CONSTRAINT [FK_RepossessedProducts_RepossessedCondition] FOREIGN KEY ([RepossessedConditionId]) REFERENCES [Merchandising].[RepossessedConditions]([Id])
ALTER TABLE [Merchandising].[RepossessedProducts] CHECK CONSTRAINT [FK_RepossessedProducts_Product]
ALTER TABLE [Merchandising].[RepossessedProducts] CHECK CONSTRAINT [FK_RepossessedProducts_RepossessedCondition]
CREATE UNIQUE NONCLUSTERED INDEX [IX_RepossessedProducts] ON [Merchandising].[RepossessedProducts] (OriginalProductId, RepossessedConditionId)
