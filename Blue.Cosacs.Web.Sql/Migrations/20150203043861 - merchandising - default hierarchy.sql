-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
delete from Merchandising.ProductHierarchy
delete from Merchandising.PromotionHierarchy
delete from Merchandising.StockCountHierarchy
delete from Merchandising.HierarchyTagCondition
delete from merchandising.HierarchyLevel
delete from Merchandising.HierarchyTag

INSERT INTO Merchandising.HierarchyLevel Values('Division')
INSERT INTO Merchandising.HierarchyLevel Values('Department')
INSERT INTO Merchandising.HierarchyLevel Values('Class')