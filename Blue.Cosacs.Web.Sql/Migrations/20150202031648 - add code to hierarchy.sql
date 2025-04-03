-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE Merchandising.HierarchyTag ADD Code int
GO

UPDATE Merchandising.HierarchyTag SET Code = -1
ALTER TABLE Merchandising.HierarchyTag ALTER COLUMN Code int NOT NULL
INSERT INTO dbo.HiLo values('Merchandising.HierarchyTagCode', 1000, 1)