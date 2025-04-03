-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleAudit]') AND name = N'pk_ScheduleAudit')
ALTER TABLE [dbo].[ScheduleAudit] DROP CONSTRAINT [pk_ScheduleAudit]
GO

ALTER TABLE [dbo].[ScheduleAudit] ADD  CONSTRAINT [pk_ScheduleAudit] PRIMARY KEY CLUSTERED 
(
	[acctno] ASC,
	[agrmtno] ASC,
	[itemId] ASC,
	[stocklocn] ASC,
	[buffbranchno] ASC,
	[buffno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
