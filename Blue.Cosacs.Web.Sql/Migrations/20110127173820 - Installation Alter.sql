IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Installation]') AND type in (N'U'))
DROP TABLE [dbo].[Installation]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Installation](
	[InstallationNo] [int] IDENTITY(1,1) NOT NULL,
	[BranchNo] [smallint] NOT NULL,
	[AcctNo] [char](12) NOT NULL,
	[AgreementNo] [int] NOT NULL,
	[ItemNo] [varchar](8) NOT NULL,
	[StockLocation] [smallint] NOT NULL,
	[InstallationDate] [datetime] NULL,
	[Status] [varchar](10) NOT NULL,
	[CreatedBy] [int] NULL,
	[LastUpdatedBy] [int] NULL,
	[CreatedOn] [datetime] NULL,
	[LastUpdatedOn] [datetime] NULL,
	[Comment] [varchar](80) NULL,	
 CONSTRAINT [PK_Installation] PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO



