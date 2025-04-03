SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InstallationBooking]') AND type in (N'U'))
CREATE TABLE [dbo].[InstallationBooking](
	[InstallationNo] [int] NOT NULL,
	[InstallationDate] [datetime] NOT NULL,
	[Zone] [varchar](12) NOT NULL,
	[TechnicianId] [int] NOT NULL,
	[Instructions] [varchar](200) NULL,
	[BookedBy] [int] NOT NULL,
	[BookedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO