-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

-----------------------------------------------
-- Add new primary key
-----------------------------------------------			

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[proposalflag]') AND name = N'pk_proposalflag')
  ALTER TABLE [dbo].[proposalflag] DROP CONSTRAINT [pk_proposalflag]
GO


ALTER TABLE [dbo].[proposalflag] ADD  CONSTRAINT [pk_proposalflag] PRIMARY KEY CLUSTERED
(
	[custid] ASC,
	[acctno] ASC,
	[dateprop] ASC,
	[checktype] ASC
	
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
