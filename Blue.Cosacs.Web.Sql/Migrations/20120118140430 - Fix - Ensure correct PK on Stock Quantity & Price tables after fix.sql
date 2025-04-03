-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockPrice]') AND name = N'PK_StockPrice')
ALTER TABLE [dbo].[StockPrice] DROP CONSTRAINT [PK_StockPrice]
GO

ALTER TABLE [dbo].[StockPrice] ADD  CONSTRAINT [PK_StockPrice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[branchno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantity]') AND name = N'PK_StockQuantity')
ALTER TABLE [dbo].[StockQuantity] DROP CONSTRAINT [PK_StockQuantity]
GO

ALTER TABLE [dbo].[StockQuantity] ADD  CONSTRAINT [PK_StockQuantity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[stocklocn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
