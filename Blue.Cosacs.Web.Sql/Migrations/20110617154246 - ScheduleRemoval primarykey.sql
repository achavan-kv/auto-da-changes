-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[ScheduleRemoval]') AND name = N'pk_ScheduleRemoval')
	ALTER TABLE [dbo].[ScheduleRemoval] DROP CONSTRAINT [pk_ScheduleRemoval]
GO

ALTER TABLE [dbo].[ScheduleRemoval] ADD  CONSTRAINT [pk_ScheduleRemoval] PRIMARY KEY CLUSTERED 
(
	[AcctNo] ASC,
	[AgrmtNo] ASC,
	[ItemId] ASC,
	[StockLocn] ASC,
	[DateRemoved] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
