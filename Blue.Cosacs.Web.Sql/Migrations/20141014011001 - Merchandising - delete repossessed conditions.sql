-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'HierarchyTagCondition')
DELETE FROM  [Merchandising].[HierarchyTagCondition]
GO

IF EXISTS (SELECT * FROM sys.tables WHERE name = 'RepossessedCondition')
DELETE FROM  [Merchandising].[RepossessedCondition]
GO