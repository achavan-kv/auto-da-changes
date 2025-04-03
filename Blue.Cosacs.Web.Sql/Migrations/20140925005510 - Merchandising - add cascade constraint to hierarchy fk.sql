-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here



ALTER TABLE [Merchandising].[HierarchyTagCondition] 
DROP CONSTRAINT [FK_HierarchyTagCondition_HierarchyTag]


ALTER TABLE [Merchandising].[HierarchyTagCondition]  
WITH CHECK ADD  CONSTRAINT [FK_HierarchyTagCondition_HierarchyTag] FOREIGN KEY([HierarchyTagId])
REFERENCES [Merchandising].[HierarchyTag] ([Id])
ON DELETE CASCADE ON UPDATE CASCADE

ALTER TABLE [Merchandising].[HierarchyTagCondition] 
CHECK CONSTRAINT [FK_HierarchyTagCondition_HierarchyTag]


