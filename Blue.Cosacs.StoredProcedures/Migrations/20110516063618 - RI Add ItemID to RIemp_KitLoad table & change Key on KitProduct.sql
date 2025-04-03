-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ItemID'
               AND OBJECT_NAME(id) = 'RItemp_Kitload')
BEGIN
  ALTER TABLE RItemp_Kitload ADD ItemID INT not null default 0
END
go

IF NOT EXISTS (SELECT * FROM syscolumns
			   WHERE name = 'ComponentID'
               AND OBJECT_NAME(id) = 'RItemp_Kitload')
BEGIN
  ALTER TABLE RItemp_Kitload ADD ComponentID INT not null default 0
END
go

delete kitproduct 
where ItemID=0 or ComponentID=0
go

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[kitproduct]') AND name = N'pk_kitproduct')
ALTER TABLE [dbo].[kitproduct] DROP CONSTRAINT [pk_kitproduct]
GO

ALTER TABLE [dbo].[kitproduct] ADD  CONSTRAINT [pk_kitproduct] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC,
	[ComponentID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
