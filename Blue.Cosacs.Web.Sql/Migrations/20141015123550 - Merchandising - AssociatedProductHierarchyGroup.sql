-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
CREATE TABLE [Merchandising].[AssociatedProductHierarchyGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssociatedProductHierarchyGroupId] [int] NOT NULL,
	[LevelId] [int] NOT NULL,	
	[TagId] [int]
 CONSTRAINT [PK_AssociatedProductHierarchyTag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [Merchandising].[AssociatedProductHierarchyGroup]  WITH CHECK ADD CONSTRAINT [FK_Merchandising_AssociatedProductHierarchyGroup_Level] FOREIGN KEY([LevelId])
REFERENCES [Merchandising].[HierarchyLevel] ([Id])
GO

ALTER TABLE [Merchandising].[AssociatedProductHierarchyGroup]  WITH CHECK ADD CONSTRAINT [FK_Merchandising_AssociatedProductHierarchyGroup_Tag] FOREIGN KEY([TagId])
REFERENCES [Merchandising].[HierarchyTag] ([Id])
GO


CREATE TABLE [Merchandising].[AssociatedProduct](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssociatedProductHierarchyGroupId] [int] NOT NULL,	
	[ProductId] [int]
 CONSTRAINT [PK_AssociatedProduct] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

) ON [PRIMARY]

ALTER TABLE [Merchandising].[AssociatedProduct]  WITH CHECK ADD CONSTRAINT [FK_Merchandising_AssociatedProduct_HierarchyGroup] FOREIGN KEY([AssociatedProductHierarchyGroupId])
REFERENCES [Merchandising].[AssociatedProductHierarchyGroup] ([Id])
GO