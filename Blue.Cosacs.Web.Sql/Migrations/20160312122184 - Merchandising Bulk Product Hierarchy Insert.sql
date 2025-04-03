CREATE TABLE [Merchandising].[ProductHierarchyStaging](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[HierarchyTagId] [int] NOT NULL,
	[HierarchyLevelId] [int] NOT NULL,
 CONSTRAINT [PK_ProductHierarchyStaging] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
))