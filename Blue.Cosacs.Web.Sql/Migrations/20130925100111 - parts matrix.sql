
CREATE TABLE [Service].[PartsCostMatrix](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Type] [varchar](50) NOT NULL,
	[Internal] [money] NOT NULL,
	[FirstYearWarranty] [money] NOT NULL,
	[ExtendedWarranty] [money] NOT NULL,
	[Customer] [money] NOT NULL,
	[Level_1] [varchar](50) SPARSE  NULL,
	[Level_2] [varchar](50) SPARSE  NULL,
	[Level_3] [varchar](50) SPARSE  NULL,
	[Level_4] [varchar](50) SPARSE  NULL,
	[Level_5] [varchar](50) SPARSE  NULL,
	[Level_6] [varchar](50) SPARSE  NULL,
	[Level_7] [varchar](50) SPARSE  NULL,
	[Level_8] [varchar](50) SPARSE  NULL,
	[Level_9] [varchar](50) SPARSE  NULL,
	[Level_10] [varchar](50) SPARSE  NULL,
	[ItemList] [varchar](4000) SPARSE  NULL,
	[IsGroupFilter] [bit] NOT NULL,
	[Label] [varchar](50) NULL,
 CONSTRAINT [PK_Id_PartsCostMatrix] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


