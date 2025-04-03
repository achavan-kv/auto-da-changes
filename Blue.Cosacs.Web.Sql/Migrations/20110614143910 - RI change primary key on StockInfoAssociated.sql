-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfoAssociated]') AND name = N'PK_StockInfoAssociated')
ALTER TABLE [dbo].[StockInfoAssociated] DROP CONSTRAINT [PK_StockInfoAssociated]
GO

ALTER TABLE [dbo].[StockInfoAssociated] ADD  CONSTRAINT [PK_StockInfoAssociated] PRIMARY KEY CLUSTERED 
(
	[ProductGroup] ASC,
	[Category] ASC,
	[Class] ASC,
	[SubClass] ASC,
	[AssocItemid] asc
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


