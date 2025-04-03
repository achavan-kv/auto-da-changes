-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- timeout: 9900
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'ScheduleAudit')
BEGIN
  ALTER TABLE ScheduleAudit ADD ItemID INT not null default 0
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'ScheduleRemoval')
BEGIN
  ALTER TABLE ScheduleRemoval ADD ItemID INT not null default 0
END
go

UPDATE ScheduleAudit 
	set ItemId=ISNULL(s.ID,0) 
from ScheduleAudit l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE ScheduleRemoval 
	set ItemId=ISNULL(s.ID,0) 
from ScheduleRemoval l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'SalesCommissionExtraSpiffs')
BEGIN
  ALTER TABLE SalesCommissionExtraSpiffs ADD ItemID INT not null default 0
END
go

UPDATE SalesCommissionExtraSpiffs 
	set ItemId=ISNULL(s.ID,0) 
from SalesCommissionExtraSpiffs l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'promoprice')
BEGIN
  ALTER TABLE promoprice ADD ItemID INT not null default 0
END
go

UPDATE promoprice 
	set ItemId=ISNULL(s.ID,0) 
from promoprice l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[promoprice]') AND name = N'ixcl_promoprice')
DROP INDEX [ixcl_promoprice] ON [dbo].[promoprice] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[promoprice]') AND name = N'pk_promoprice')
DROP INDEX pk_promoprice ON [dbo].[promoprice] WITH ( ONLINE = OFF )
GO

CREATE CLUSTERED INDEX [ixcl_promoprice] ON [dbo].[promoprice] 
(
	[ItemID] ASC,
	[stocklocn] ASC,
	[hporcash] ASC,
	[fromdate] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
truncate table PurchaseOrderOutstanding
GO 
IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'PurchaseOrderOutstanding')
BEGIN
  ALTER TABLE PurchaseOrderOutstanding ADD ItemID INT not null default 0
END
go

UPDATE PurchaseOrderOutstanding 
	set ItemId=ISNULL(s.ID,0) 
from PurchaseOrderOutstanding l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[PurchaseOrderOutstanding]') AND name = N'PK_PurchaseOrderOutstanding')
ALTER TABLE [dbo].[PurchaseOrderOutstanding] DROP CONSTRAINT [PK_PurchaseOrderOutstanding]
GO

ALTER TABLE [dbo].[PurchaseOrderOutstanding] ADD  CONSTRAINT [PK_PurchaseOrderOutstanding] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[stocklocn] ASC,
	[purchaseordernumber] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'sundchgtyp')
BEGIN
  ALTER TABLE sundchgtyp ADD ItemID INT not null default 0
END
go

UPDATE sundchgtyp 
	set ItemId=ISNULL(s.ID,0) 
from sundchgtyp l INNER JOIN stockinfo s on l.chargecode=s.itemno
where ISNULL(ItemId,0)=0
go

--IF NOT EXISTS (SELECT * FROM syscolumns
--			   WHERE name = 'ItemID'
--               AND OBJECT_NAME(id) = 'itemdetails')
--BEGIN
--  ALTER TABLE itemdetails ADD ItemID INT not null default 0
--END
--go

--UPDATE itemdetails 
--	set ItemId=ISNULL(s.ID,0) 
--from itemdetails l INNER JOIN stockinfo s on l.Itemno=s.itemno
--where ISNULL(ItemId,0)=0
--go

--IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[itemdetails]') AND name = N'pk_itemdetails')
--ALTER TABLE [dbo].[itemdetails] DROP CONSTRAINT [pk_itemdetails]
--GO

