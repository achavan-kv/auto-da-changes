if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_ParentSKUMaster]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_ParentSKUMaster]
END
GO

Create PROCEDURE [dbo].[VE_ParentSKUMaster]
AS
BEGIN
       SET NOCOUNT ON;
	   SELECT distinct
       CASE WHEN t.id in (70,72) THEN 'F' ELSE
              CASE WHEN t.id in (57) THEN 'S' ELSE
                   CASE WHEN t.id in (33) THEN 'C' ELSE 
				   CASE WHEN t.id in (84,65) THEN 'O'
                     END 
				 END
              END
       END    AS ProductType,
         MProduct.SKU as ExternalItemNo,
              POSDescription as Description,
              isnull(MProduct.CorporateUPC, 0) as UPC ,
			  isnull(MProduct.VendorStyleLong, '') as Model,
			 (select (BrandCode + ' ' + BrandName) from Merchandising.Brand B where B.Id=MProduct.BrandId) AS Brand,
              MProduct.Id as ExternalProductID,
					
				(SELECT max(MCostPrice.SupplierCost) From Merchandising.CostPrice MCostPrice INNER JOIN Merchandising.Product MProduct ON MProduct.Id = MCostPrice.ProductId where MProduct.id  in (select DISTINCT ProductId from merchandising.CostPrice where ProductId IN (select DISTINCT id from Merchandising.product where id in (select ProductId from [Merchandising].[ProductHierarchyView] where tagid ='17' and levelid ='1') and ProductType='RegularStock' and status in (2,3,6,4,7) and SKUStatus='A' ) ) and MCostPrice.AverageWeightedCostUpdated=(select top 1 MCostPrice.AverageWeightedCostUpdated FROM Merchandising.CostPrice MCostPrice where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.AverageWeightedCostUpdated desc) and MCostPrice.LastLandedCostUpdated=(select top 1 MCostPrice.LastLandedCostUpdated FROM Merchandising.CostPrice MCostPrice where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.LastLandedCostUpdated desc)) as VendorCost ,
				(select max( MCostPrice.AverageWeightedCost) From Merchandising.CostPrice MCostPrice INNER JOIN Merchandising.Product MProduct ON MProduct.Id = MCostPrice.ProductId where MProduct.id  in (select DISTINCT ProductId from merchandising.CostPrice where ProductId IN (select DISTINCT id from Merchandising.product where id in (select ProductId from [Merchandising].[ProductHierarchyView] where tagid ='17' and levelid ='1') and ProductType='RegularStock' and status in (2,3,6,4,7) and SKUStatus='A' ) ) and MCostPrice.AverageWeightedCostUpdated=(select top 1 MCostPrice.AverageWeightedCostUpdated FROM Merchandising.CostPrice MCostPrice where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.AverageWeightedCostUpdated desc) and MCostPrice.LastLandedCostUpdated=(select top 1 MCostPrice.LastLandedCostUpdated FROM Merchandising.CostPrice MCostPrice where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.LastLandedCostUpdated desc) )as AverageWeightedCost ,
				(select max(MCostPrice.LastLandedCost) From Merchandising.CostPrice MCostPrice INNER JOIN Merchandising.Product MProduct ON MProduct.Id = MCostPrice.ProductId where MProduct.id  in (select DISTINCT ProductId from merchandising.CostPrice where ProductId IN (select DISTINCT id from Merchandising.product where id in (select ProductId from [Merchandising].[ProductHierarchyView] where tagid ='17' and levelid ='1') and ProductType='RegularStock' and status in (2,3,6,4,7) and SKUStatus='A' ) ) and MCostPrice.AverageWeightedCostUpdated=(select top 1 MCostPrice.AverageWeightedCostUpdated FROM Merchandising.CostPrice MCostPrice where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.AverageWeightedCostUpdated desc) and MCostPrice.LastLandedCostUpdated=(select top 1 MCostPrice.LastLandedCostUpdated FROM Merchandising.CostPrice MCostPrice where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.LastLandedCostUpdated desc)) as LatestLandedCost,
			  --max(MCostPrice.SupplierCost) as VendorCost,
			  --max(MCostPrice.AverageWeightedCost) as AverageWeightedCost,
			  --max(MCostPrice.LastLandedCost) as LatestLandedCost,
     --         MCostPrice.LastLandedCostUpdated,
              CASE WHEN MProduct.Status IN (2,3,6,4,7) THEN 1 ELSE 0 END AS Active,
			   (select distinct CASE WHEN t.id in (70,72) THEN CONCAT(0,t.code) 
			   WHEN t.id in (57) THEN  CONCAT(0,t.code)
			   WHEN t.id in (33) THEN CONCAT(0,t.code)
			    WHEN t.id in (84,65) THEN CONCAT(0,t.code)
				else 'Optical'
			   end
			   from Merchandising.ProductHierarchyView where levelid =t.levelid and ProductType=MProduct.producttype)
			     as Category,
			 --CASE WHEN SInfo.taxrate = 12.5 THEN '12' WHEN SInfo.taxrate = 0 THEN '1' END as ExternalTaxID,
			 CASE WHEN SInfo.taxrate = (select taxrate from country) THEN '12' WHEN SInfo.taxrate = 0 THEN '1' END as ExternalTaxID,
			  -- CASE WHEN SInfo.taxrate = 16.5 THEN '12' WHEN SInfo.taxrate = 0 THEN '1' END as ExternalTaxID,
              MS.Code as ExternalVendorID,
              0 as ExternalCommissionID,
			  SpectacleLensStyle= (select top 1 t.name from Merchandising.HierarchyTag t  INNER JOIN  Merchandising.ProductHierarchy h 
                on t.Id = h.HierarchyTagId
			   where t.LevelId=3 and h.ProductId=MProduct.Id)  ,
			  MProduct.Features 
			  
			--select distinct *  
			  FROM  Merchandising.Product MProduct
              INNER JOIN  StockInfo SInfo ON MProduct.SKU = SInfo.SKU
			  INNER JOIN  Merchandising.ProductHierarchyView PHV ON PHV.ProductId= MProduct.id
              INNER JOIN  Merchandising.RetailPrice MRP ON MProduct.Id = MRP.ProductId
              INNER JOIN  Merchandising.CostPrice MCostPrice  ON MProduct.Id = MCostPrice.ProductId
              INNER JOIN  Merchandising.Supplier MS ON MProduct.PrimaryVendorId = MS.Id
              INNER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
              INNER JOIN  Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId
              and MProduct.id  in
					   (select DISTINCT ProductId from merchandising.CostPrice where ProductId IN (select DISTINCT id from Merchandising.product where id in
				(select ProductId from [Merchandising].[ProductHierarchyView] where tagid ='17' and levelid ='1') and ProductType='RegularStock'
				 and status in (2,3,6,4,7) and SKUStatus='A' ) )
       and MCostPrice.AverageWeightedCostUpdated=(select top 1 MCostPrice.AverageWeightedCostUpdated FROM Merchandising.CostPrice MCostPrice
       where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.AverageWeightedCostUpdated desc)
	   and MCostPrice.LastLandedCostUpdated=(select top 1 MCostPrice.LastLandedCostUpdated FROM Merchandising.CostPrice MCostPrice
       where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.LastLandedCostUpdated desc)
	   and  t.levelid=2

			  SELECT  
                     MRP.Regularprice as Retail,
                     l.SalesID AS BranchNo,
                    PSL.StockOnHand AS Quantity,
                     MRP.ProductId as ProductId,
                     MS.Code as ExternalVendorID
              FROM  Merchandising.RetailPrice MRP
			   INNER JOIN Merchandising.Product MProduct ON MProduct.Id  = MRP.ProductId 
			   and mrp.lastupdated=
			   (select top 1 lastupdated from Merchandising.RetailPrice where productid = MRP.productid order by lastupdated desc)
              INNER JOIN [Merchandising].[ProductStockLevel] PSL ON PSL.ProductId =  MProduct.Id
              INNER JOIN [Merchandising].[Location] l ON psl.LocationId = l.Id
              INNER JOIN  Merchandising.Supplier MS ON MProduct.PrimaryVendorId = MS.Id
			   and MProduct.id  in
       (select DISTINCT ProductId from merchandising.RetailPrice where ProductId IN (select DISTINCT id from Merchandising.product where id in
(select ProductId from [Merchandising].[ProductHierarchyView] where tagid ='17' and levelid ='1') and ProductType='RegularStock'
 and status in (2,3,6,4,7) and SKUStatus='A' ) ) 
 and l.SalesId in (select branchno from branch b ,merchandising.location l where b.branchno=l.SalesId and Fascia='Courts Optical')

order by MProduct.Id asc
END