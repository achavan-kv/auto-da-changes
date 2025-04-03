/*
Role View for Cosacs 

*/


IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'V'
		   AND so.NAME = 'RoleView'
		   AND ss.name = 'dbo')
DROP VIEW  dbo.[RoleView]
GO


CREATE VIEW  dbo.[RoleView]
AS
SELECT *
FROM Admin.[Role] 

GO

