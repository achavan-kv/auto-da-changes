SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[InstallationSparePart]') AND type in (N'U'))
BEGIN
	DROP TABLE [dbo].[InstallationSparePart]
END   

CREATE TABLE [dbo].[InstallationSparePart](
	[InstallationNo] [int] NOT NULL,	
	[PartNo] [varchar](8) NOT NULL,
	[StockLocation] [smallint] NOT NULL,
	[IsNonCourts] [bit] NOT NULL,
	[Description] [varchar](30) NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[Quantity] [float] NOT NULL,	
	[Total] [money] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[InstallationNo] ASC,
	[PartNo] ASC,
	[StockLocation] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

