-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
ALTER TABLE [dbo].[StockInfo] ALTER COLUMN WarrantyType char(1) NULL

UPDATE [dbo].[StockInfo]
SET WarrantyType = NULL
WHERE itemtype = 'S'

UPDATE [dbo].[StockInfo]
SET WarrantyType = NULL
WHERE itemtype  in('N',' ') and category not in (select code from code where category='WAR')

