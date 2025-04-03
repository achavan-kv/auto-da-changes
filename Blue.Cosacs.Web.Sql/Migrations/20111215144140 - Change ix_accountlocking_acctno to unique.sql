-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[AccountLocking]') AND name = N'ix_accountlocking_acctno')
DROP INDEX [ix_accountlocking_acctno] ON [dbo].[AccountLocking] WITH ( ONLINE = OFF )
GO

CREATE unique NONCLUSTERED INDEX [ix_accountlocking_acctno] ON [dbo].[AccountLocking] 
(
	[acctno] ASC
)
ON [PRIMARY]
GO
