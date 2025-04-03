-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[instantcreditflag]') AND type in (N'U'))
DROP TABLE [dbo].[instantcreditflag]
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[instantcreditflag](
	[origbr] [smallint] NULL,
	[custid] [varchar](20) NOT NULL,
	[acctno] [varchar](12) NOT NULL,
	[checktype] [varchar](4) NOT NULL,
	[datecleared] [datetime] NULL,
	[empeenopflg] [int] NULL,
	[unclearedby] [int] NULL
) ON [PRIMARY]
GO




