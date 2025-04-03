IF EXISTS (SELECT 'a' FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRCurrentStockItemsValueView]'))
    DROP VIEW [Merchandising].[WTRCurrentStockItemsValueView]
GO

CREATE VIEW [Merchandising].[WTRCurrentStockItemsValueView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Division, DepartmentCode, Department, ClassCode, Class, Price, GrossProfit)
    AS
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), GETDATE(), 112)) AS DateKey
               , CAST(GETDATE() AS DATE) AS TransactionDate
               , l.Fascia
               , l.SalesId AS LocationId
               , l.Name AS LocationName
               , 'Stock Value' AS SaleType
               , ISNULL(division.Tag, '(Unknown Division)') AS Division
               , ISNULL(Department.Code, '0') AS DepartmentCode
               , ISNULL(Department.Tag, '(Unknown Department)') AS Department
               , ISNULL(Class.Code, '0') AS ClassCode
               , ISNULL(Class.Tag, '(Unknown Class)') AS Class
               , SUM(ISNULL(stLevel.StockOnHand, 0) * ISNULL(curCost.AverageWeightedCost, 0)) AS Price
               , 0 AS GrossProfit
        FROM Merchandising.ProductStockLevel stLevel
        INNER JOIN Merchandising.CurrentCostPriceView curCost
            ON stLevel.ProductId = curCost.ProductId
        INNER JOIN Merchandising.Location l
            ON l.Id = stLevel.LocationId
        INNER JOIN Merchandising.Product p
            ON stLevel.ProductId = p.Id
        LEFT OUTER JOIN Merchandising.ProductHierarchyView AS division 
	        ON division.ProductId = stLevel.ProductId 
	        AND division.[Level] = 'Division' 
        LEFT OUTER JOIN  Merchandising.ProductHierarchyView AS Department 
	        ON Department.ProductId = stLevel.ProductId
	        AND Department.[Level] = 'Department'
        LEFT OUTER JOIN MERchandising.ProductHierarchyView as Class
	        on Class.ProductId = stLevel.ProductId
	        and Class.[Level] = 'Class'
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
        GROUP BY l.Fascia,
                 l.SalesId, 
                 l.Name, 
                 ISNULL(division.Tag, '(Unknown Division)'), 
                 ISNULL(Department.Code, '0'),
                 ISNULL(Department.Tag, '(Unknown Department)'), 
                 ISNULL(Class.Code, '0'), 
                 ISNULL(Class.Tag, '(Unknown Class)')
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
                   DepartmentCode AS DepartmentCode,
                   Department AS Department,
                   ClassCode AS ClassCode,
                   Class AS Class,
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData e
            INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] f on e.Fascia = f.FasciaName
            GROUP BY DateKey, 
                     TransactionDate, 
                     Fascia, 
                     f.FasciaId,
                     SaleType,
                     Division,
                     DepartmentCode,
                     Department,
                     ClassCode,
                     Class
            UNION ALL --Country
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName, 
                   SaleType AS SaleType,                    
                   Division AS Division, 
                   DepartmentCode AS DepartmentCode,
                   Department AS Department,
                   ClassCode AS ClassCode,
                   Class AS Class,
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey, 
                     TransactionDate,
                     SaleType,
                     Division,
                     DepartmentCode,
                     Department,
                     ClassCode,
                     Class
        ) AS data
GO