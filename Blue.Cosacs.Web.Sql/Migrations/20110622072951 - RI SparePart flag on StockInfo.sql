-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SparePart'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD SparePart bit not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemPOSDescr'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD ItemPOSDescr VARCHAR(25)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SparePart'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD SparePart bit not null default 0
END

go

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockInfo_InsertUpdate]'))
Disable TRIGGER [dbo].[Trig_StockInfo_InsertUpdate] on dbo.StockInfo
GO

UPDATE StockInfo
	set SparePart=1
Where category=20 and SparePart=0

UPDATE StockInfoAudit
	set SparePart=1
Where category=20 and SparePart=0

IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[Trig_StockInfo_InsertUpdate]'))
Enable TRIGGER [dbo].[Trig_StockInfo_InsertUpdate] on dbo.StockInfo
GO
