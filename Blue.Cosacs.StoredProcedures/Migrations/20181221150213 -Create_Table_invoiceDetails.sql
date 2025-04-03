-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
GO

/****** Object:  Table [dbo].[invoiceDetails]    Script Date: 12/22/2018 4:12:58 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO
IF NOT EXISTS ( SELECT [name] FROM sys.tables WHERE [name] = 'invoiceDetails' )
BEGIN

CREATE TABLE [dbo].[invoiceDetails](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[acctno] [varchar](12) NOT NULL,
	[agrmtno] [int] NOT NULL,
	[InvoiceVersion] [varchar](15) NOT NULL,
	[datedel] [smalldatetime] NOT NULL,
	[itemno] [varchar](18) NOT NULL,
	[stocklocn] [smallint] NOT NULL,
	[quantity] [float] NOT NULL,
	[branchno] [smallint] NULL,
	[Price] [money] NULL,
	[taxamt] [float] NULL,
	[ItemID] [int] NULL,
	[ParentItemID] [int] NULL,
	[AgreementInvNoVersion] [VARCHAR](20) NULL,
PRIMARY KEY NONCLUSTERED 
(
	[id] ASC,
	[acctno] ASC,
	[agrmtno] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


END

SET ANSI_PADDING OFF
GO


