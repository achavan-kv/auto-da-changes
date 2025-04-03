-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  not EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'ix_StockInfo_ItemType')
CREATE NONCLUSTERED INDEX [ix_StockInfo_ItemType] ON [dbo].[StockInfo] ([itemtype])
INCLUDE ([category],[ID])