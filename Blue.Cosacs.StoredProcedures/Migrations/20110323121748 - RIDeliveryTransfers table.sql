SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RIDeliveryTransfers]') AND type in (N'U'))
CREATE TABLE [dbo].[RIDeliveryTransfers](
	[Record] [varchar](100) NULL
) ON [PRIMARY]

GO

