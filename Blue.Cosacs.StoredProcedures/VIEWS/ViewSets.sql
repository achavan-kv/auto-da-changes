IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'V'
		   AND so.NAME = 'ViewSets'
		   AND ss.name = 'dbo')
DROP VIEW  ViewSets
GO

CREATE VIEW dbo.ViewSets
AS
SELECT  * FROM dbo.Sets
        
