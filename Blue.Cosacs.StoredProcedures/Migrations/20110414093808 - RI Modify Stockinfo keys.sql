-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_StockPrice_StockInfo]') AND parent_object_id = OBJECT_ID(N'[dbo].[StockPrice]'))
ALTER TABLE [dbo].[StockPrice] DROP CONSTRAINT [fk_StockPrice_StockInfo]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_StockQuantity_StockInfo]') AND parent_object_id = OBJECT_ID(N'[dbo].[StockQuantity]'))
ALTER TABLE [dbo].[StockQuantity] DROP CONSTRAINT [fk_StockQuantity_StockInfo]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_SDelivery_StockInfo]') AND parent_object_id = OBJECT_ID(N'[dbo].[delivery]'))
ALTER TABLE [dbo].[delivery] DROP CONSTRAINT [fk_SDelivery_StockInfo]
GO

ALTER TABLE [dbo].[StockInfo] DROP CONSTRAINT [PK_StockInfo]
go

ALTER TABLE [dbo].[StockInfo] ADD  CONSTRAINT [PK_StockInfo] PRIMARY KEY CLUSTERED 
(
	[itemno] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'ix_StockInfo_Item_ID')
DROP INDEX [ix_StockInfo_Item_ID] ON [dbo].[StockInfo] WITH ( ONLINE = OFF )
GO





