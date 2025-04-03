IF EXISTS (
		SELECT 1
		FROM sysobjects
		WHERE id = object_id(N'[Merchandising].[CreateStockAdjustmentFromStockCount]')
			AND OBJECTPROPERTY(id, N'IsProcedure') = 1
		)
BEGIN
	DROP PROCEDURE [Merchandising].[CreateStockAdjustmentFromStockCount]
END
GO

CREATE PROCEDURE Merchandising.CreateStockAdjustmentFromStockCount
	-- ===============================================================================================
	-- Version:		<000> 
	-- ===============================================================================================
	--================================================================================
	-- Project            : Blue.Cosacs.Web
	-- Name               : Stock Count Close
	-- Author             : Ritesh Joge
	-- Create Date	      : 25 Aug 2020
	-- Description        : 
	--						CR Closing StockCount Product : Below script get the product from stockcount product as a input param
	--						Update the cost price and insert into stockadjustment product table.
	-- Change Control
	-- --------------
	-- Date           By        Description
	-- ----           --        -----------
	-- 25/08/20       RJ		Below script get the product from stockcount product as a input param. Update the cost price and insert into stockadjustment product table.
	--================================================================================
	@ProductStock
AS
Merchandising.StockCountProducts READONLY AS

BEGIN
	SET NOCOUNT ON;

	IF OBJECT_ID('tempdb..#tempProductStock') IS NOT NULL
		DROP TABLE #tempProductStock

	CREATE TABLE #tempProductStock (
		[Id] INT NULL
		,[StockAdjustmentId] INT NULL
		,[ProductId] INT NULL
		,[Quantity] INT NULL
		,[Comments] VARCHAR(MAX) NULL
		,[ReferenceNumber] VARCHAR(50) NULL
		,[AverageWeightedCost] MONEY NULL
		);

	INSERT INTO #tempProductStock
	SELECT [Id]
		,[StockAdjustmentId]
		,[ProductId]
		,[Quantity]
		,[Comments]
		,[ReferenceNumber]
		,[AverageWeightedCost]
	FROM @ProductStock;

	WITH StockCountSummary
	AS (
		SELECT [ProductId]
			,[AverageWeightedCost]
			,ROW_NUMBER() OVER (
				PARTITION BY [ProductId] ORDER BY [AverageWeightedCost] DESC
				) AS sk
		FROM [Merchandising].[CurrentCostPriceView] WITH (NOLOCK)
		WHERE [ProductId] IN (
				SELECT DISTINCT [ProductId]
				FROM #tempProductStock
				)
		)
	UPDATE tp
	SET tp.[AverageWeightedCost] = s.[AverageWeightedCost]
	FROM #tempProductStock tp
	INNER JOIN StockCountSummary s
		ON tp.[ProductId] = s.[ProductId]
	WHERE sk = 1;

	INSERT INTO [Merchandising].[StockAdjustmentProduct]
	SELECT [StockAdjustmentId]
		,[ProductId]
		,[Quantity]
		,[Comments]
		,[ReferenceNumber]
		,[AverageWeightedCost]
	FROM #tempProductStock
END