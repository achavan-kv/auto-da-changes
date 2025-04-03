-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
-- Related to issue: #15036


IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommission]') AND name = N'pk_SalesCommission')
ALTER TABLE [dbo].[SalesCommission] DROP CONSTRAINT [pk_SalesCommission]
GO


ALTER TABLE [dbo].[SalesCommission] ADD  CONSTRAINT [pk_SalesCommission] PRIMARY KEY CLUSTERED 
(
	[Employee] ASC,
	[RunDate] ASC,
	[AcctNo] ASC,
	[AgrmtNo] ASC,
	[ItemId] ASC,
	[CommissionType] ASC,
	[CommissionAmount] ASC,
	[Buffno] ASC,
	[ContractNo] ASC,
	[StockLocn] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO