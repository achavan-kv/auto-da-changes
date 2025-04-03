/*
User View for Cosacs including columns from CourtsPerson

*/


IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'V'
		   AND so.NAME = 'UserView'
		   AND ss.name = 'dbo')
DROP VIEW  dbo.[UserView]
GO


CREATE VIEW  dbo.[UserView]
AS
SELECT u.*, c.*
FROM Admin.[User] u INNER JOIN CourtsPersontable c on u.Id=c.UserId
		
GO


