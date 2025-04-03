-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-- Kit Item ID
IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'Kitproduct')
BEGIN
  ALTER TABLE kitproduct ADD ItemID int not null default 0
END

-- Component Item ID
IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ComponentID'
               AND OBJECT_NAME(id) = 'Kitproduct')
BEGIN
  ALTER TABLE kitproduct ADD ComponentID int not null default 0
END
go

UPDATE Kitproduct
	set ItemID=s.ID 
From Kitproduct k,StockInfo s 
Where s.id=(select MIN(id) from StockInfo s where k.itemno=s.itemno)
	and k.ItemID=0
	
UPDATE Kitproduct
	set ComponentID=s.ID 
From Kitproduct k,StockInfo s 
Where s.id=(select MIN(id) from StockInfo s where k.Componentno=s.itemno)
	and k.ComponentID=0

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[kitproduct]') AND name = N'pk_kitproduct')
ALTER TABLE [dbo].[kitproduct] DROP CONSTRAINT [pk_kitproduct]
GO

ALTER TABLE [dbo].[kitproduct] ADD  CONSTRAINT [pk_kitproduct] PRIMARY KEY CLUSTERED 
(
	[itemno] ASC,
	[componentno] ASC,
	[ItemID] ASC,
	[ComponentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

