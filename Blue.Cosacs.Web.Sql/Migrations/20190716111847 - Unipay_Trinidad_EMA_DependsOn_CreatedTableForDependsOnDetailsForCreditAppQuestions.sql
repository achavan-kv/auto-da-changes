IF EXISTS (SELECT 1 FROM sysobjects, INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS
   WHERE NAME = 'EMA_DependsOnQuestions' AND CONSTRAINT_NAME ='FK_EMA_DependsOnQuestions_EMA_DependsOn'
   )
BEGIN
	ALTER TABLE [dbo].[EMA_DependsOnQuestions] DROP CONSTRAINT [FK_EMA_DependsOnQuestions_EMA_DependsOn]
END 
GO
IF EXISTS (SELECT 1 FROM sysobjects 
   WHERE NAME = 'EMA_DependsOn'
   )
BEGIN
	DROP table EMA_DependsOn
END 
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[EMA_DependsOn](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[QId] [int] NOT NULL,
	[catalog] [nvarchar](500) NULL,
 CONSTRAINT [PK_DependsOn] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[EMA_DependsOn]  WITH CHECK ADD  CONSTRAINT [FK_EMA_DependsOn_CreditAppQuestionnaire] FOREIGN KEY([QId])
REFERENCES [dbo].[CreditAppQuestionnaire] ([QuestionId])
GO
ALTER TABLE [dbo].[EMA_DependsOn] CHECK CONSTRAINT [FK_EMA_DependsOn_CreditAppQuestionnaire]
GO
