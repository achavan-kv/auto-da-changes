-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

CREATE TABLE [Merchandising].[HierarchyTagCondition](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HierarchyTagId] [int] NOT NULL,
	[Condition] [varchar](100) NOT NULL,
	[Percentage] [decimal](9, 8) NULL,
 CONSTRAINT [PK_HierarchyTagCondition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


ALTER TABLE [Merchandising].[HierarchyTagCondition]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyTagCondition_HierarchyTag] FOREIGN KEY([HierarchyTagId])
REFERENCES [Merchandising].[HierarchyTag] ([Id])

ALTER TABLE [Merchandising].[HierarchyTagCondition] CHECK CONSTRAINT [FK_HierarchyTagCondition_HierarchyTag]


ALTER TABLE Merchandising.HierarchyTag ADD
	FirstYearWarrantyProvision decimal(9, 8) NULL
	