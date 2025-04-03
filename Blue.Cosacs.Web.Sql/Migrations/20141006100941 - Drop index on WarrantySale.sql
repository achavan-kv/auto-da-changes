-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF EXISTS(select 'a' from sys.indexes where name = 'idx_warrantySale_CustId_ItemId_StockLocation')
    DROP INDEX [idx_warrantySale_CustId_ItemId_StockLocation] ON [Warranty].[WarrantySale]
GO

update statistics warranty.warrantySale
GO