-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

--alter table cosacs_archive.dbo.Schedule_archive drop column ItemID
--alter table cosacs_archive.dbo.Schedule_archive drop column RetItemID
--alter table cosacs_archive.dbo.Lineitem_archive drop column ItemID
--alter table cosacs_archive.dbo.Lineitem_archive drop column ParentItemID
--alter table cosacs_archive.dbo.Delivery_archive drop column ItemID
--alter table cosacs_archive.dbo.Delivery_archive drop column ParentItemID
--alter table cosacs_archive.dbo.Delivery_archive drop column RetItemID
IF NOT EXISTS(select * from cosacs_archive.dbo.sysobjects
				where name = 'stockinfotemp')
BEGIN
	select * into cosacs_archive.dbo.stockinfotemp from stockinfo

	--IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='UpgradeArchiveDB')
	--	DROP PROCEDURE UpgradeArchiveDB
	--GO

	--CREATE PROCEDURE UpgradeArchiveDB
	 
	-- as
	 
	--USE[cosacs_archive]

	--IF NOT EXISTS (SELECT * FROM cosacs_archive.dbo.syscolumns
	--			   WHERE name = 'ItemID'
	--               AND OBJECT_NAME(id) = 'Schedule_archive')
	--BEGIN
	  ALTER TABLE cosacs_archive.dbo.Schedule_archive ADD ItemID INT not null default 0
	--END
	--go

	--USE[cosacs_archive]

	UPDATE cosacs_archive.dbo.Schedule_archive
		set ItemId=ISNULL(s.ID,0) 
	from cosacs_archive.dbo.Schedule_archive l INNER JOIN cosacs_archive.dbo.stockinfotemp s on l.itemno=s.itemno
	where ISNULL(ItemId,0)=0
	--go

	--USE[cosacs_archive]

	--IF NOT EXISTS (SELECT * FROM cosacs_archive.dbo.syscolumns
	--			   WHERE name = 'RetItemID'
	--               AND OBJECT_NAME(id) = 'Schedule_archive')
	--BEGIN
	  ALTER TABLE cosacs_archive.dbo.Schedule_archive ADD RetItemID INT not null default 0
	--END
	--go

	--USE[cosacs_archive]

	UPDATE cosacs_archive.dbo.Schedule_archive
		set RetItemId=ISNULL(s.ID,0) 
	from cosacs_archive.dbo.Schedule_archive l INNER JOIN cosacs_archive.dbo.stockinfotemp s on l.retitemno=s.itemno
	where ISNULL(RetItemId,0)=0
	--go

	--USE[cosacs_archive]

	--IF NOT EXISTS (SELECT * FROM cosacs_archive.dbo.syscolumns
	--			   WHERE name = 'ItemID'
	--               AND OBJECT_NAME(id) = 'Lineitem_archive')
	--BEGIN
	  ALTER TABLE cosacs_archive.dbo.Lineitem_archive ADD ItemID INT not null default 0
	--END
	--go

	--USE[cosacs_archive]

	UPDATE cosacs_archive.dbo.Lineitem_archive
		set ItemId=ISNULL(s.ID,0) 
	from cosacs_archive.dbo.Lineitem_archive l INNER JOIN cosacs_archive.dbo.stockinfotemp s on l.itemno=s.itemno
	where ISNULL(ItemId,0)=0
	--go

	--USE[cosacs_archive]

	--IF NOT EXISTS (SELECT * FROM syscolumns
	--			   WHERE name = 'ParentItemID'
	--               AND OBJECT_NAME(id) = 'Lineitem_archive')
	--BEGIN
	  ALTER TABLE cosacs_archive.dbo.Lineitem_archive ADD ParentItemID INT not null default 0
	--END
	--go

	--USE[cosacs_archive]

	UPDATE cosacs_archive.dbo.Lineitem_archive
		set ParentItemID=ISNULL(s.ID,0) 
	from cosacs_archive.dbo.Lineitem_archive l INNER JOIN cosacs_archive.dbo.stockinfotemp s on l.parentitemno=s.itemno
	where ISNULL(ItemId,0)=0
	--go


	--USE[cosacs_archive]

	--IF NOT EXISTS (SELECT * FROM syscolumns
	--			   WHERE name = 'ItemID'
	--               AND OBJECT_NAME(id) = 'Delivery_archive')
	--BEGIN
	  ALTER TABLE cosacs_archive.dbo.Delivery_archive ADD ItemID INT not null default 0
	--END
	--go

	--USE[cosacs_archive]

	UPDATE cosacs_archive.dbo.Delivery_archive
		set ItemId=ISNULL(s.ID,0) 
	from cosacs_archive.dbo.Delivery_archive l INNER JOIN cosacs_archive.dbo.stockinfotemp s on l.itemno=s.itemno
	where ISNULL(ItemId,0)=0
	--go

	--USE[cosacs_archive]

	--IF NOT EXISTS (SELECT * FROM syscolumns
	--			   WHERE name = 'ParentItemID'
	--               AND OBJECT_NAME(id) = 'Delivery_archive')
	--BEGIN
	  ALTER TABLE cosacs_archive.dbo.Delivery_archive ADD ParentItemID INT not null default 0
	--END
	--go

	--USE[cosacs_archive]

	--UPDATE Delivery_archive
	--	set ParentItemID=ISNULL(s.ID,0) 
	--from Delivery_archive l INNER JOIN stockinfotemp s on l.parentitemno=s.itemno
	--where ISNULL(ParentItemId,0)=0
	--go

	--USE[cosacs_archive]

	--IF NOT EXISTS (SELECT * FROM syscolumns
	--			   WHERE name = 'RetItemID'
	--               AND OBJECT_NAME(id) = 'Delivery_archive')
	--BEGIN
	  ALTER TABLE cosacs_archive.dbo.Delivery_archive ADD RetItemID INT not null default 0
	--END
	--go

	--USE[cosacs_archive]

	UPDATE cosacs_archive.dbo.Delivery_archive
		set RetItemID=ISNULL(s.ID,0) 
	from cosacs_archive.dbo.Delivery_archive l INNER JOIN cosacs_archive.dbo.stockinfotemp s on l.retitemno=s.itemno
	where ISNULL(RetItemId,0)=0
	--go

--USE[cosacs_archive]
END
go

--drop table cosacs_archive.dbo.stockinfotemp

--declare @sql nvarchar(max)

--set @sql= 'Use [cosacs_archive] exec ' + DB_Name() + '.dbo.UpgradeArchiveDB'

--select @sql
--execute sp_executesql @sql

--drop table cosacs_archive.dbo.stockinfotemp

--go


--select  DB_NAME() --as dbname into #dbname


--declare @dbname nvarchar(max)
--select @dbname= dbname from #dbname

--set @dbname= 'Use [' + @dbname + ']'

--select @dbname 
--execute sp_executesql @dbname

--select * from stockinfo