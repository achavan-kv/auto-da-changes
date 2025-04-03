IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[TestGetRandPersonView]'))
	DROP VIEW [dbo].TestGetRandPersonView
GO

CREATE VIEW [dbo].TestGetRandPersonView
AS
SELECT TOP 1 empeeno
FROM courtsperson
ORDER BY NEWID()
GO