--ALTER TABLE [dbo].[itemdetails] ADD  CONSTRAINT [pk_itemdetails] PRIMARY KEY CLUSTERED 
--(
--	[acctno] ASC,
--	[agrmtno] ASC,
--	[ItemID] ASC,
--	[stocklocn] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
--GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Delivery_Error_log')
BEGIN
  ALTER TABLE Delivery_Error_log ADD ItemID INT not null default 0
  ALTER TABLE Delivery_Error_log ADD ParentItemID INT not null default 0
END
go

UPDATE Delivery_Error_log 
	set ItemId=ISNULL(s.ID,0) 
from Delivery_Error_log l INNER JOIN stockinfo s on l.Itemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE Delivery_Error_log 
	set ParentItemId=ISNULL(s.ID,0) 
from Delivery_Error_log l INNER JOIN stockinfo s on l.Itemno=s.itemno
where ISNULL(ParentItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Delivery_Error_log]') AND name = N'pk_delivery_error_log')
ALTER TABLE [dbo].[Delivery_Error_log] DROP CONSTRAINT [pk_delivery_error_log]
GO

ALTER TABLE [dbo].[Delivery_Error_log] ADD  CONSTRAINT [pk_delivery_error_log] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[ItemID] ASC,
	[stocklocn] ASC,
	[buffno] ASC,
	[contractno] ASC,
	[ParentItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'DiscountsAuthorised')
BEGIN
  ALTER TABLE DiscountsAuthorised ADD ItemID INT not null default 0
  ALTER TABLE DiscountsAuthorised ADD ParentItemID INT not null default 0
END
go

UPDATE DiscountsAuthorised 
	set ItemId=ISNULL(s.ID,0) 
from DiscountsAuthorised l INNER JOIN stockinfo s on l.DiscountItemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE DiscountsAuthorised 
	set ParentItemId=ISNULL(s.ID,0) 
from DiscountsAuthorised l INNER JOIN stockinfo s on l.ParentItemno=s.itemno
where ISNULL(ParentItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'lineitembfCollection')
BEGIN
  ALTER TABLE lineitembfCollection ADD ItemID INT not null default 0
END
go

UPDATE lineitembfCollection 
	set ItemId=ISNULL(s.ID,0) 
from lineitembfCollection l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

delete l
from lineitemosdelnotes l
where not exists(select * from stockinfo s where l.itemno=s.itemno )
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'lineitemosdelnotes')
BEGIN
  ALTER TABLE lineitemosdelnotes ADD ItemID INT not null default 0
END
go

UPDATE lineitemosdelnotes 
	set ItemId=ISNULL(s.ID,0) 
from lineitemosdelnotes l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitemosdelnotes]') AND name = N'pk_lineitemosdelnotes')
ALTER TABLE [dbo].[lineitemosdelnotes] DROP CONSTRAINT [pk_lineitemosdelnotes]
GO

ALTER TABLE [dbo].[lineitemosdelnotes] ADD  CONSTRAINT [pk_lineitemosdelnotes] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[itemID] ASC,
	[stocklocn] ASC,
	[contractno] ASC,
	[delnotebranch] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'CashandGoTopSellStock')
BEGIN
  ALTER TABLE CashandGoTopSellStock ADD ItemID INT not null default 0
END
go

UPDATE CashandGoTopSellStock 
	set ItemId=ISNULL(s.ID,0) 
from CashandGoTopSellStock l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CashandGoTopSellStock]') AND name = N'pk_CashandGoTopSellStock')
ALTER TABLE [dbo].[CashandGoTopSellStock] DROP CONSTRAINT [pk_CashandGoTopSellStock]
GO

ALTER TABLE [dbo].[CashandGoTopSellStock] ADD  CONSTRAINT [pk_CashandGoTopSellStock] PRIMARY KEY CLUSTERED 
(
	[branchno] ASC,
	[itemID] ASC,
	[daterefresh] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'CollectionNoteChange')
BEGIN
  ALTER TABLE CollectionNoteChange ADD ItemID INT not null default 0
END
go

UPDATE CollectionNoteChange 
	set ItemId=ISNULL(s.ID,0) 
from CollectionNoteChange l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CollectionNoteChange]') AND name = N'pk_CollectionNoteChange')
ALTER TABLE [dbo].[CollectionNoteChange] DROP CONSTRAINT [pk_CollectionNoteChange]
GO

