-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
ALTER TABLE [Merchandising].[HierarchyTagCondition] ADD RepossessedConditionId [int] NOT NULL
ALTER TABLE [Merchandising].[HierarchyTagCondition] WITH CHECK  ADD CONSTRAINT [FK_HierarchyTagCondition_RepossessedCondition] FOREIGN KEY ([RepossessedConditionId]) REFERENCES [Merchandising].[RepossessedCondition]([Id])
ALTER TABLE [Merchandising].[HierarchyTagCondition] CHECK CONSTRAINT [FK_HierarchyTagCondition_RepossessedCondition]
ALTER TABLE [Merchandising].[HierarchyTagCondition] DROP COLUMN [Condition]

ALTER TABLE [Merchandising].[RepossessedCondition] ADD CONSTRAINT [UQ_RepossessedCondition_Name]  UNIQUE  (Name)
ALTER TABLE [Merchandising].[RepossessedCondition] ADD CONSTRAINT [UQ_RepossessedCondition_SkuSuffix] UNIQUE  (SkuSuffix)

