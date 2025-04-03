-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantity]') AND name = N'ix_stckquantity')
ALTER TABLE [dbo].[StockQuantity] DROP CONSTRAINT [ix_stckquantity]
GO


IF  NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantity]') AND name = N'PK_StockQuantity')

ALTER TABLE [dbo].[StockQuantity] ADD  CONSTRAINT [PK_StockQuantity] PRIMARY KEY CLUSTERED 
(
	[itemno] ASC,
	[stocklocn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO



IF  NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_lineitem_itemno')
CREATE NONCLUSTERED INDEX [ix_lineitem_itemno] ON [dbo].[lineitem] 
(
	[itemno] ASC,
	[acctno] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO



