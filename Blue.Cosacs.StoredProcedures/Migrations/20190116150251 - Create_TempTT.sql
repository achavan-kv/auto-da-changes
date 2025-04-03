IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tempTT]') AND type in (N'U'))
BEGIN
/****** Object:  Table [dbo].[tempTT]    Script Date: 1/11/2019 2:35:48 PM ******/
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[tempTT](
	[ScoreType] [nvarchar](10) NULL,
	[Country] [nvarchar](100) NULL,
	[DeclineScore] [nvarchar](100) NULL,
	[ReferScore] [nvarchar](100) NULL,
	[BureauMinimum] [nvarchar](100) NULL,
	[BureauMaximum] [nvarchar](100) NULL,
	[Region] [nvarchar](100) NULL,
	[InterceptScore] [nvarchar](100) NULL,
	[Type] [nvarchar](100) NULL,
	[Result] [nvarchar](1000) NULL,
	[State] [nvarchar](100) NULL,
	[RuleName] [nvarchar](1000) NULL,
	[ApplyRF] [nvarchar](10) NULL,
	[ApplyHP] [nvarchar](10) NULL,
	[ReferDeclined] [nvarchar](10) NULL,
	[ReferAccepted] [nvarchar](10) NULL,
	[RuleRejects] [nvarchar](10) NULL,
	[ReferToBureau] [nvarchar](10) NULL,
	[ClauseType] [nvarchar](10) NULL,
	[ClauseState] [nvarchar](50) NULL,
	[Operand] [nvarchar](100) NULL,
	[OlType] [nvarchar](100) NULL,
	[OlTableName] [nvarchar](100) NULL,
	[Operator] [nvarchar](100) NULL,
	[O2Operand] [nvarchar](100) NULL,
	[O2Type] [nvarchar](100) NULL
) ON [PRIMARY]

END


