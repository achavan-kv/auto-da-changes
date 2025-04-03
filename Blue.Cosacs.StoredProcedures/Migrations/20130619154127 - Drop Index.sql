-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS (select * from sys.indexes where name ='IX_StockInfo_ItemNo')
	DROP INDEX [IX_StockInfo_ItemNo] ON [dbo].[StockInfo]

go


