-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
GO

/****** Object:  Table [dbo].[StoreCardLastAppSuccess]    Script Date: 06/17/2011 12:28:57 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF NOT EXISTS (SELECT * FROM sysobjects WHERE NAME = 'StoreCardLastAppSuccess')
CREATE TABLE [dbo].[StoreCardLastAppSuccess](
	[custid] [varchar](20) NOT NULL,
	[Title] [varchar](25) NULL,
	[FirstName] [varchar](30) NULL,
	[Name] [varchar](60) NOT NULL,
	[StoreCardLimit] [money] NULL,
	[cusaddr1] [varchar](50) NULL,
	[cusaddr2] [varchar](50) NULL,
	[cusaddr3] [varchar](50) NULL,
	[cuspocode] [varchar](10) NULL,
	[branchname] [varchar](20) NOT NULL,
	[branchaddr1] [varchar](26) NOT NULL,
	[branchaddr2] [varchar](26) NULL,
	[branchaddr3] [varchar](26) NULL,
	[ApprovalDate] [datetime] NULL,
	[OfferExpiryDate] [datetime] NULL,
	[runno] [smallint] NULL,
	[HomePhone] [varchar](20) NULL,
	[MobilePhone] [varchar](20) NULL,
	[branchno] [smallint] NOT NULL
) ON [PRIMARY]

GO
