-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF   not EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[locklogfinal]') AND type in (N'U'))
Begin
CREATE TABLE [dbo].[locklogfinal](
	[EVENTTYPE] [varchar](32) NULL,
	[EventInfo] [nvarchar](255) NULL,
	[spidlocking] [int] NOT NULL,
	[Parameters] [int] NULL,
	[lockedbytext] [nvarchar](255) NULL,
	[lockedouttext] [nvarchar](255) NULL,
	[dateblock] [datetime] NOT NULL,
	[userblocking] [nchar](256) NULL,
	[spidlockedout] [int] NULL,
	[daterun] [datetime] NULL,
	[last_batch] [datetime] NULL
) ON [PRIMARY]

End
