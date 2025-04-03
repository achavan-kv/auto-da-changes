SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InstallationChargeTo]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[InstallationChargeTo]
END   

CREATE TABLE [dbo].[InstallationChargeAnalysis](
	[InstallationNo] [int] NOT NULL,
	[BreakDownCode] [varchar](12) NOT NULL,
	[Electrical] [money] NOT NULL, 
	[Furniture] [money] NOT NULL
PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC,
	[BreakDownCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
