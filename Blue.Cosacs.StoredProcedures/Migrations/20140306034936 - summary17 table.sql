-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here


CREATE TABLE [dbo].[summary17](
	[monthid] [int] NULL,
	[branch] [varchar](30) NOT NULL,
	[accttype] [char](1) NULL,
	[category] [smallint] NOT NULL,
	[Qty] [float] NOT NULL,
	[SalesValue] [float] NULL
) ON [PRIMARY]

GO

