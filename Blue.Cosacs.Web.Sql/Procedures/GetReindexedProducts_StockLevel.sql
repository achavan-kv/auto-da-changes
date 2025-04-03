IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE  Table_Name = 'Product' AND  Column_Name = 'PreviousProductType'
           AND TABLE_SCHEMA = 'Merchandising')
BEGIN
	ALTER TABLE Merchandising.Product ADD PreviousProductType nvarchar(50) NULL
END


IF EXISTS (
		SELECT *
		FROM dbo.sysobjects
		WHERE id = OBJECT_ID('[Merchandising].[GetReindexedProducts_StockLevel]')
			AND OBJECTPROPERTY(id, 'IsProcedure') = 1
		)
	DROP PROCEDURE [Merchandising].[GetReindexedProducts_StockLevel]
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [Merchandising].[GetReindexedProducts_StockLevel] 
@INDEX_UPDATED_DATA_ONLY BIT
AS
BEGIN
	SET NOCOUNT ON

	----------------------------------------------------------------------------------------------
	---ALL DATA MOVE INTO TEMP TBALE
	SELECT product.Id AS ProductId
		,SKU
		,PreviousProductType
		,LongDescription
		,ProductType
		,product.Tags
		,StoreTypes
		,
		--primaryVendor.Name as Vendors,
		--Vendor.Suppliers,
		Vendors = REPLACE(SUPPLIERS, ']', ',' + '"' + PRIMARYVENDOR.NAME + '"' + ']')
		,product.CreatedDate
		,[status].name AS ProductStatus
		,convert(VARCHAR, l.LocationId) LocationNumber
		,[Stock].name AS LocationName
		,[Stock].Fascia
		,[Stock].Warehouse
		,l.VirtualWarehouse
		,l.Id LocationId
		,condition.Condition AS RepossessedCondition
		,stock.StockAvailable AS StockAvailable
		,stock.StockOnHand AS StockOnHand
		,stock.StockOnOrder AS StockOnOrder
		,stock.StockAllocated AS StockAllocated
		,ISNULL(c.AverageWeightedCost, 0) AS AverageWeightedCost
	INTO #TEMP
	FROM merchandising.product product
	INNER JOIN merchandising.ProductStatus [status] ON product.[Status] = [status].id
		AND product.[Status] != '5' --index optimize
		AND product.[Status] != '1' --index optimize 
		--and product.Id in(1)
	LEFT OUTER JOIN Merchandising.[ProductSupplierConcatView] [Vendor] ON [Vendor].ProductId = Product.id
	JOIN Merchandising.[LocationStockLevelView] [Stock] ON [Stock].ProductId = Product.id
		AND stock.locationid IS NOT NULL
	LEFT OUTER JOIN Merchandising.Combo combo ON combo.Id = product.Id
	LEFT OUTER JOIN Merchandising.[SetProduct] [SET] ON [Set].Id = product.Id
	LEFT OUTER JOIN [Merchandising].[RepossessedProductConditionView] condition ON condition.productid = product.id
	LEFT OUTER JOIN Merchandising.Supplier primaryVendor ON primaryVendor.Id = product.PrimaryVendorId
	LEFT OUTER JOIN Merchandising.Location l ON l.Id = stock.LocationId
		AND stock.LocationId NOT LIKE '%CLOSE%'
	LEFT OUTER JOIN Merchandising.CurrentCostPriceView c ON stock.ProductId = c.ProductId
	WHERE (
			PRODUCT.LASTUPDATEDDATE >= GETDATE() - (
				SELECT CAST(ValueString AS INT)
				FROM [Config].[Setting]
				WHERE ID = 'ScheduleJobUpdateDaySetting'
				)
			OR @INDEX_UPDATED_DATA_ONLY = 0
			)
		--------------------------------------------------------------------------------------------- merchandising.GetLocationLevelStock
		---SELECT TOP 1 RECORD OF EACH PRODUCT FROM TEMP AND MOVE INTO PRODUCT TABLE
		;

	WITH cte
	AS (
		SELECT *
			,ROW_NUMBER() OVER (
				PARTITION BY ProductId ORDER BY CreatedDate DESC
				) AS rn
		FROM #TEMP
		)
	SELECT *
	INTO #Product
	FROM cte
	WHERE rn = 1

	----------------------------------------------------------------------------------------------
	---SELECT TEMP_SummariseAll
	--drop table #TEMP_SummariseAll
	SELECT ProductId
		,'All Locations' AS LocationName
		,0 AS LocationId
		,'ALL' AS LocationNumber
		,'ALL' AS fascia
		,1 AS Warehouse
		,(sum(StockAvailable)) AS StockAvailable
		,(sum(StockOnHand)) AS StockOnHand
		,(sum(StockOnOrder)) AS StockOnOrder
		,(sum(StockAllocated)) AS StockAllocated
	INTO #TEMP_SummariseAll
	FROM #TEMP A
	WHERE VirtualWarehouse = 0
	GROUP BY PRODUCTID

	-----------------------------------
	--drop table #TEMP_SummariseAllWarehouses
	SELECT ProductId
		,'All Warehouses' AS LocationName
		,0 AS LocationId
		,'ALL' AS LocationNumber
		,a.fascia
		,(sum(StockAvailable)) AS StockAvailable
		,(sum(StockOnHand)) AS StockOnHand
		,(sum(StockOnOrder)) AS StockOnOrder
		,(sum(StockAllocated)) AS StockAllocated
	INTO #TEMP_SummariseAllWarehouses
	FROM #TEMP A
	WHERE Warehouse = 1
	GROUP BY PRODUCTID
		,fascia

	--select * from #TEMP_SummariseAllWarehouses
	---select  * from #temp where Warehouse=1
	------------------------------------------------------------------------
	--drop table #TEMP_SummariseFascias
	SELECT ProductId
		,Fascia + ' Locations' AS LocationName
		,0 AS LocationId
		,Fascia AS LocationNumber
		,1 AS Warehouse
		,(sum(StockAvailable)) AS StockAvailable
		,(sum(StockOnHand)) AS StockOnHand
		,(sum(StockOnOrder)) AS StockOnOrder
		,(sum(StockAllocated)) AS StockAllocated
	INTO #TEMP_SummariseFascias
	FROM #TEMP A
	WHERE VirtualWarehouse = 0
	GROUP BY PRODUCTID
		,Fascia

	-----------------------------------------------
	--drop table #TEMP_SummariseFasciaWarehouses
	SELECT ProductId
		,Fascia + ' Warehouses' AS LocationName
		,0 AS LocationId
		,Fascia AS LocationNumber
		,(sum(StockAvailable)) AS StockAvailable
		,(sum(StockOnHand)) AS StockOnHand
		,(sum(StockOnOrder)) AS StockOnOrder
		,(sum(StockAllocated)) AS StockAllocated
	INTO #TEMP_SummariseFasciaWarehouses
	FROM #TEMP A
	WHERE Warehouse = 1
	GROUP BY PRODUCTID
		,Fascia

	-----------------------------------------------
	SELECT t1.ProductId
		,t1.SKU
		,t1.PreviousProductType
		,t1.LongDescription
		,t1.ProductType
		,t1.Tags
		,t1.StoreTypes
		,t1.Vendors
		,
		--t1.Suppliers,
		t1.CreatedDate
		,t1.ProductStatus
		,t1.LocationNumber
		,t1.LocationName
		,t1.Fascia
		,CASE t1.Warehouse
			WHEN 1
				THEN 'YES'
			ELSE 'NO'
			END AS Warehouse
		,t1.VirtualWarehouse
		,t1.LocationId
		,t1.RepossessedCondition
		,t1.StockAvailable
		,t1.StockOnHand
		,t1.StockOnOrder
		,t1.StockAllocated
		,t1.AverageWeightedCost
		,'MerchandiseStock' AS Type
	FROM #temp t1
	
	UNION
	
	SELECT p.ProductId
		,p.SKU
		,P.PreviousProductType
		,p.LongDescription
		,p.ProductType
		,p.Tags
		,p.StoreTypes
		,p.Vendors
		,
		-- p.Suppliers,
		p.CreatedDate
		,p.ProductStatus
		,t.LocationNumber
		,t.LocationName
		,t.Fascia
		,CASE T.Warehouse
			WHEN 1
				THEN 'YES'
			ELSE 'NO'
			END AS Warehouse
		,p.VirtualWarehouse
		,t.LocationId
		,p.RepossessedCondition
		,t.StockAvailable
		,t.StockOnHand
		,t.StockOnOrder
		,t.StockAllocated
		,p.AverageWeightedCost
		,'MerchandiseStock' AS Type
	FROM #TEMP_SummariseAll t
	INNER JOIN #Product P ON P.ProductId = t.ProductId
	
	UNION
	
	---------------------------------------------------------------------------------
	SELECT p.ProductId
		,p.SKU
		,P.PreviousProductType
		,p.LongDescription
		,p.ProductType
		,p.Tags
		,p.StoreTypes
		,p.Vendors
		,
		--p.Suppliers,
		p.CreatedDate
		,p.ProductStatus
		,t.LocationNumber
		,t.LocationName
		,t.Fascia
		,CASE p.Warehouse
			WHEN 1
				THEN 'YES'
			ELSE 'NO'
			END AS Warehouse
		,p.VirtualWarehouse
		,t.LocationId
		,p.RepossessedCondition
		,t.StockAvailable
		,t.StockOnHand
		,t.StockOnOrder
		,t.StockAllocated
		,p.AverageWeightedCost
		,'MerchandiseStock' AS Type
	FROM #TEMP_SummariseAllWarehouses t
	INNER JOIN #Product P ON P.ProductId = t.ProductId
	
	UNION
	
	----------------------------------------------------------------------------------------------
	SELECT p.ProductId
		,p.SKU
		,P.PreviousProductType
		,p.LongDescription
		,p.ProductType
		,p.Tags
		,p.StoreTypes
		,p.Vendors
		,
		-- p.Suppliers,
		p.CreatedDate
		,p.ProductStatus
		,t.LocationNumber
		,t.LocationName
		,t.LocationNumber AS Fascia
		,CASE T.Warehouse
			WHEN 1
				THEN 'YES'
			ELSE 'NO'
			END AS Warehouse
		,p.VirtualWarehouse
		,t.LocationId
		,p.RepossessedCondition
		,t.StockAvailable
		,t.StockOnHand
		,t.StockOnOrder
		,t.StockAllocated
		,p.AverageWeightedCost
		,'MerchandiseStock' AS Type
	FROM #TEMP_SummariseFascias t
	INNER JOIN #Product P ON P.ProductId = t.ProductId
	
	UNION
	
	-----------------------------------------------------------------------------
	SELECT p.ProductId
		,p.SKU
		,P.PreviousProductType
		,p.LongDescription
		,p.ProductType
		,p.Tags
		,p.StoreTypes
		,p.Vendors
		,
		-- p.Suppliers,
		p.CreatedDate
		,p.ProductStatus
		,t.LocationNumber
		,t.LocationName
		,t.LocationNumber AS Fascia
		,CASE p.Warehouse
			WHEN 1
				THEN 'YES'
			ELSE 'NO'
			END AS Warehouse
		,p.VirtualWarehouse
		,t.LocationId
		,p.RepossessedCondition
		,t.StockAvailable
		,t.StockOnHand
		,t.StockOnOrder
		,t.StockAllocated
		,p.AverageWeightedCost
		,'MerchandiseStock' AS Type
	FROM #TEMP_SummariseFasciaWarehouses t
	INNER JOIN #Product P ON P.ProductId = t.ProductId
	ORDER BY ProductId

	DROP TABLE #TEMP

	DROP TABLE #TEMP_SummariseAll

	DROP TABLE #TEMP_SummariseAllWarehouses

	DROP TABLE #TEMP_SummariseFascias

	DROP TABLE #TEMP_SummariseFasciaWarehouses
END
GO