ALTER TABLE [dbo].[CollectionNoteChange] ADD  CONSTRAINT [pk_CollectionNoteChange] PRIMARY KEY CLUSTERED 
(
	[AcctNo] ASC,
	[AgrmtNo] ASC,
	[ItemID] ASC,
	[OldRetStockLocn] ASC,
	[DateChanged] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Exchange')
BEGIN
  ALTER TABLE Exchange ADD ItemID INT not null default 0
  ALTER TABLE Exchange ADD WarrantyID INT not null default 0
END
go

UPDATE Exchange 
	set ItemId=ISNULL(s.ID,0) 
from Exchange l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE Exchange 
	set WarrantyID=ISNULL(s.ID,0) 
from Exchange l INNER JOIN stockinfo s on l.WarrantyNo=s.itemno
where ISNULL(WarrantyID,0)=0 and WarrantyNo!=''
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[Exchange]') AND name = N'pk_Exchange')
ALTER TABLE [dbo].[Exchange] DROP CONSTRAINT [pk_Exchange]
GO

ALTER TABLE [dbo].[Exchange] ADD  CONSTRAINT [pk_Exchange] PRIMARY KEY CLUSTERED 
(
	[BuffNo] ASC,
	[AcctNo] ASC,
	[AgrmtNo] ASC,
	[ItemID] ASC,
	[StockLocn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'lineitemStockNotprintedDN')
BEGIN
  ALTER TABLE lineitemStockNotprintedDN ADD ItemID INT not null default 0
END
go

UPDATE lineitemStockNotprintedDN 
	set ItemId=ISNULL(s.ID,0) 
from lineitemStockNotprintedDN l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitemStockNotprintedDN]') AND name = N'pk_lineitemNotprintedDN')
ALTER TABLE [dbo].[lineitemStockNotprintedDN] DROP CONSTRAINT [pk_lineitemNotprintedDN]
GO

ALTER TABLE [dbo].[lineitemStockNotprintedDN] ADD  CONSTRAINT [pk_lineitemNotprintedDN] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[ItemID] ASC,
	[stocklocn] ASC,
	[contractno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

--IF NOT EXISTS (SELECT * FROM syscolumns
--			   WHERE name = 'ItemID'
--               AND OBJECT_NAME(id) = 'lineitemwarrantieserased')
--BEGIN
--  ALTER TABLE lineitemwarrantieserased ADD ItemID INT not null default 0
--  ALTER TABLE lineitemwarrantieserased ADD WarntyID INT not null default 0
--  ALTER TABLE lineitemwarrantieserased ADD KitID INT not null default 0
--  ALTER TABLE lineitemwarrantieserased ADD RetItemID INT not null default 0
--END
--go

--UPDATE lineitemwarrantieserased 
--	set ItemId=ISNULL(s.ID,0) 
--from lineitemwarrantieserased l INNER JOIN stockinfo s on l.itemno=s.itemno
--where ISNULL(ItemId,0)=0
--go

--UPDATE lineitemwarrantieserased 
--	set WarntyID=ISNULL(s.ID,0) 
--from lineitemwarrantieserased l INNER JOIN stockinfo s on l.WarrantyNo=s.itemno
--where ISNULL(WarntyID,0)=0 
--go

--UPDATE lineitemwarrantieserased 
--	set KitID=ISNULL(s.ID,0) 
--from lineitemwarrantieserased l INNER JOIN stockinfo s on l.itemno=s.itemno
--where ISNULL(KitID,0)=0 and Kitno!=''
--go

--UPDATE lineitemwarrantieserased 
--	set RetItemID=ISNULL(s.ID,0) 
--from lineitemwarrantieserased l INNER JOIN stockinfo s on l.RetItemNo=s.itemno
--where ISNULL(RetItemID,0)=0 and ISNULL(RetItemNo,'')!=''
--go

--IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitemwarrantieserased]') AND name = N'pk_lineitemwarrantieserased')
--ALTER TABLE [dbo].[lineitemwarrantieserased] DROP CONSTRAINT [pk_lineitemwarrantieserased]
--GO

--ALTER TABLE [dbo].[lineitemwarrantieserased] ADD  CONSTRAINT [pk_lineitemwarrantieserased] PRIMARY KEY CLUSTERED 
--(
--	[acctno] ASC,
--	[agrmtno] ASC,
--	[ItemID] ASC,
--	[WarntyID] ASC,
--	[stocklocn] ASC,
--	[contractno] ASC
--)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
--GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'order_removed')
BEGIN
  ALTER TABLE order_removed ADD ItemID INT not null default 0
END
go

UPDATE order_removed 
	set ItemId=ISNULL(s.ID,0) 
from order_removed l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[order_removed]') AND name = N'pk_order_removed')
ALTER TABLE [dbo].[order_removed] DROP CONSTRAINT [pk_order_removed]
GO

ALTER TABLE [dbo].[order_removed] ADD  CONSTRAINT [pk_order_removed] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[ItemID] ASC,
	[stocklocn] ASC,
	[buffbranchno] ASC,
	[buffno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'ProductFaults')
BEGIN
  ALTER TABLE ProductFaults ADD ItemID INT not null default 0
  ALTER TABLE ProductFaults ADD RetItemID INT not null default 0
END
go

UPDATE ProductFaults 
	set ItemId=ISNULL(s.ID,0) 
