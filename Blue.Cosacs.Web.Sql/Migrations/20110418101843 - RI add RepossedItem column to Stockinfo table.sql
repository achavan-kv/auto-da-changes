-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'RepossessedItem'
               AND OBJECT_NAME(id) = 'StockInfo')
BEGIN
  ALTER TABLE StockInfo ADD RepossessedItem BIT not null default(0)
END

IF   not EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'ix_StockInfo_IUPC_RepoItem')
BEGIN
CREATE NONCLUSTERED INDEX [ix_StockInfo_IUPC_RepoItem] ON [dbo].[StockInfo] 
(
	[IUPC] ASC,
	[repossessedItem] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
END

GO