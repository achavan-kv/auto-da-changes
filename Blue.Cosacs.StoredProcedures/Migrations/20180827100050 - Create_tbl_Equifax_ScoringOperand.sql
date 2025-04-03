
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Equifax_ScoringOperand](
	[OperandID] [int] IDENTITY(1,1) NOT NULL,
	[OperandName] [varchar](50) NOT NULL,
	[OperandType] [varchar](20) NOT NULL,
	[OperandOptions] [smallint] NOT NULL,
	[DropDownName] [varchar](25) NOT NULL,
 CONSTRAINT [PK_Equifax_ScoringOperand] PRIMARY KEY CLUSTERED 
(
	[OperandID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[Equifax_ScoringOperand] ADD  CONSTRAINT [DF_Equifax_ScoringOperand_OperandName]  DEFAULT ('') FOR [OperandName]
GO

ALTER TABLE [dbo].[Equifax_ScoringOperand] ADD  CONSTRAINT [DF_Equifax_ScoringOperand_OperandType]  DEFAULT ('') FOR [OperandType]
GO

ALTER TABLE [dbo].[Equifax_ScoringOperand] ADD  CONSTRAINT [DF_Equifax_ScoringOperand_OperandOptions]  DEFAULT ((0)) FOR [OperandOptions]
GO

ALTER TABLE [dbo].[Equifax_ScoringOperand] ADD  CONSTRAINT [DF_Equifax_ScoringOperand_DropDownName]  DEFAULT ('') FOR [DropDownName]
GO