from ProductFaults l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE ProductFaults 
	set RetItemID=ISNULL(s.ID,0) 
from ProductFaults l INNER JOIN stockinfo s on l.ReturnItemNo=s.itemno
where ISNULL(RetItemID,0)=0 and ReturnItemNo!=''
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ProductFaults]') AND name = N'PK_ProductFaults')
ALTER TABLE [dbo].[ProductFaults] DROP CONSTRAINT [PK_ProductFaults]
GO

ALTER TABLE [dbo].[ProductFaults] ADD  CONSTRAINT [PK_ProductFaults] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[itemID] ASC,
	[RetItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 95) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'DeliveryReprint')
BEGIN
  ALTER TABLE DeliveryReprint ADD ItemID INT not null default 0
END
go

UPDATE DeliveryReprint 
	set ItemId=ISNULL(s.ID,0) 
from DeliveryReprint l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[DeliveryReprint]') AND name = N'pk_DeliveryReprint')
ALTER TABLE [dbo].[DeliveryReprint] DROP CONSTRAINT [pk_DeliveryReprint]
GO

ALTER TABLE [dbo].[DeliveryReprint] ADD  CONSTRAINT [pk_DeliveryReprint] PRIMARY KEY CLUSTERED 
(
	[AcctNo] ASC,
	[AgrmtNo] ASC,
	[ItemID] ASC,
	[StockLocn] ASC,
	[DatePrinted] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'StockItemTrans')
BEGIN
  ALTER TABLE StockItemTrans ADD ItemID INT not null default 0
END
go

UPDATE StockItemTrans 
	set ItemId=ISNULL(s.ID,0) 
from StockItemTrans l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockItemTrans]') AND name = N'pk_StockItemTrans')
ALTER TABLE [dbo].[StockItemTrans] DROP CONSTRAINT [pk_StockItemTrans]
GO

ALTER TABLE [dbo].[StockItemTrans] ADD  CONSTRAINT [pk_StockItemTrans] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'CollectionReason')
BEGIN
  ALTER TABLE CollectionReason ADD ItemID INT not null default 0
END
go

UPDATE CollectionReason 
	set ItemId=ISNULL(s.ID,0) 
from CollectionReason l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'complaint')
BEGIN
  ALTER TABLE complaint ADD ItemID INT not null default 0
END
go

UPDATE complaint 
	set ItemId=ISNULL(s.ID,0) 
from complaint l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go


IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'warrantyrenewalpurchase')
BEGIN
  ALTER TABLE warrantyrenewalpurchase ADD ItemID INT not null default 0
END
go

UPDATE warrantyrenewalpurchase 
	set ItemId=ISNULL(s.ID,0) 
from warrantyrenewalpurchase l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Installation')
BEGIN
  ALTER TABLE Installation ADD ItemID INT not null default 0
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemNo'
               AND OBJECT_NAME(id) = 'Installation')
BEGIN
  ALTER TABLE Installation ADD ItemNo VARCHAR(18) not null default ''
END
go

UPDATE Installation 
	set ItemId=ISNULL(s.ID,0) 
from Installation l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'SR_ServiceRequest')
BEGIN
  ALTER TABLE SR_ServiceRequest ADD ItemID INT not null default 0
END
go

UPDATE SR_ServiceRequest 
	set ItemId=ISNULL(s.ID,0) 
from SR_ServiceRequest l INNER JOIN stockinfo s on l.ProductCode=s.itemno
where ISNULL(ItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Summary1AcctStatus')
BEGIN
  ALTER TABLE Summary1AcctStatus ADD ItemID INT not null default 0
END
go

UPDATE Summary1AcctStatus 
	set ItemId=ISNULL(s.ID,0) 
from Summary1AcctStatus l INNER JOIN stockinfo s on l.ItemNo=s.itemno
where ISNULL(ItemId,0)=0
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'ItemLocking')
BEGIN
  ALTER TABLE ItemLocking ADD ItemID INT not null default 0
END
go

UPDATE ItemLocking 
	set ItemId=ISNULL(s.ID,0) 
from ItemLocking l INNER JOIN stockinfo s on l.ItemNo=s.itemno
where ISNULL(ItemId,0)=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ItemLocking]') AND name = N'pk_ItemLocking')
ALTER TABLE [dbo].[ItemLocking] DROP CONSTRAINT [pk_ItemLocking]
GO

ALTER TABLE [dbo].[ItemLocking] ADD  CONSTRAINT [pk_ItemLocking] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[stocklocn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
