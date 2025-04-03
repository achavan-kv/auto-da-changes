
GO

IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[BuyerSalesHistoryReportView]'))
BEGIN
	DROP VIEW [Merchandising].[BuyerSalesHistoryReportView]
END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

/***********************************************************************************************************

-- File Name    : [Merchandising].[BuyerSalesHistoryReportView]
-- Version		: 001

-- Change Control
-- --------------
-- Date			By					Description
-- ----			--					-----------
-- 23 Feb 2020	Nilesh Choubisa		#6877781 - Replace # character with blank character from LongDescription 
--                                  to fix BuyerSalesHistory export data cutting after # character.
************************************************************************************************************/

CREATE VIEW [Merchandising].[BuyerSalesHistoryReportView]
AS

WITH AWC 
AS
(
	SELECT ProductId, AverageWeightedCost, ROW_NUMBER() OVER (PARTITION BY productId ORDER BY averageweightedCostUpdated DESC, id DESC) [Row]
	FROM Merchandising.CostPrice
)
SELECT 
	ROW_NUMBER() OVER( ORDER BY Product.Id, years.NumericYear ) AS Id,  
	product.Id AS ProductId, 
	product.Sku, 
	REPLACE(REPLACE(REPLACE(replace(product.LongDescription,'#',''), CHAR(10), ''), CHAR(13), ''),CHAR(9), ' ') AS LongDescription, 
	product.ProductType, 
	brand.BrandName, 
	supplier.Name AS Vendor,
	location.Id AS LocationId, 
	location.Name AS LocationName, 
	location.Fascia, 
	location.Warehouse,
	stock.StockOnHand, 
	stock.StockOnOrder,
	cost.AverageWeightedCost, 
	price.CashPrice, 
	price.taxrate, 
	years.[Year], 
	years.NumericYear, 
	years.StartDate, 
	years.EndDate,PreviousProductType
FROM Merchandising.CurrentStockPriceByLocationView price
CROSS JOIN Merchandising.BuyerSalesHistoryYearView years
INNER JOIN Merchandising.Location location 
	ON location.id = price.LocationId
INNER JOIN Merchandising.Product product 
	ON price.productId = product.id 
	AND (NULLIF(NULLIF(StoreTypes, '[]'), '') IS NULL 
	OR StoreTypes LIKE '%' + location.StoreType + '%')
INNER JOIN Merchandising.ProductStatus status 
	ON status.Id = product.Status
INNER JOIN Merchandising.Supplier supplier 
	ON supplier.Id = product.primaryvendorId
INNER JOIN AWC cost ON cost.ProductId = product.Id AND [Row] = 1
LEFT JOIN Merchandising.ProductStockLevel stock 
	ON stock.LocationId = location.id 
	AND stock.ProductId = product.Id
LEFT JOIN merchandising.Brand brand 
	ON brand.Id = product.BrandId
WHERE status.Name NOT IN ('Inactive', 'Deleted')



GO