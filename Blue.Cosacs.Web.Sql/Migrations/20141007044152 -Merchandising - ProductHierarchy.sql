-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
CREATE TABLE [Merchandising].[ProductHierarchy] (
	Id int NOT NULL IDENTITY(1,1),
	ProductId int NOT NULL,
	HierarchyTagId int NOT NULL,
	CONSTRAINT [PK_ProductHierarchy] PRIMARY KEY (Id ASC))

ALTER TABLE [Merchandising].[ProductHierarchy] WITH CHECK ADD CONSTRAINT [FK_ProductHierarchy_Product] FOREIGN KEY ([ProductId]) REFERENCES [Merchandising].[Product]([Id])
ALTER TABLE [Merchandising].[ProductHierarchy] CHECK CONSTRAINT [FK_ProductHierarchy_Product]

ALTER TABLE [Merchandising].[ProductHierarchy] WITH CHECK ADD CONSTRAINT [FK_ProductHierarchy_HierarchyTag] FOREIGN KEY ([HierarchyTagId]) REFERENCES [Merchandising].[HierarchyTag]([Id])
ALTER TABLE [Merchandising].[ProductHierarchy] CHECK CONSTRAINT [FK_ProductHierarchy_HierarchyTag]