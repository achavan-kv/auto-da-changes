-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


disable trigger trig_lineiteminsert on dbo.lineitem
go
disable trigger trig_lineiteminsertupdate on dbo.lineitem
go
disable trigger trig_lineitemupdate on dbo.lineitem
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'LineItem')
BEGIN
  ALTER TABLE LineItem ADD ItemID INT not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ParentItemID'
               AND OBJECT_NAME(id) = 'LineItem')
BEGIN
  ALTER TABLE LineItem ADD ParentItemID INT not null default 0
END
go

UPDATE lineitem 
	set ItemId=ISNULL(s.ID,0)
from lineitem l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE lineitem 
	set ParentItemID=ISNULL(s.ID,0) 
from lineitem l INNER JOIN stockinfo s on l.ParentItemNo=s.itemno
where ParentItemNo !='' and ISNULL(ParentItemID,0)=0
go

--IF EXISTS (SELECT * FROM syscolumns
--			   WHERE name = 'ItemID'
--               AND OBJECT_NAME(id) = 'LineItem')
--BEGIN
--  ALTER TABLE LineItem alter column ItemID INT not null
--  ALTER TABLE LineItem ADD  DEFAULT (0) FOR [ItemID]  
--END

--IF EXISTS (SELECT * FROM syscolumns
--			   WHERE name = 'ParentItemID'
--               AND OBJECT_NAME(id) = 'LineItem')
--BEGIN
--  ALTER TABLE LineItem ADD ParentItemID INT not null  
--  ALTER TABLE LineItem ADD  DEFAULT (0) FOR [ParentItemID]
--END
--go

enable trigger trig_lineiteminsert on dbo.lineitem
go
enable trigger trig_lineiteminsertupdate on dbo.lineitem
go
enable trigger trig_lineitemupdate on dbo.lineitem
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'LineItemAudit')
BEGIN
  ALTER TABLE LineItemAudit ADD ItemID INT not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ParentItemID'
               AND OBJECT_NAME(id) = 'LineItemAudit')
BEGIN
  ALTER TABLE LineItemAudit ADD ParentItemID INT not null default 0
END
go



UPDATE LineItemAudit 
	set ItemId=ISNULL(s.ID,0) 
from LineItemAudit l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE LineItemAudit 
	set ParentItemID=ISNULL(s.ID,0) 
from LineItemAudit l INNER JOIN stockinfo s on l.ParentItemNo=s.itemno
where ParentItemNo !='' and ISNULL(ParentItemID,0)=0
go



IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'lineitem_amend')
BEGIN
  ALTER TABLE lineitem_amend ADD ItemID INT not null default 0
END

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ParentItemID'
               AND OBJECT_NAME(id) = 'lineitem_amend')
BEGIN
  ALTER TABLE lineitem_amend ADD ParentItemID INT not null default 0
END
go

UPDATE lineitem_amend 
	set ItemId=ISNULL(s.ID,0) 
from lineitem_amend l INNER JOIN stockinfo s on l.itemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE lineitem_amend 
	set ParentItemID=ISNULL(s.ID,0) 
from lineitem_amend l INNER JOIN stockinfo s on l.ParentItemNo=s.itemno
where ParentItemNo !='' and ISNULL(ParentItemID,0)=0
go