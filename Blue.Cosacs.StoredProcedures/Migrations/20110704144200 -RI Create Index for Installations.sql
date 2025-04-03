-- transaction: false
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_lineitem_itemNo')
DROP INDEX [ix_lineitem_itemNo] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_lineitem_itemId')
DROP INDEX [ix_lineitem_itemId] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_lineitem_ItemId] ON [dbo].[lineitem] 
(
	[itemid] ASC,
	[acctno] ASC
)
INCLUDE ( 
[parentitemId],[quantity])
WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 80) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'IX_LineItemForInstallation')
DROP INDEX [IX_LineItemForInstallation] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_LineItemForInstallation] ON [dbo].[lineitem] 
(
	[itemId] ASC,
	[quantity] ASC
)
INCLUDE ([acctno],[agrmtno],[ordval],[parentlocation],[ParentItemID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[delivery]') AND name = N'ix_delivery_delorcoll')
DROP INDEX [ix_delivery_delorcoll] ON [dbo].[delivery] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_delivery_delorcoll] ON [dbo].[delivery] 
(
	[delorcoll] ASC,
	[acctno] ASC,
	[itemid] ASC
)
INCLUDE ( [agrmtno],[datedel],
[stocklocn])WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 50) ON [PRIMARY]
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_qtydiff')
DROP INDEX [ix_qtydiff] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_qtydiff] ON [dbo].[lineitem] 
(
	[qtydiff] ASC,
	[acctno] ASC,
	[itemid] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 100) ON [PRIMARY]
GO

----?????????????????????????????
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_qtydiffacctno')
DROP INDEX [ix_qtydiffacctno] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO


CREATE NONCLUSTERED INDEX [ix_qtydiffacctno] ON [dbo].[lineitem] 
(
	[qtydiff] ASC,
	[acctno] ASC
)WITH (PAD_INDEX  = ON, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 100) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'LI1')
DROP INDEX [LI1] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [LI1] ON [dbo].[lineitem] 
(
	[itemtype] ASC,
	[ordval] ASC
)
INCLUDE ( [acctno],
[agrmtno],
[itemid],
[stocklocn],
[taxamt]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_quantity')
DROP INDEX [ix_quantity] ON [dbo].[lineitem] WITH ( ONLINE = OFF )

GO
CREATE NONCLUSTERED INDEX [ix_quantity]
ON [dbo].[lineitem] ([quantity])
INCLUDE ([acctno],[agrmtno],[ordval],[parentlocation],[ItemID],[ParentItemID])
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[agreement]') AND name = N'ix_acctno')
DROP INDEX [ix_acctno] ON [dbo].[agreement] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[agreement]') AND name = N'ix_dateauth')
DROP INDEX [ix_dateauth] ON [dbo].[agreement] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [ix_dateauth]
ON [dbo].[agreement] ([dateauth])
INCLUDE ([acctno],[agrmtno],[dateagrmt],[holdprop])


