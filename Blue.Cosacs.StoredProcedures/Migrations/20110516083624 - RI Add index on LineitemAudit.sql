-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[LineitemAudit]') AND name = N'IX_LineitemAudit_RunNo')
DROP INDEX [IX_LineitemAudit_RunNo] ON [dbo].[LineitemAudit] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [IX_LineitemAudit_RunNo] ON [dbo].[LineitemAudit] 
(
	[RunNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
