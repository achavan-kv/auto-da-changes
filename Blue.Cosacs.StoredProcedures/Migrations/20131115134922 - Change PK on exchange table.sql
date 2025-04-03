-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

ALTER TABLE [dbo].[Exchange] DROP CONSTRAINT [pk_Exchange]
GO

ALTER TABLE [dbo].[Exchange] ADD  CONSTRAINT [pk_Exchange] PRIMARY KEY CLUSTERED 
(
	[BuffNo] ASC,
	[AcctNo] ASC,
	[AgrmtNo] ASC,
	[ItemID] ASC,
	[StockLocn] ASC,
	[WarrantyID] asc
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO

