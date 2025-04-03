-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


CREATE TABLE [Merchandising].[HierarchyTag](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[LevelId] [int] NOT NULL,
	[Name] [varchar](100) NOT NULL,
 CONSTRAINT [PK_MerchandisingHierarchyTag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_Merchandising_HierarchyTag] UNIQUE NONCLUSTERED 
(
	[LevelId] ASC,
	[Name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

----Add Foreign Key

ALTER TABLE [Merchandising].[HierarchyTag]  WITH CHECK ADD  CONSTRAINT [FK_Merchandising_HierarchyTag_Level] FOREIGN KEY([LevelId])
REFERENCES [Merchandising].[HierarchyLevel] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [Merchandising].[HierarchyTag] CHECK CONSTRAINT [FK_Merchandising_HierarchyTag_Level]
GO


