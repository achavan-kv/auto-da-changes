-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Facttrans')
BEGIN
  ALTER TABLE Facttrans ADD ItemID INT not null default 0
END
go

--Update the ItemID column for existing Facttrans records

update Facttrans set ItemID = isnull((select si.ID from StockInfo si inner join StockQuantity sq on si.ID = sq.ID
								where si.itemno = ft.itemno
								and sq.stocklocn = ft.stocklocn
								and si.repossesseditem = 0),0)
from Facttrans ft



IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'lastfactexport')
BEGIN
  ALTER TABLE lastfactexport ADD ItemID INT not null default 0
END
go


IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ParentItemID'
               AND OBJECT_NAME(id) = 'lastfactexport')
BEGIN
  ALTER TABLE lastfactexport ADD ParentItemID INT not null default 0
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'OriginalItemItemID'
               AND OBJECT_NAME(id) = 'lastfactexport')
BEGIN
  ALTER TABLE lastfactexport ADD OriginalItemItemID INT not null default 0
END
go


IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'WarrantyItemID'
               AND OBJECT_NAME(id) = 'lastfactexport')
BEGIN
  ALTER TABLE lastfactexport ADD WarrantyItemID INT not null default 0
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RetItemID'
               AND OBJECT_NAME(id) = 'lastfactexport')
BEGIN
  ALTER TABLE lastfactexport ADD RetItemID INT not null default 0
END
go

--Update the new ID columns for existing lastfactexport records

update lastfactexport set ItemID = isnull((select si.ID from StockInfo si inner join StockQuantity sq on si.ID = sq.ID
								where si.itemno = lf.itemno
								and sq.stocklocn = lf.stocklocn
								and si.repossesseditem = 0),0)
from lastfactexport lf


update lastfactexport set ParentItemID = isnull((select si.ID from StockInfo si inner join StockQuantity sq on si.ID = sq.ID
								where si.itemno = lf.parentitemno
								and sq.stocklocn = lf.parentlocation
								and si.repossesseditem = 0),0)
from lastfactexport lf

update lastfactexport set OriginalItemItemID = isnull((select si.ID from StockInfo si inner join StockQuantity sq on si.ID = sq.ID
								where si.itemno = lf.originalitem
								and sq.stocklocn = lf.stocklocn
								and si.repossesseditem = 0),0)
from lastfactexport lf

update lastfactexport set WarrantyItemID = isnull((select si.ID from StockInfo si inner join StockQuantity sq on si.ID = sq.ID
								where si.itemno = lf.warrantyitemno
								and sq.stocklocn = lf.stocklocn
								and si.repossesseditem = 0),0)
from lastfactexport lf

update lastfactexport set RetItemID = isnull((select si.ID from StockInfo si inner join StockQuantity sq on si.ID = sq.ID
								where si.itemno = lf.retitemno
								and sq.stocklocn = lf.retstocklocn
								and si.repossesseditem = 0),0)
from lastfactexport lf

