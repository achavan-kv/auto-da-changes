-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE [Merchandising].[AssociatedProduct]
   DROP [FK_Merchandising_AssociatedProduct_HierarchyGroup]
GO

ALTER TABLE [Merchandising].[AssociatedProduct]
	DROP COLUMN [AssociatedProductHierarchyGroupId]	
GO

ALTER TABLE [Merchandising].[AssociatedProduct]
	ADD [AssociatedHierarchy] varchar(max)
GO

DROP TABLE [Merchandising].[AssociatedProductHierarchyGroup]
GO
 