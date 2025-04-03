-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'WarrantyBand')
BEGIN
  ALTER TABLE WarrantyBand ADD ItemID INT not null default 0
END

go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'warrantycodes')
BEGIN
  ALTER TABLE warrantycodes ADD ItemID INT not null default 0
END

go

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[warrantyband_warrantylength_default]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[warrantyband] DROP CONSTRAINT [warrantyband_warrantylength_default]
END

alter TABLE WarrantyBand alter column WarrantyLength INT not null

ALTER TABLE [dbo].[warrantyband] ADD  CONSTRAINT [warrantyband_warrantylength_default]  DEFAULT ((0)) FOR [warrantylength]
GO

IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_WarrantyBand_FYW]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[warrantyband] DROP CONSTRAINT [DF_WarrantyBand_FYW]
END

GO

ALTER TABLE [dbo].[warrantyband] ADD  CONSTRAINT [DF_WarrantyBand_FYW]  DEFAULT ((12)) FOR [firstYearWarPeriod]
GO

UPDATE warrantyband 
	set ItemId=ISNULL(s.ID,0),WarrantyLength=w.WarrantyLength*12,firstYearWarPeriod=firstYearWarPeriod*12
from warrantyband w INNER JOIN stockinfo s on w.Waritemno=s.itemno
where ISNULL(ItemId,0)=0
go

UPDATE warrantycodes 
	set ItemId=ISNULL(s.ID,0)
from warrantycodes w INNER JOIN stockinfo s on w.WarrantyNo=s.itemno
where ISNULL(ItemId,0)=0
go

Alter TABLE warrantyband alter column Waritemno VARCHAR(18) not null
go

Alter TABLE warrantycodes alter column WarrantyNo VARCHAR(18) not null
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[warrantyband]') AND name = N'pk_warrantyband')
ALTER TABLE [dbo].[warrantyband] DROP CONSTRAINT [pk_warrantyband]
GO

ALTER TABLE [dbo].[warrantyband] ADD  CONSTRAINT [pk_warrantyband] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[refcode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[warrantycodes]') AND name = N'pk_warrantycodes')
ALTER TABLE [dbo].[warrantycodes] DROP CONSTRAINT [pk_warrantycodes]
GO

ALTER TABLE [dbo].[warrantycodes] ADD  CONSTRAINT [pk_warrantycodes] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


insert into code (origbr,category,code,codedescript,statusflag,sortorder,reference,additional)
select distinct 0,'WAR',864,'RI Warranty category', 'L',0,864,null

where not exists(select code from code c where c.code=864 and c.category ='WAR')
	and not exists(select * from country where Countrycode in('M','Y','P','C'))
go
