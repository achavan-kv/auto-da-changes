IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTREndOfMonthStockItemsValueView]'))
	DROP VIEW [Merchandising].[WTREndOfMonthStockItemsValueView]

/****** Object:  View [Merchandising].[WTREndOfMonthStockItemsValueView]    Script Date: 8/24/2018 6:17:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE VIEW [Merchandising].[WTREndOfMonthStockItemsValueView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Division, DepartmentCode, Department, ClassCode, Class, Price, GrossProfit)
    AS
    (
        SELECT svs.SnapshotDateId AS DateKey
               , CAST(CAST(svs.SnapshotDateId AS VARCHAR) AS DATE) AS TransactionDate
               , l.Fascia
               , l.SalesId AS LocationId
               , l.Name AS LocationName
               , 'Stock Value' AS SaleType
               , ISNULL(division.Tag, '(Unknown Division)') AS Division
               , ISNULL(Department.Code, '0') AS DepartmentCode
               , ISNULL(Department.Tag, '(Unknown Department)') AS Department
               , ISNULL(Class.Code, '0') AS ClassCode
               , ISNULL(Class.Tag, '(Unknown Class)') AS Class
               , SUM(svs.StockOnHandValue) AS Price
               , 0 AS GrossProfit
        FROM Merchandising.StockValuationSnapshot svs
        INNER JOIN Merchandising.Product p
            ON svs.ProductId = p.Id
        INNER JOIN Merchandising.Location l
            ON l.Id = svs.LocationId
        INNER JOIN Merchandising.Dates d
            ON svs.SnapshotDateId = d.DateKey
        LEFT OUTER JOIN Merchandising.ProductHierarchyView AS division 
	        ON division.ProductId = svs.ProductId 
	        AND division.[Level] = 'Division' 
        LEFT OUTER JOIN  Merchandising.ProductHierarchyView AS Department 
	        ON Department.ProductId = svs.ProductId
	        AND Department.[Level] = 'Department'
        LEFT OUTER JOIN MERchandising.ProductHierarchyView as Class
	        ON Class.ProductId = svs.ProductId
	        AND Class.[Level] = 'Class' 
        WHERE p.[Status] != (SELECT Id FROM Merchandising.ProductStatus where Name = 'Non Active')
			AND  CAST(CAST(svs.SnapshotDateId AS VARCHAR) AS DATE) > DATEADD(month, -24, GETDATE())    --Added by Charudatt Aug/24/2018 because of timeout issue
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
        GROUP BY svs.SnapshotDateId, 
                 CAST(CAST(svs.SnapshotDateId AS VARCHAR) AS DATE), 
                 l.Fascia,
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


