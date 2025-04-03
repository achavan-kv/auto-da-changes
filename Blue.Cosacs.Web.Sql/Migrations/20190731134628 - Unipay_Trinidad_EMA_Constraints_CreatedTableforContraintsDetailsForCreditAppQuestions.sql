
IF EXISTS (SELECT * FROM sysobjects 
   WHERE NAME = 'EMA_Constraints'
   )
BEGIN
	DROP table EMA_Constraints
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EMA_Constraints](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QId] [int] NOT NULL,
	[Max] [bigint] NULL,
	[Min] [bigint] NULL,
	[Regex] [nvarchar](500) NULL,
	[MaxErrorMessage] [nvarchar](500) NULL,
	[MinErrorMessage] [nvarchar](500) NULL,
	[RegexErrorMessage] [nvarchar](500) NULL,
 CONSTRAINT [PK_Constraints] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[EMA_Constraints]  WITH CHECK ADD  CONSTRAINT [FK_EMA_Constraints_CreditAppQuestionnaire] FOREIGN KEY([QId])
REFERENCES [dbo].[CreditAppQuestionnaire] ([QuestionId])
GO
ALTER TABLE [dbo].[EMA_Constraints] CHECK CONSTRAINT [FK_EMA_Constraints_CreditAppQuestionnaire]
GO
SET IDENTITY_INSERT [dbo].[EMA_Constraints] ON 
GO
INSERT [dbo].[EMA_Constraints] 
	([Id], [QId], [Max], [Min], [Regex], [MaxErrorMessage], [MinErrorMessage], [RegexErrorMessage]) 
VALUES 
	(1, 1001, 85, 18, N'^(0?[1-9]|1[0-2])\/(0?[1-9]|[12][0-9]|3[01])\/\d{4}$', N'Age should be less than ##age## years.', N'Age should be greater than ##age## years.', N'The answer does not match the required format, the format should be MM/dd/yyyy.')
GO
SET IDENTITY_INSERT [dbo].[EMA_Constraints] OFF
GO



