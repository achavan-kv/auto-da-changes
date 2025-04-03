SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF NOT EXISTS (SELECT 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RICommittedStock]') AND type in (N'U'))
CREATE TABLE [dbo].[RICommittedStock](
	[Record] [varchar](100) NULL
) ON [PRIMARY]

GO