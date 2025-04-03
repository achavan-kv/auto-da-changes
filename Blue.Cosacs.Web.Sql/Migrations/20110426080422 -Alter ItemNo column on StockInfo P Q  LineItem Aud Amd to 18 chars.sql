-- transaction: true
-- timeout: 4000
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'PK_StockInfo')
ALTER TABLE [dbo].[StockInfo] DROP CONSTRAINT [PK_StockInfo]

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'ix_StockInfo_IUPC_ID')
DROP INDEX [ix_StockInfo_IUPC_ID] ON [dbo].[StockInfo] WITH ( ONLINE = OFF )

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfo]') AND name = N'ix_StockInfo_IUPC_RepoItem')
DROP INDEX [ix_StockInfo_IUPC_RepoItem] ON [dbo].[StockInfo] WITH ( ONLINE = OFF )
GO

Alter TABLE StockInfo alter column itemno VARCHAR(18) not null
go

ALTER TABLE [dbo].[StockInfo] ADD  CONSTRAINT [PK_StockInfo] PRIMARY KEY CLUSTERED 
(
	[itemno] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [ix_StockInfo_IUPC_ID] ON [dbo].[StockInfo] 
(
	[IUPC] ASC,
	[ID] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]

CREATE NONCLUSTERED INDEX [ix_StockInfo_IUPC_RepoItem] ON [dbo].[StockInfo] 
(
	[IUPC] ASC,
	[RepossessedItem] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockPrice]') AND name = N'PK_StockPrice')
ALTER TABLE [dbo].[StockPrice] DROP CONSTRAINT [PK_StockPrice]
GO

Alter TABLE StockPrice alter column itemno VARCHAR(18) not null
go

ALTER TABLE [dbo].[StockPrice] ADD  CONSTRAINT [PK_StockPrice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	--[itemno] ASC,
	[branchno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockPriceAudit]') AND name = N'PK_StockPriceAudit')
ALTER TABLE [dbo].[StockPriceAudit] DROP CONSTRAINT [PK_StockPriceAudit]
GO

Alter TABLE StockPriceAudit alter column itemno VARCHAR(18) not null
go

ALTER TABLE [dbo].[StockPriceAudit] ADD  CONSTRAINT [PK_StockPriceAudit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	--[Itemno] ASC,
	[BranchNo] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantity]') AND name = N'PK_StockQuantity')
ALTER TABLE [dbo].[StockQuantity] DROP CONSTRAINT [PK_StockQuantity]
GO

Alter TABLE StockQuantity alter column itemno VARCHAR(18) not null
go


ALTER TABLE [dbo].[StockQuantity] ADD  CONSTRAINT [PK_StockQuantity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	--[itemno] ASC,
	[stocklocn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantityAuditCosacs]') AND name = N'PK_StockQuantityAuditCosacs')
ALTER TABLE [dbo].[StockQuantityAuditCosacs] DROP CONSTRAINT [PK_StockQuantityAuditCosacs]
GO

Alter TABLE StockQuantityAuditCosacs alter column itemno VARCHAR(18) not null
go

ALTER TABLE [dbo].[StockQuantityAuditCosacs] ADD  CONSTRAINT [PK_StockQuantityAuditCosacs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	--[ItemNo] ASC,
	[StockLocn] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'pk_lineitem')
ALTER TABLE [dbo].[lineitem] DROP CONSTRAINT [pk_lineitem]
GO

Alter TABLE lineitem alter column itemno VARCHAR(18) not null
Alter TABLE lineitem alter column parentitemno VARCHAR(18) not null
go



ALTER TABLE [dbo].[lineitem] ADD  CONSTRAINT [pk_lineitem] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[itemno] ASC,
	[stocklocn] ASC,
	[contractno] ASC,
	[parentitemno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

