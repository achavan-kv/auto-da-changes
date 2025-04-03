IF OBJECT_ID('Merchandising.StockCountStart') IS NOT NULL
	DROP PROCEDURE Merchandising.StockCountStart
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author		: Nilesh
-- Create date	: 24-Jan-2020
-- Version		: 1.1
-- Description	: This stored procedure is used to get the product for stock count start. The date will visible on Stock Count  page.
--				  * Inside this we are not considering product -
-- 				  *  With status 'Deleted' 
--                *  Product have no cost and no hierarchy.
-- =============================================
CREATE PROCEDURE [Merchandising].[StockCountStart]  --184,99999 
	@StockCountId INT
	,@StartedById INT
AS

SET NOCOUNT ON;

DECLARE @StockCountLocationId INT
	,@CreatedDate DATETIME

SET @CreatedDate = GETDATE()

SELECT @StockCountLocationId = Locationid
FROM merchandising.StockCount
WHERE id = @StockCountId

CREATE TABLE #Product_StockCount (
	StockCountId INT
	,ProductId INT
	,StartStockOnHand INT
	,[Count] INT
	,SystemAdjustment INT
	,VerifiedById INT
	,CreatedDate DATETIME
	,Hierarchy VARCHAR(300)
	)

BEGIN TRY
	IF EXISTS (
			SELECT 1
			FROM merchandising.StockCountHierarchyView
			WHERE StockCountId = @StockCountId
				AND HierarchyTagId IS NOT NULL
			)
	BEGIN
		INSERT INTO #Product_StockCount (
			StockCountId
			,ProductId
			,StartStockOnHand
			,[Count]
			,SystemAdjustment
			,VerifiedById
			,CreatedDate
			,Hierarchy
			)
		SELECT DISTINCT @StockCountId
			,psl.ProductId
			,psl.StockOnHand
			,0
			,0
			,0
			,CreatedDate = @CreatedDate
			,Hierarchy
		FROM merchandising.ProductStockLocationView psl
		INNER JOIN Merchandising.product p ON psl.ProductId = p.id
		AND locationid = @StockCountLocationId 
		AND P.[Status] <> 5 and p.SKUStatus <> 'D'
		INNER  JOIN Merchandising.CostPrice cp ON P.ID =cp.ProductId
		INNER JOIN (
			SELECT ht.name AS Hierarchy
				,ph.productid
			FROM merchandising.hierarchytag ht
			INNER JOIN Merchandising.ProductHierarchy ph ON ht.id = ph.HierarchyTagid
			INNER JOIN (
				SELECT TOP 1 hierarchytagid
					,HierarchyLevelId
				FROM merchandising.StockCountHierarchyView shv
				WHERE stockcountid = @StockCountId
					AND HierarchyTagId IS NOT NULL
				ORDER BY HierarchyLevelId DESC
				) b ON ht.id = b.hierarchytagid
				AND ph.HierarchyLevelId = b.HierarchyLevelId
			) a ON psl.productid = a.productid
		WHERE 
			p.ProductType IN (
				'RegularStock'
				,'SparePart'
				,'RepossessedStock'
				)
	END
	ELSE
	BEGIN
		INSERT INTO #Product_StockCount (
			StockCountId
			,ProductId
			,StartStockOnHand
			,[Count]
			,SystemAdjustment
			,VerifiedById
			,CreatedDate
			,Hierarchy
			)
		SELECT DISTINCT @StockCountId
			,psl.ProductId
			,psl.StockOnHand
			,0
			,0
			,0
			,CreatedDate = @CreatedDate
			,(
				SELECT TOP 1 name
				FROM merchandising.hierarchytag ht
				INNER JOIN Merchandising.ProductHierarchy ph ON ht.id = ph.HierarchyTagid
					AND psl.ProductId = ph.productid
				ORDER BY ht.levelid DESC
				) AS Hierarchy
		FROM merchandising.ProductStockLocationView psl
		INNER JOIN Merchandising.product p ON psl.ProductId = p.id
		AND locationid = @StockCountLocationId 
		AND P.[Status] <> 5 and p.SKUStatus <> 'D'
		INNER  JOIN Merchandising.CostPrice cp ON P.ID =cp.ProductId
		WHERE 
			p.ProductType IN (
				'RegularStock'
				,'SparePart'
				,'RepossessedStock'
				)
	END
	BEGIN TRANSACTION

	
	INSERT INTO Merchandising.StockCountProduct (
		StockCountId
		,ProductId
		,StartStockOnHand
		,[Count]
		,SystemAdjustment
		,VerifiedById
		,CreatedDate
		,Hierarchy
		)
	SELECT  StockCountId
		,ProductId
		,StartStockOnHand
		,Count
		,SystemAdjustment
		,VerifiedById
		,CreatedDate
		,Hierarchy
	FROM #Product_StockCount
	WHERE Hierarchy IS NOT NULL

	DECLARE @result_ProductIds VARCHAR(MAX)

	SELECT @result_ProductIds = COALESCE(@result_ProductIds + ',', '') + CONVERT(VARCHAR(12), ProductId)
	FROM #Product_StockCount

	UPDATE merchandising.StockCount
	SET StartedDate = @CreatedDate
		,StartedById = @StartedById
	WHERE id = @StockCountId

	SELECT @result_ProductIds AS ProductIds	

	DROP TABLE #Product_StockCount
	COMMIT TRANSACTION 

END TRY

BEGIN CATCH
	ROLLBACK TRANSACTION
END CATCH
