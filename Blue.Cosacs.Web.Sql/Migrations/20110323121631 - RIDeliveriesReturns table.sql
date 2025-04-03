SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RIDeliveriesReturns]') AND type in (N'U'))
CREATE TABLE [dbo].[RIDeliveriesReturns](
	[Record] [varchar](200) NULL
) ON [PRIMARY]

GO
