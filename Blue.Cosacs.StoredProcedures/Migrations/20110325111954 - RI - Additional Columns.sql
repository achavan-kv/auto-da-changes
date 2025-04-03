-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ID'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD ID INT Identity
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SKU'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD SKU VARCHAR(8)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'IUPC'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD IUPC VARCHAR(18)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Class'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD Class CHAR(3)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'SubClass'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD SubClass CHAR(5)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ColourName'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD ColourName varCHAR(12)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ColourCode'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD ColourCode varCHAR(3)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'QtyBoxes'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD QtyBoxes smallint
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'WarrantyLength'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD WarrantyLength smallint
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorWarranty'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD VendorWarranty smallint 
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'Brand'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD Brand VarCHAR(25)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorStyle'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD VendorStyle VarCHAR(12)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorLongStyle'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD VendorLongStyle VarCHAR(25)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorEAN'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD VendorEAN VarCHAR(18)
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'VendorLongStyle'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD VendorLongStyle VarCHAR(25)
END


IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ID'
               AND OBJECT_NAME(id) = 'StockQuantity')
BEGIN
  ALTER TABLE StockQuantity ADD ID INT 
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ID'
               AND OBJECT_NAME(id) = 'StockPrice')
BEGIN
  ALTER TABLE StockPrice ADD ID INT 
END

go

ALTER TABLE [dbo].[StockInfo] DROP CONSTRAINT [PK_StockInfo]
go

ALTER TABLE [dbo].[StockInfo] ADD  CONSTRAINT [PK_StockInfo] PRIMARY KEY CLUSTERED 
(
	[ID]
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [ix_StockInfo_Item_ID] ON [dbo].[StockInfo] 
(
	[Itemno] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [ix_StockInfo_IUPC_ID] ON [dbo].[StockInfo] 
(
	[IUPC] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

ALTER TABLE [dbo].[StockQuantity]  WITH NOCHECK ADD  CONSTRAINT [fk_StockQuantity_StockInfo] FOREIGN KEY([ID])
REFERENCES [dbo].[StockInfo] ([ID])

ALTER TABLE [dbo].[StockPrice]  WITH NOCHECK ADD  CONSTRAINT [fk_StockPrice_StockInfo] FOREIGN KEY([ID])
REFERENCES [dbo].[StockInfo] ([ID])