SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InstallationResolution]') AND type in (N'U'))
CREATE TABLE [dbo].[InstallationResolution](
	[InstallationNo] [int] NOT NULL,	
	[PrimaryChargeTo] [varchar](12) NOT NULL,
	[AdditionalCost] [money] NOT NULL DEFAULT(0),
	[LabourCost] [money] NOT NULL DEFAULT(0),
	[NonCourtsPartCost] [money] NOT NULL DEFAULT(0),
	[CourtsPartCost] [money] NOT NULL DEFAULT(0),
	[TotalCost] [money] NOT NULL,
	[IsCompleted] [bit] NOT NULL DEFAULT(0),
PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO