-- transaction: false

if (Select OBJECTPROPERTY(OBJECT_ID('StockInfo'), 'TableFullTextChangeTrackingOn')) = 1
BEGIN
	DROP FULLTEXT INDEX ON [dbo].[StockInfo]
	DROP FULLTEXT CATALOG [StockCatalog]
END
GO


alter table stockinfo
add Idcol int null
GO

update stockinfo
set Idcol = id
GO

IF EXISTS (SELECT * 
           FROM sys.foreign_keys 
           WHERE object_id = OBJECT_ID(N'[dbo].[fk_StockPrice_StockInfo]') 
             AND parent_object_id = OBJECT_ID(N'[dbo].[StockPrice]'))
BEGIN
ALTER TABLE [dbo].[StockPrice] DROP CONSTRAINT [fk_StockPrice_StockInfo]    
END
GO

IF EXISTS (SELECT * 
           FROM sys.foreign_keys 
           WHERE object_id = OBJECT_ID(N'[dbo].[fk_StockQuantity_StockInfo]') 
             AND parent_object_id = OBJECT_ID(N'[dbo].[StockQuantity]'))
BEGIN
ALTER TABLE [dbo].[StockQuantity] DROP CONSTRAINT [fk_StockQuantity_StockInfo]
END
GO

ALTER TABLE [dbo].[lineitem] DROP CONSTRAINT [fk_LineItem_ItemId]
GO

ALTER TABLE [dbo].[LineitemAudit] DROP CONSTRAINT [fk_LineItemAudit_ItemId]
GO

ALTER TABLE [dbo].[delivery] DROP CONSTRAINT [fk_Delivery_ItemId]
GO

DROP INDEX [ix_StockInfo_IUPC_ID] ON [dbo].[StockInfo]
GO

DROP INDEX [ix_StockInfo_Category] ON [dbo].[StockInfo]
GO

DROP INDEX [ix_StockInfo_ItemType] ON [dbo].[StockInfo]
GO

ALTER TABLE [dbo].[StockInfo] DROP CONSTRAINT [PK_StockInfo]
GO



ALTER TABLE [dbo].[StockInfo] 
DROP column id
GO

sp_rename 'dbo.StockInfo.idcol', 'Id', 'COLUMN'
GO

alter table stockinfo
alter column Id int not null
GO

ALTER TABLE [dbo].[StockInfo] ADD  CONSTRAINT [PK_StockInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO


CREATE NONCLUSTERED INDEX [ix_StockInfo_Category] ON [dbo].[StockInfo]
(
	[category] ASC
)
INCLUDE ( 	[ID],
	[VendorWarranty]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX [ix_StockInfo_ItemType] ON [dbo].[StockInfo]
(
	[itemtype] ASC
)
INCLUDE ( 	[category],
	[ID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


CREATE NONCLUSTERED INDEX [ix_StockInfo_IUPC_ID] ON [dbo].[StockInfo]
(
	[IUPC] ASC,
	[ID] ASC
)WITH (PAD_INDEX = ON, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

ALTER TABLE [dbo].[lineitem]  WITH NOCHECK ADD  CONSTRAINT [fk_LineItem_ItemId] FOREIGN KEY([ItemID])
REFERENCES [dbo].[StockInfo] ([ID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[lineitem] CHECK CONSTRAINT [fk_LineItem_ItemId]
GO

ALTER TABLE [dbo].[LineitemAudit]  WITH NOCHECK ADD  CONSTRAINT [fk_LineItemAudit_ItemId] FOREIGN KEY([ItemID])
REFERENCES [dbo].[StockInfo] ([ID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[LineitemAudit] CHECK CONSTRAINT [fk_LineItemAudit_ItemId]
GO

ALTER TABLE [dbo].[delivery]  WITH NOCHECK ADD  CONSTRAINT [fk_Delivery_ItemId] FOREIGN KEY([ItemID])
REFERENCES [dbo].[StockInfo] ([ID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[delivery] CHECK CONSTRAINT [fk_Delivery_ItemId]
GO

ALTER TABLE [dbo].[StockPrice]  WITH NOCHECK ADD  CONSTRAINT [fk_StockPrice_StockInfo] FOREIGN KEY([ID])
REFERENCES [dbo].[StockInfo] ([ID])
GO

ALTER TABLE [dbo].[StockPrice] CHECK CONSTRAINT [fk_StockPrice_StockInfo]
GO

ALTER TABLE [dbo].[StockQuantity]  WITH NOCHECK ADD  CONSTRAINT [fk_StockQuantity_StockInfo] FOREIGN KEY([ID])
REFERENCES [dbo].[StockInfo] ([ID])
GO

ALTER TABLE [dbo].[StockQuantity] CHECK CONSTRAINT [fk_StockQuantity_StockInfo]
GO


if (SELECT FULLTEXTSERVICEPROPERTY('IsFullTextInstalled')) = 1
BEGIN
	CREATE FULLTEXT CATALOG [StockCatalog]
	WITH ACCENT_SENSITIVITY = OFF

	CREATE FULLTEXT INDEX ON StockInfo(itemno, itemdescr1, itemdescr2, ItemPOSDescr,Brand,VendorLongStyle,VendorEAN)
	KEY INDEX PK_StockInfo 
	ON StockCatalog
END
GO


