-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [dbo].[StockInfo]
ALTER COLUMN itemdescr2 VARCHAR(45) NOT NULL
GO

ALTER TABLE [dbo].[StockInfo]
ALTER COLUMN SKU VARCHAR(18)
GO

ALTER TABLE [dbo].[StockInfoAudit]
ALTER COLUMN SKU VARCHAR(18)
GO

UPDATE [dbo].[StockInfo] 
SET SKU = itemno
GO

UPDATE [dbo].[StockInfoAudit]
SET SKU = itemno
GO

ALTER TABLE [dbo].[StockInfo]
ALTER COLUMN SKU VARCHAR(18) NOT NULL
GO

ALTER TABLE [dbo].[StockInfoAudit]
ALTER COLUMN SKU VARCHAR(18) NOT NULL
GO