
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[VE_ParentSKUMasterEOD]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[VE_ParentSKUMasterEOD]
END
GO

Create PROCEDURE [dbo].[VE_ParentSKUMasterEOD]
AS 
BEGIN
	SET NOCOUNT ON;
	
	Declare @TempEOD table 
  (ProductType char(1),
  ExternalItemNo varchar(30),
   Description varchar(max),
   UPC varchar(200),
   Model varchar(200),
   Brand varchar(200),
   VendorCost money,
   AverageWeightedCost money,
   LatestLandedCost money,
   ExternalProductID int,
   LastLandedCostUpdated datetime,
   Active int,
   Category varchar(200),
   ExternalTaxID varchar(200),
   ExternalVendorID varchar(200),
   ExternalCommissionID varchar(200),
   SpectacleLensStyle varchar(200),
   Features  varchar(max)
   )

   insert into @TempEOD
   (
   ProductType,
   ExternalItemNo ,
   Description ,
   UPC ,
   Model,
   Brand, 
   VendorCost ,
   AverageWeightedCost,
   LatestLandedCost,
   ExternalProductID ,
   LastLandedCostUpdated ,
   Active ,
   Category,
   ExternalTaxID,
   ExternalVendorID ,
   ExternalCommissionID ,
   SpectacleLensStyle,
   Features 
   )
SELECT DISTINCT
	CASE WHEN t.id in (70,72) THEN 'F' ELSE 
		CASE WHEN t.id in (57) THEN 'S' ELSE 
			CASE WHEN t.id in (33) THEN 'C' ELSE 'O' 
			END
		END
	END	AS ProductType,
		MProduct.SKU as ExternalItemNo,
		POSDescription as Description,
		isnull(MProduct.CorporateUPC, 0) as UPC ,
		 isnull(MProduct.VendorStyleLong, '') as Model,
		  (select (BrandCode + ' ' + BrandName) from Merchandising.Brand B where B.Id=MProduct.BrandId) AS Brand,
		max(MCostPrice.SupplierCost) as VendorCost,
		max(MCostPrice.AverageWeightedCost) as AverageWeightedCost,
	    max(MCostPrice.LastLandedCost) as LatestLandedCost,
		MProduct.Id as ExternalProductID,
		MCostPrice.LastLandedCostUpdated,
		CASE WHEN MProduct.Status IN (2,3,6,4,7) THEN 1 ELSE 0 END AS Active,
		(select distinct CASE WHEN t.id in (70,72) THEN CONCAT(0,t.code) 
			   WHEN t.id in (57) THEN  CONCAT(0,t.code)
			   WHEN t.id in (33) THEN CONCAT(0,t.code) 
			    WHEN t.id in (84,65) THEN CONCAT(0,t.code)
				else 'Optical'
			   end
			   from Merchandising.ProductHierarchyView where levelid =t.levelid and ProductType=MProduct.producttype)
			     as Category,
		 --CASE WHEN SInfo.Rate = 12.5 THEN '12' WHEN SInfo.Rate = 0 THEN '1' END as ExternalTaxID,
		 CASE WHEN SInfo.taxrate = (select taxrate from country) THEN '12' WHEN SInfo.taxrate = 0 THEN '1' END as ExternalTaxID,

		MS.Code as ExternalVendorID,
		0 as ExternalCommissionID,
		  SpectacleLensStyle = (select top 1 t.name from Merchandising.HierarchyTag t  INNER JOIN  Merchandising.ProductHierarchy h 
                on t.Id = h.HierarchyTagId
			   where t.LevelId=3 and h.ProductId=MProduct.Id)  
			  ,MProduct.Features
	 	
	  FROM  Merchandising.Product MProduct
			  --INNER JOIN  Merchandising.TaxRate SInfo ON MProduct.id = SInfo.ProductId
			   INNER JOIN  StockInfo SInfo ON MProduct.SKU = SInfo.SKU
              INNER JOIN  Merchandising.RetailPrice MRP ON MProduct.Id = MRP.ProductId
              INNER JOIN  Merchandising.CostPrice MCostPrice  ON MProduct.Id = MCostPrice.ProductId
              INNER JOIN  Merchandising.Supplier MS ON MProduct.PrimaryVendorId = MS.Id
              INNER JOIN Merchandising.ProductHierarchy h ON MProduct.Id = h.ProductId
              INNER JOIN  Merchandising.HierarchyTag t on t.Id = h.HierarchyTagId
              and MProduct.id in 
	 (select DISTINCT ProductId from merchandising.CostPrice where ProductId IN (select DISTINCT id from Merchandising.product where id in
(select ProductId from [Merchandising].[ProductHierarchyView] where tagid ='17' and levelid ='1') and ProductType='RegularStock'
 and status in (2,3,6,4,7) and SKUStatus='A' ) )
	and MCostPrice.LastLandedCostUpdated=(select top 1 MCostPrice.LastLandedCostUpdated FROM Merchandising.CostPrice MCostPrice 
	where MProduct.Id = MCostPrice.ProductId  order by MCostPrice.LastLandedCostUpdated desc)
 AND ISNULL(CAST(MProduct.LastUpdatedDate as date),CAST(MProduct.CreatedDate as date)) = CAST((GETDATE()-1) as date)
   and  t.levelid=2
       group by MProduct.SKU ,POSDescription,
	   MProduct.CorporateUPC,MProduct.Id,t.id ,MProduct.Status,
	   MCostPrice.LastLandedCostUpdated, MS.Code,MProduct.VendorStyleLong,BrandId,taxrate,t.LevelId,MProduct.ProductType,t.code
	   , MProduct.Features

	select * from @TempEOD
SELECT DISTINCT 
			MRP.Regularprice as Retail, 
			MRP.lastupdated ,
			l.SalesID AS BranchNo,
			PSL.StockOnHand AS Quantity,
			MRP.ProductId as ProductId,
			MS.Code as ExternalVendorID
		FROM  Merchandising.RetailPrice MRP
		INNER JOIN [Merchandising].[ProductStockLevel] PSL ON PSL.ProductId =  MRP.Id
		INNER JOIN [Merchandising].[Location] l ON psl.LocationId = l.Id
		INNER JOIN Merchandising.Product MProduct ON MProduct.Id  = MRP.ProductId 
		INNER JOIN  Merchandising.Supplier MS ON MProduct.PrimaryVendorId = MS.Id
		and l.SalesId in (select branchno from branch b ,merchandising.location l where b.branchno=l.SalesId and Fascia='Courts Optical')
	    and MProduct.Id in( select ExternalProductID from @TempEOD)
		and (CAST(MRp.lastupdated as date)) = CAST((GETDATE()-1) as date)
END