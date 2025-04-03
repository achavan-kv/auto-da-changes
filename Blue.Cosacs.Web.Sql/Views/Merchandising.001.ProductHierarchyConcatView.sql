SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM SYSOBJECTS 
           WHERE NAME = 'ProductHierarchyConcatView'
           AND xtype = 'V')
BEGIN 
DROP VIEW [Merchandising].[ProductHierarchyConcatView]
END
GO 

-- ========================================================================
-- Version:		<001> 
-- ========================================================================

CREATE VIEW [Merchandising].[ProductHierarchyConcatView]
AS

SELECT [Id],[ProductId],[Hierarchy],[LevelTags]
FROM [Merchandising].[ProductHierarchyConcatTable] WITH(nolock)

GO




