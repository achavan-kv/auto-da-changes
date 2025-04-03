-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'PK_StockInfo')
ALTER TABLE [dbo].[StockInfo] DROP CONSTRAINT [PK_StockInfo]
GO

ALTER TABLE [dbo].[StockInfo] ADD  CONSTRAINT [PK_StockInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
	
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'ix_StockInfo_VendEAN_Repo')
DROP INDEX [ix_StockInfo_VendEAN_Repo] ON [dbo].[StockInfo] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_StockInfo_VendEAN_Repo] ON [dbo].[StockInfo] 
(
	[VendorEAN] ASC,
	[RepossessedItem] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

