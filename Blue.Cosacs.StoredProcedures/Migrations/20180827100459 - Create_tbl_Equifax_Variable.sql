
SET ANSI_NULLS OFF
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING OFF
GO

CREATE TABLE [dbo].[Equifax_Variable](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Variable] [varchar](50) NOT NULL,
	[Weightage] [float] NOT NULL,
	[Flag_CustomerStatus] [char](2) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO