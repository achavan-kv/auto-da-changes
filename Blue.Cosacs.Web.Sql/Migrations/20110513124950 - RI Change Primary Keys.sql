-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


select [acctno] ,
	[agrmtno] ,
	[itemID] ,
	[stocklocn] ,
	[buffno] ,
	[contractno] ,
	[ParentItemID], COUNT(*) as Nbr
	into #dupdels
from delivery where itemid=0
group by [acctno] ,
	[agrmtno] ,
	[itemID] ,
	[stocklocn] ,
	[buffno] ,
	[contractno] ,
	[ParentItemID] having COUNT(*)>1

UPDATE delivery set contractno=d.itemno
from delivery d INNER JOIN #dupdels dd on d.acctno = dd.acctno	 and d.agrmtno = dd.agrmtno
		and  d.itemID= dd.ItemID and d.stocklocn = dd.stocklocn and d.buffno = dd.buffno 
			and d.contractno = dd.contractno and d.ParentItemID = dd.ParentItemID
where d.itemid=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[delivery]') AND name = N'pk_delivery')
ALTER TABLE [dbo].[delivery] DROP CONSTRAINT [pk_delivery]
GO

ALTER TABLE [dbo].[delivery] ADD  CONSTRAINT [pk_delivery] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[itemID] ASC,
	[stocklocn] ASC,
	[buffno] ASC,
	[contractno] ASC,
	[ParentItemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

select [acctno] ,
	[agrmtno] ,
	[itemID] ,
	[stocklocn] ,
	[contractno] ,
	[ParentItemID], COUNT(*) as Nbr
	into #duplines
from lineitem where itemid=0
group by [acctno] ,
	[agrmtno] ,
	[itemID] ,
	[stocklocn] ,
	[contractno] ,
	[ParentItemID] having COUNT(*)>1
	
UPDATE lineitem set contractno=l.itemno
from lineitem l INNER JOIN #duplines dd on l.acctno = dd.acctno	 and l.agrmtno = dd.agrmtno
		and  l.itemID= dd.ItemID and l.stocklocn = dd.stocklocn  
			and l.contractno = dd.contractno and l.ParentItemID = dd.ParentItemID
where l.itemid=0

go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'pk_lineitem')
ALTER TABLE [dbo].[lineitem] DROP CONSTRAINT [pk_lineitem]
GO

ALTER TABLE [dbo].[lineitem] ADD  CONSTRAINT [pk_lineitem] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[itemID] ASC,
	[stocklocn] ASC,
	[contractno] ASC,
	[parentitemID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

