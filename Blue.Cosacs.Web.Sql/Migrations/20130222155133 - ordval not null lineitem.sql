ALTER TABLE [dbo].[lineitem] DISABLE TRIGGER ALL

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'LI1') 
AND (select  is_nullable 
from    sys.columns 
where   object_id = object_id('lineitem') 
        and name = 'ordval') = 1
DROP INDEX [LI1] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'IX_LineItemForInstallation') AND (select  is_nullable 
from    sys.columns 
where   object_id = object_id('lineitem') 
        and name = 'ordval') = 1
DROP INDEX [IX_LineItemForInstallation] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_quantity') AND (select  is_nullable 
from    sys.columns 
where   object_id = object_id('lineitem') 
        and name = 'ordval') = 1
DROP INDEX [ix_quantity] ON [dbo].[lineitem] WITH ( ONLINE = OFF )
GO


IF (select  is_nullable 
from    sys.columns 
where   object_id = object_id('lineitem') 
        and name = 'ordval') = 1 
BEGIN
UPDATE dbo.lineitem
SET ordval = 0
WHERE ordval IS NULL
END

GO

IF (select  is_nullable 
from    sys.columns 
where   object_id = object_id('lineitem') 
        and name = 'ordval') = 1
BEGIN
	ALTER TABLE dbo.lineitem
	ALTER COLUMN ordval MONEY NOT NULL
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'ix_quantity')
BEGIN
CREATE NONCLUSTERED INDEX [ix_quantity] ON [dbo].[lineitem] 
(
	[quantity] ASC
)
INCLUDE ( [acctno],
[agrmtno],
[ordval],
[parentlocation],
[ItemID],
[ParentItemID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'IX_LineItemForInstallation')
BEGIN
CREATE NONCLUSTERED INDEX [IX_LineItemForInstallation] ON [dbo].[lineitem] 
(
	[ItemID] ASC,
	[quantity] ASC
)
INCLUDE ( [acctno],
[agrmtno],
[ordval],
[parentlocation],
[ParentItemID]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[lineitem]') AND name = N'LI1') 
BEGIN
CREATE NONCLUSTERED INDEX [LI1] ON [dbo].[lineitem] 
(
	[itemtype] ASC,
	[ordval] ASC
)
INCLUDE ( [acctno],
[agrmtno],
[ItemID],
[stocklocn],
[taxamt]) WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
END
GO


ALTER TABLE [dbo].[lineitem] ENABLE TRIGGER ALL