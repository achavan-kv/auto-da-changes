IF EXISTS (SELECT 'a' FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRCurrentStockItemsValueBranchView]'))
    DROP VIEW [Merchandising].[WTRCurrentStockItemsValueBranchView]
GO

CREATE VIEW [Merchandising].[WTRCurrentStockItemsValueBranchView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Division, Price, GrossProfit)
    AS
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), GETDATE(), 112)) AS DateKey
               , CAST(GETDATE() AS DATE) AS TransactionDate
               , l.Fascia
               , l.SalesId AS LocationId
               , l.Name AS LocationName
               , 'Company Stock Value' AS SaleType
               , 'Branch' AS Division
               , SUM(stLevel.StockOnHand * curCost.AverageWeightedCost) AS Price
               , 0 AS GrossProfit
        FROM  Merchandising.ProductStockLevel stLevel
        INNER JOIN Merchandising.CurrentCostPriceView curCost
            ON stLevel.ProductId = curCost.ProductId
        INNER JOIN Merchandising.Location l
            ON l.Id = stLevel.LocationId
        INNER JOIN Merchandising.Product p
            ON curCost.ProductId = p.Id
        WHERE p.[Status] != (SELECT Id FROM Merchandising.ProductStatus where Name = 'Non Active')
            AND p.Id NOT IN (SELECT DISTINCT p.Id
                             FROM Merchandising.Product p
                             INNER JOIN Merchandising.ProductStockLevel psl
                             ON p.Id = psl.ProductId
                             INNER JOIN Merchandising.ProductStatus ps
                             ON p.[Status] = ps.Id
                             WHERE ps.Name = 'Deleted'
                                 AND psl.StockOnHand = 0
                                 AND psl.StockOnOrder = 0
                                 AND psl.StockAvailable = 0)
        GROUP BY l.Fascia, l.SalesId, l.Name
    )

    SELECT ROW_NUMBER() OVER(ORDER BY DateKey) AS Id,
           data.*
    FROM (
            SELECT * FROM ExportData
            UNION ALL --Fascia
            SELECT DateKey, 
                   TransactionDate, 
                   Fascia, 
                   f.FasciaId AS LocationId, 
                   Fascia AS LocationName, 
                   SaleType AS SaleType,                    
                   Division AS Division, 
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData e
            INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] f on e.Fascia = f.FasciaName
            GROUP BY DateKey, 
                     TransactionDate, 
                     Fascia, 
                     f.FasciaId,
                     SaleType,
                     Division
            UNION ALL --Country
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName, 
                   SaleType AS SaleType,    
                   Division AS Division, 
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey, 
                     TransactionDate,
                     SaleType,
                     Division
        ) AS data
GO