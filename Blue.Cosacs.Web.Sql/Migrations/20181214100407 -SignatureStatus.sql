IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'SignatureStatus'
   )
BEGIN
DROP table SignatureStatus
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[SignatureStatus](
	[SignId] [int] IDENTITY(1,1) NOT NULL,
	[acctNo] [varchar](20) NULL,
	[custId] [varchar](20) NULL,
	[status] [varchar](10) NULL,
	[updatedDate] [datetime] NULL,
	[isApproved] [bit] NOT NULL CONSTRAINT [DF_SignatureStatus_isApproved]  DEFAULT ((0)),
	[approvedDate] [datetime] NULL,
	[isActive] [bit] NOT NULL CONSTRAINT [DF_SignatureStatus_isActive]  DEFAULT ((0)),
	[activeDate] [datetime] NULL,
 CONSTRAINT [PK_SignatureStatus] PRIMARY KEY CLUSTERED 
(
	[SignId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
