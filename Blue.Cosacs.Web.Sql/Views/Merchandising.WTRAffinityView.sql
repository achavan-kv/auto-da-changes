IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRAffinityView]'))
	DROP VIEW [Merchandising].[WTRAffinityView]

/****** Object:  View [Merchandising].[WTRAffinityView]    Script Date: 8/25/2018 3:39:25 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRAffinityView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Department, Division, Price, GrossProfit)
    AS
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) AS DateKey
        , CAST(d.datetrans AS DATE) AS TransactionDate
        , l.Fascia
        , l.SalesId AS LocationId
        , l.Name AS LocationName 
        , CASE ns.[Type]
            WHEN 'rassist' THEN 'Ready Assist' 
            ELSE 'Other Annual'
          END AS SaleType
        , 'Affinity' AS Department
        , 'Affinity' AS Division
        , SUM(CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' 
	            THEN d .transvalue
				ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100) 
              END) AS Price
        , SUM((CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' 
	            THEN  d .transvalue
	            ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100)  END) - d.quantity * COALESCE (branchprice.CostPrice, fasciaprice.CostPrice, allprice.CostPrice) 
             )AS GrossProfit
        FROM country AS c 
        CROSS JOIN delivery AS d 
        INNER JOIN branch AS b 
	        ON b.branchno = d.branchno 
        INNER JOIN Merchandising.Location l
            ON l.SalesId = b.branchno
        INNER JOIN StockInfo AS i 
	        ON i.Id = d.ItemID 
        INNER JOIN NonStocks.NonStock AS ns 
	        ON ns.SKU = i.SKU 
            AND ns.[Type] IN ('rassist', 'annual')
            AND ns.Active != 0
        INNER JOIN Merchandising.Dates AS dates 
	        ON CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) = dates.DateKey
        LEFT OUTER JOIN NonStocks.NonStockPrice AS branchprice 
	        ON branchprice.NonStockId = ns.Id 
	        AND branchprice.BranchNumber = b.branchno 
	        AND branchprice.EffectiveDate <= d.datetrans 
        LEFT OUTER JOIN  NonStocks.NonStockPrice AS fasciaprice 
	        ON fasciaprice.NonStockId = ns.Id 
	        AND fasciaprice.Fascia = b.StoreType 
	        AND fasciaprice.EffectiveDate <= d.datetrans 
	        AND fasciaprice.BranchNumber IS NULL 
        LEFT OUTER JOIN NonStocks.NonStockPrice AS allprice 
	        ON allprice.NonStockId = ns.Id 
	        AND allprice.Fascia IS NULL 
	        AND allprice.BranchNumber IS NULL 
	        AND allprice.EffectiveDate <= d.datetrans 
        WHERE (
			        branchprice.EffectiveDate IS NULL 
			        OR branchprice.EffectiveDate IN
				        (
					        SELECT MAX(EffectiveDate) AS Expr1
					        FROM NonStocks.NonStockPrice AS m
					        WHERE (Id = branchprice.Id) 
					        AND (EffectiveDate <= d.datetrans)
				        )
		        ) 
	        AND (
			        fasciaprice.EffectiveDate IS NULL 
			        OR fasciaprice.EffectiveDate IN
				        (
					        SELECT MAX(EffectiveDate) AS Expr1
					        FROM NonStocks.NonStockPrice AS m
					        WHERE (Id = fasciaprice.Id) 
					        AND (EffectiveDate <= d.datetrans)
				        )
		        ) 
	        AND (
			        allprice.EffectiveDate IS NULL 
			        OR allprice.EffectiveDate IN
				        (
					        SELECT MAX(EffectiveDate) AS Expr1
					        FROM NonStocks.NonStockPrice AS m
					        WHERE (Id = allprice.Id) 
					        AND (EffectiveDate <= d.datetrans)
				        )
    		    ) 
	and d.datetrans > DATEADD(month, -24, GETDATE())    --Added by Nilesh May/09/2018 because of timeout issue
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)), 
                 CAST(d.datetrans AS DATE), 
                 l.Fascia, 
                 l.SalesId, 
                 l.Name, 
                 CASE ns.[Type]
                    WHEN 'rassist' THEN 'Ready Assist' 
                    ELSE 'Other Annual'
                 END
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
                   Department AS Department, 
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
                     Department,
                     Division
            UNION ALL --Country
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName, 
                   SaleType AS SaleType, 
                   Department AS Department, 
                   Division AS Division, 
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey, 
                     TransactionDate,
                     SaleType,
                     Department,
                     Division
        ) AS data


GO
