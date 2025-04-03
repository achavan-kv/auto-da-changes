-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockInfoAudit]') AND name = N'PK_StockInfoAudit')
ALTER TABLE [dbo].[StockInfoAudit] DROP CONSTRAINT [PK_StockInfoAudit]
GO

ALTER TABLE [dbo].[StockInfoAudit] ADD  CONSTRAINT [PK_StockInfoAudit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockPriceAudit]') AND name = N'PK_StockPriceAudit')
ALTER TABLE [dbo].[StockPriceAudit] DROP CONSTRAINT [PK_StockPriceAudit]
GO

ALTER TABLE [dbo].[StockPriceAudit] ADD  CONSTRAINT [PK_StockPriceAudit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[BranchNo] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantityAuditCosacs]') AND name = N'PK_StockQuantityAuditCosacs')
ALTER TABLE [dbo].[StockQuantityAuditCosacs] DROP CONSTRAINT [PK_StockQuantityAuditCosacs]
GO

ALTER TABLE [dbo].[StockQuantityAuditCosacs] ADD  CONSTRAINT [PK_StockQuantityAuditCosacs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[StockLocn] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem_amend]') AND name = N'pk_lineitem_amend')
ALTER TABLE [dbo].[lineitem_amend] DROP CONSTRAINT [pk_lineitem_amend]
GO

ALTER TABLE [dbo].[lineitem_amend] ADD  CONSTRAINT [pk_lineitem_amend] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[ItemId] ASC,
	[stocklocn] ASC,
	[contractno] ASC,
	[ParentItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO