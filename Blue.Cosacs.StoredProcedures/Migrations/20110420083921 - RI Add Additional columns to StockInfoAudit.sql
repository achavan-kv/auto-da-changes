-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ID'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD ID INT not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SKU'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD SKU VARCHAR(8)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'IUPC'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD IUPC VARCHAR(18)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Class'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD Class CHAR(3)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SubClass'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD SubClass CHAR(5)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ColourName'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD ColourName varCHAR(12)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ColourCode'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD ColourCode varCHAR(3)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'QtyBoxes'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD QtyBoxes smallint
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'WarrantyLength'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD WarrantyLength smallint
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorWarranty'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD VendorWarranty smallint 
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Brand'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD Brand VarCHAR(25)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorStyle'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD VendorStyle VarCHAR(12)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorLongStyle'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD VendorLongStyle VarCHAR(25)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorEAN'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD VendorEAN VarCHAR(18)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorLongStyle'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD VendorLongStyle VarCHAR(25)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepossessedItem'
               AND OBJECT_NAME(id) = 'StockInfoAudit')
BEGIN
  ALTER TABLE StockInfoAudit ADD RepossessedItem BIT not null default(0)
END
go



UPDATE StockInfoAudit
	set ID=s.ID 
From StockInfoAudit a ,StockInfo s 
--where s.ID=(select MIN(id) from StockInfo where a.itemno=s.itemno group by a.itemno)
where a.itemno=s.itemno

go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfoAudit]') AND name = N'PK_StockInfoAudit')
ALTER TABLE [dbo].[StockInfoAudit] DROP CONSTRAINT [PK_StockInfoAudit]
GO

ALTER TABLE [dbo].[StockInfoAudit] ADD  CONSTRAINT [PK_StockInfoAudit] PRIMARY KEY CLUSTERED 
(
	[Itemno] ASC,
	[ID] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
