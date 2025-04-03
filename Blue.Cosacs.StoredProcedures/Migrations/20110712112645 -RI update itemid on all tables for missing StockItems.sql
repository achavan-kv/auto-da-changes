-- transaction: false
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE lineitem
	set itemid=s.id
from lineitem l INNER JOIN stockInfo s on l.itemno=s.itemno
where l.itemid=0

go

UPDATE Delivery
	set itemid=s.id
from Delivery d INNER JOIN stockInfo s on d.itemno=s.itemno
where d.itemid=0

go

UPDATE Exchange
	set itemid=s.id
from Exchange e INNER JOIN stockInfo s on e.itemno=s.itemno
where e.itemid=0

go

UPDATE lineitemAudit
	set itemid=s.id
from lineitemAudit l INNER JOIN stockInfo s on l.itemno=s.itemno
where l.itemid=0

go

UPDATE lineitem_amend
	set itemid=s.id
from lineitem_amend l INNER JOIN stockInfo s on l.itemno=s.itemno
where l.itemid=0

go

Alter TABLE lineitembfCollection alter column itemno VARCHAR(18) not null
go

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_LineItem_ItemId]') AND parent_object_id = OBJECT_ID(N'[dbo].[LineItem]'))
ALTER TABLE [dbo].[LineItem] DROP CONSTRAINT [fk_LineItem_ItemId]
GO

ALTER TABLE [dbo].[LineItem]  WITH NOCHECK ADD  CONSTRAINT [fk_LineItem_ItemId] FOREIGN KEY([itemId])
REFERENCES [dbo].[StockInfo] ([ID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[LineItem] CHECK CONSTRAINT [fk_LineItem_ItemId]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_Delivery_ItemId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Delivery]'))
ALTER TABLE [dbo].[Delivery] DROP CONSTRAINT [fk_Delivery_ItemId]
GO

ALTER TABLE [dbo].[Delivery]  WITH NOCHECK ADD  CONSTRAINT [fk_Delivery_ItemId] FOREIGN KEY([itemId])
REFERENCES [dbo].[StockInfo] ([ID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[Delivery] CHECK CONSTRAINT [fk_Delivery_ItemId]
GO

IF  EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[fk_LineItemAudit_ItemId]') AND parent_object_id = OBJECT_ID(N'[dbo].[LineItemAudit]'))
ALTER TABLE [dbo].[LineItemAudit] DROP CONSTRAINT [fk_LineItemAudit_ItemId]
GO

ALTER TABLE [dbo].[LineItemAudit]  WITH NOCHECK ADD  CONSTRAINT [fk_LineItemAudit_ItemId] FOREIGN KEY([itemId])
REFERENCES [dbo].[StockInfo] ([ID])
ON UPDATE CASCADE
GO

ALTER TABLE [dbo].[LineItemAudit] CHECK CONSTRAINT [fk_LineItemAudit_ItemId]
GO

