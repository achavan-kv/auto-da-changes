-- transaction: true
-- Change the previous line to false to disable running this whole migration in one transaction.
-- Removing that first line will default to 'true'.
-- 
-- Put your SQL code here
delete from Merchandising.ProductHierarchy

alter table Merchandising.ProductHierarchy
add HierarchyLevelId int not null

alter table Merchandising.ProductHierarchy
add constraint Merchandising_ProductHierarchy_HierarchyLevelId_FK foreign key (HierarchyLevelId)
references Merchandising.HierarchyLevel(Id)

alter table Merchandising.ProductHierarchy
add constraint UQ_Merchandising_ProductHierarchy_Product_Level unique (ProductId, HierarchyLevelId)
