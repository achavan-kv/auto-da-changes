--
-- Script Comment : Product Warrantable Flag Management
-- Script Name : Product_Warrantable_Flag_Management_Script.sql
-- Created For	: 10.6 Image Release
-- Created By	: Charudatt
-- Created On	: 13/03/2020

SET XACT_ABORT ON
Go
SET IMPLICIT_TRANSACTIONS OFF
Go
	IF EXISTS (SELECT ScriptName FROM DataFix 
			   WHERE ScriptName like 'Product_Warrantable_Flag_Management_Script' AND version = 1)
	BEGIN
		PRINT 'This script has already been run and can not be run twice. Please contact CoSACS Support Centre'
	END 
	ELSE 
		BEGIN
			Begin Tran
			Update p Set p.Warrantable = null From Merchandising.Product p
			--Where p.Warrantable is not null

			;With cteProductHierarchy (Id,sku,Division,Department,Class) As
			(
			Select distinct p.ID,p.sku,ph1.HierarchyTagId As Division
			,ph2.HierarchyTagId As Department
			,ph3.HierarchyTagId As Class
			 FROM Merchandising.Product p
			INNER  Join Merchandising.ProductHierarchy ph1
			ON p.Id = ph1.ProductId AND ph1.HierarchyLevelId =1 
			INNER Join Merchandising.ProductHierarchy ph2
			ON p.Id = ph2.ProductId AND ph2.HierarchyLevelId =2
			INNER Join Merchandising.ProductHierarchy ph3
			ON p.Id = ph3.ProductId AND ph3.HierarchyLevelId =3
			),
			cteProduct (Id,SKU) As
			(
			Select distinct ph.id,ph.sku
			From cteProductHierarchy ph 
			INNER JOIN Warranty.LinkProduct lp
			ON Cast(ph.Division As varchar) = lp.Level_1
			AND Cast(ph.Department As varchar) =lp.Level_2
			AND cast( ph.Class As varchar) = lp.Level_3 
			Union all
			Select p.id,p.SKU From Merchandising.Product p
			Inner Join Warranty.LinkProduct lp On p.sku=lp.ItemNumber
			)
			Update p Set p.Warrantable = 1  From Merchandising.Product p
			Inner Join cteProduct cp On p.id=cp.id
			Where p.Warrantable is null


			Update p Set p.Warrantable = 0 From Merchandising.Product p
			Where p.Warrantable is null

			
			INSERT INTO Datafix
			select getdate(),  'Product_Warrantable_Flag_Management_Script.sql', 'Product Warrantable Flag Management', 'Charudatt, Zensar', 1
			Commit Tran
		END


