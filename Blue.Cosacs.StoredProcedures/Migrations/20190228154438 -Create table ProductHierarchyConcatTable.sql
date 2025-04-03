SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'ProductHierarchyConcatTable'
           AND xtype = 'U')
BEGIN 
CREATE TABLE [Merchandising].[ProductHierarchyConcatTable](
	[Id] [int] NULL,
	[ProductId] [int] NULL,
	[Hierarchy] [varchar](max) NULL,
	[LevelTags] [varchar](max) NULL
) 

END
GO




