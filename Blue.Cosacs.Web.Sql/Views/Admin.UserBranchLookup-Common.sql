
GO

IF EXISTS (SELECT * FROM sys.objects so
		   INNER JOIN sys.schemas ss ON so.schema_id = ss.schema_id
		   WHERE so.type = 'V'
		   AND so.NAME = 'BranchLookup'
		   AND ss.name = 'Admin')
BEGIN
	DROP VIEW [Admin].[BranchLookup]
END

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ========================================================================
-- Version:		<001> 
-- ========================================================================

CREATE VIEW [Admin].[BranchLookup]
AS

	SELECT  b.branchno,
			BranchNameLong = b.branchname + ' ' + CONVERT(VARCHAR(10),b.branchno),
			b.branchname,
			b.StoreType,
			l.Id as BranchLocationId
	FROM	branch b
			LEFT JOIN Merchandising.Location l
			ON l.SalesId = b.branchno

GO

