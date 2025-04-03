-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


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
ALTER TABLE [dbo].[promoprice] DROP CONSTRAINT [pk_promoprice]
GO

CREATE CLUSTERED INDEX [ixcl_promoprice] ON [dbo].[promoprice] 
(
	[ItemID] ASC,
	[stocklocn] ASC,
	[hporcash] ASC,
	[fromdate] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO