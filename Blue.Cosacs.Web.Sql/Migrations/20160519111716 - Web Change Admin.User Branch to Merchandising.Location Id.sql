-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here

IF NOT EXISTS(SELECT 1 FROM sys.columns c where c.name = 'BranchId' AND  c.object_id = OBJECT_ID('[Admin].[User]'))
	ALTER TABLE Admin.[User]
	ADD BranchId INT NULL
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns c where c.name = 'BranchId' AND  c.object_id = OBJECT_ID('[Admin].[User]'))
	UPDATE u
	SET BranchNo = ISNULL(l.SalesId, 0)
	FROM Admin.[User] u
	INNER JOIN Merchandising.Location l on u.BranchNo = l.Id

IF EXISTS(SELECT TOP 1 'A' FROM Merchandising.Location)
	UPDATE u
	SET BranchId = ISNULL(l.Id, 0)
	FROM Admin.[User] u
	INNER JOIN Merchandising.Location l on u.BranchNo = l.SalesId
ELSE 
	--v9 has no data on this table therefore we can not do tis part 
	--so we just set it to a non null value
	UPDATE Admin.[User]
	SET BranchId = 0
	
ALTER TABLE Admin.[User]
ALTER COLUMN BranchId INT NOT NULL
