-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[CashierTotals]') AND name = N'idx_CashierTotals_empeeno')
DROP INDEX [idx_CashierTotals_empeeno] ON [dbo].[CashierTotals] WITH ( ONLINE = OFF )
GO

CREATE NONCLUSTERED INDEX [idx_CashierTotals_empeeno] ON [dbo].[CashierTotals] 
(
	[empeeno] ASC,
	[branchno] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO

