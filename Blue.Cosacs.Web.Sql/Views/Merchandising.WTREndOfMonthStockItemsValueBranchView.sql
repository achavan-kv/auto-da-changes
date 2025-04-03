IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTREndOfMonthStockItemsValueBranchView]'))
	DROP VIEW [Merchandising].[WTREndOfMonthStockItemsValueBranchView]

/****** Object:  View [Merchandising].[WTREndOfMonthStockItemsValueBranchView]    Script Date: 8/25/2018 4:02:47 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================

CREATE VIEW [Merchandising].[WTREndOfMonthStockItemsValueBranchView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Division, Price, GrossProfit)
    AS
    (
        SELECT svs.SnapshotDateId AS DateKey
               , CAST(CAST(svs.SnapshotDateId AS VARCHAR) AS DATE) AS TransactionDate
               , l.Fascia
               , l.SalesId AS LocationId
               , l.Name AS LocationName
               , 'Company Stock Value' AS SaleType
               , 'Branch' AS Division
               , SUM(svs.StockOnHandValue) AS Price
               , 0 AS GrossProfit
        FROM  Merchandising.StockValuationSnapshot svs
        INNER JOIN Merchandising.Location l
            ON l.Id = svs.LocationId
        INNER JOIN Merchandising.Dates d
            ON d.DateKey = svs.SnapshotDateId
        INNER JOIN Merchandising.Product p
            ON svs.ProductId = p.Id
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
        GROUP BY svs.SnapshotDateId, CAST(CAST(svs.SnapshotDateId AS VARCHAR) AS DATE), 
                 l.Fascia,
                 l.SalesId, 
                 l.Name
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