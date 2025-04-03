-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

UPDATE StockQuantity
	set ID=i.ID 
From Stockinfo i INNER JOIN StockQuantity q on i.itemno=q.itemno
where q.id is null

go
IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'Trig_StockPrice_InsertUpdate')
BEGIN
	disable trigger Trig_StockPrice_InsertUpdate on StockPrice
END

go

UPDATE StockPrice
	set ID=i.ID 
From Stockinfo i INNER JOIN StockPrice p on i.itemno=p.itemno
where p.id is null
go

IF EXISTS (SELECT * FROM sysobjects
		   WHERE xtype = 'TR'
		   AND name = 'Trig_StockPrice_InsertUpdate')
BEGIN
	enable trigger Trig_StockPrice_InsertUpdate on StockPrice
END

go 

Alter TABLE StockPrice alter column [ID] INT not null
go

Alter TABLE StockQuantity alter column [ID] INT not null

go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockPrice]') AND name = N'PK_StockPrice')
ALTER TABLE [dbo].[StockPrice] DROP CONSTRAINT [PK_StockPrice]
GO

ALTER TABLE [dbo].[StockPrice] ADD  CONSTRAINT [PK_StockPrice] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[itemno] ASC,
	[branchno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantity]') AND name = N'PK_StockQuantity')
ALTER TABLE [dbo].[StockQuantity] DROP CONSTRAINT [PK_StockQuantity]
GO

ALTER TABLE [dbo].[StockQuantity] ADD  CONSTRAINT [PK_StockQuantity] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[itemno] ASC,
	[stocklocn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ID'
               AND OBJECT_NAME(id) = 'StockPriceAudit')
BEGIN
  ALTER TABLE StockPriceAudit ADD ID INT 
END
go

UPDATE StockPriceAudit
	set ID=i.ID 
From Stockinfo i INNER JOIN StockPriceAudit p on i.itemno=p.itemno
where p.id is null
go

Alter TABLE StockPriceAudit alter column [ID] INT not null

go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockPriceAudit]') AND name = N'PK_StockPriceAudit')
ALTER TABLE [dbo].[StockPriceAudit] DROP CONSTRAINT [PK_StockPriceAudit]
GO

ALTER TABLE [dbo].[StockPriceAudit] ADD  CONSTRAINT [PK_StockPriceAudit] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[Itemno] ASC,
	[BranchNo] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ID'
               AND OBJECT_NAME(id) = 'StockQuantityAuditCosacs')
BEGIN
  ALTER TABLE StockQuantityAuditCosacs ADD ID INT 
END

go

UPDATE StockQuantityAuditCosacs
	set ID=i.ID 
From Stockinfo i INNER JOIN StockQuantityAuditCosacs q on i.itemno=q.itemno
where q.id is null
go

Alter TABLE StockQuantityAuditCosacs alter column [ID] INT not null

go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[StockQuantityAuditCosacs]') AND name = N'PK_StockQuantityAuditCosacs')
ALTER TABLE [dbo].[StockQuantityAuditCosacs] DROP CONSTRAINT [PK_StockQuantityAuditCosacs]
GO

ALTER TABLE [dbo].[StockQuantityAuditCosacs] ADD  CONSTRAINT [PK_StockQuantityAuditCosacs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC,
	[ItemNo] ASC,
	[StockLocn] ASC,
	[DateChange] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
