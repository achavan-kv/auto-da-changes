-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[schedule]') AND name = N'pk_schedule')
ALTER TABLE [dbo].[schedule] DROP CONSTRAINT [pk_schedule]
GO


ALTER TABLE [dbo].[schedule] ADD  CONSTRAINT [pk_schedule] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[itemid] ASC,
	[contractno] ASC,
	[stocklocn] ASC,
	[buffbranchno] ASC,
	[buffno] ASC,
	[delorcoll] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO


