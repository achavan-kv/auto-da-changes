-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


alter TABLE SalesCommissionExtraSpiffs alter column ItemNo VARCHAR(18) not null

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SalesCommissionExtraSpiffs]') AND name = N'pk_SalesCommissionExtraSpiffs')
ALTER TABLE [dbo].[SalesCommissionExtraSpiffs] DROP CONSTRAINT [pk_SalesCommissionExtraSpiffs]
GO

ALTER TABLE [dbo].[SalesCommissionExtraSpiffs] ADD  CONSTRAINT [pk_SalesCommissionExtraSpiffs] PRIMARY KEY CLUSTERED 
(
	[AcctNo] ASC,
	[ItemNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
