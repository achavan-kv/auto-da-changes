-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductHeirarchy]') AND name = N'PK_ProductHeirarchy')
ALTER TABLE [dbo].[ProductHeirarchy] DROP CONSTRAINT [PK_ProductHeirarchy]
GO

ALTER TABLE [dbo].[ProductHeirarchy] ADD  CONSTRAINT [PK_ProductHeirarchy] PRIMARY KEY CLUSTERED 
(
	[CatalogType] ASC,
	[PrimaryCode] ASC,
	[ParentCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO