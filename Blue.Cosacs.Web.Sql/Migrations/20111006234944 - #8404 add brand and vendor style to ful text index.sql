-- transaction: false
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER FULLTEXT INDEX ON [dbo].[StockInfo] ADD ([Brand])
GO

ALTER FULLTEXT INDEX ON [dbo].[StockInfo] ADD ([VendorLongStyle])
GO

ALTER FULLTEXT INDEX ON [dbo].[StockInfo] START FULL POPULATION
GO