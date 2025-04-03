-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  not EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockPrice]') AND name = N'ix_StockPrice_Refcode')
Begin
CREATE NONCLUSTERED INDEX [ix_StockPrice_Refcode]
ON [dbo].[StockPrice] ([Refcode])
INCLUDE ([ID])
End
GO

IF  not EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'ix_StockInfo_Category')
Begin
CREATE NONCLUSTERED INDEX [ix_StockInfo_Category]
ON [dbo].[StockInfo] ([category])
INCLUDE ([ID],[VendorWarranty])
End
GO



