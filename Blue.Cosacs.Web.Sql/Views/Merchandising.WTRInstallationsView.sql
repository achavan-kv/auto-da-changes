IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRInstallationsView]'))
	DROP VIEW [Merchandising].[WTRInstallationsView]

/****** Object:  View [Merchandising].[WTRInstallationsView]    Script Date: 11/22/2018 8:44:16 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRInstallationsView]
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
	        WHEN 'inst' THEN 'Installation'
	        ELSE 'Assembly'
	      END AS SaleType
        , 'Installation and Delivery' AS Department
        , 'Installation and Delivery' AS Division
        , SUM(CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'  ---Modified by Charudatt on 23Aug2018
	            THEN d .transvalue   ---Modified by Charudatt on 23Aug2018
	            ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100)   ---Modified by Charudatt on 23Aug2018
              END) AS Price
        , SUM((CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' ---Modified by Charudatt on 23Aug2018
	            THEN d .transvalue ---Modified by Charudatt on 23Aug2018
	            ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100)  END) - d.quantity * COALESCE (branchprice.CostPrice, fasciaprice.CostPrice, allprice.CostPrice, 0) ---Modified by Charudatt on 23Aug2018
             )AS GrossProfit
        FROM country AS c 
        CROSS JOIN delivery AS d 
		INNER JOIN Merchandising.Dates AS dates 
	        ON CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) = dates.DateKey
        INNER JOIN branch AS b 
	        ON b.branchno = LEFT(d.acctno, 3) 
        INNER JOIN StockInfo AS i 
	        ON i.Id = d.ItemID 
        INNER JOIN Merchandising.Location l
            ON l.SalesId = b.branchno
        INNER JOIN NonStocks.NonStock AS ns 
	        ON ns.SKU = i.SKU 
            AND ns.Type IN ('inst', 'assembly')
            AND ns.Active != 0
        LEFT OUTER JOIN NonStocks.NonStockPrice AS branchprice 
	        ON branchprice.NonStockId = ns.Id 
	        AND branchprice.BranchNumber = b.branchno 
	        AND branchprice.EffectiveDate = (SELECT MAX(EffectiveDate)
											 FROM NonStocks.NonStockPrice
											 WHERE NonStockId = branchprice.NonStockId
												AND BranchNumber = branchprice.BranchNumber
												AND EffectiveDate <= d.datetrans)
        LEFT OUTER JOIN  NonStocks.NonStockPrice AS fasciaprice 
	        ON fasciaprice.NonStockId = ns.Id 
	        AND fasciaprice.Fascia = b.StoreType
	        AND fasciaprice.BranchNumber IS NULL
            AND fasciaprice.EffectiveDate = (SELECT MAX(EffectiveDate)
											 FROM NonStocks.NonStockPrice
											 WHERE NonStockId = fasciaprice.NonStockId
												AND Fascia = fasciaprice.Fascia
	                                            AND BranchNumber IS NULL
												AND EffectiveDate <= d.datetrans)
        LEFT OUTER JOIN NonStocks.NonStockPrice AS allprice 
	        ON allprice.NonStockId = ns.Id 
	        AND allprice.Fascia IS NULL 
	        AND allprice.BranchNumber IS NULL 
	        AND allprice.EffectiveDate = (SELECT MAX(EffectiveDate)
										  FROM NonStocks.NonStockPrice
										  WHERE NonStockId = allprice.NonStockId
												AND Fascia IS NULL 
	                                            AND BranchNumber IS NULL 
												AND EffectiveDate <= d.datetrans)
		WHERE d.datetrans > DATEADD(month, -24, GETDATE())    --Added by Charudatt Aug/24/2018 because of timeout issue
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)), 
                 CAST(d.datetrans AS DATE), 
                 l.Fascia, 
                 l.SalesId, 
                 l.Name, 
                 CASE ns.[Type] 
	                WHEN 'inst' THEN 'Installation' 
	                ELSE 'Assembly' 
	             END

   --     UNION ALL

   --     SELECT CONVERT(INT, CONVERT(VARCHAR(8), COALESCE(sr.FinaliseReturnDate, sr.ResolutionDate, sr.LastUpdatedOn), 112)) AS DateKey
   --     , CAST(COALESCE(sr.FinaliseReturnDate, sr.ResolutionDate, sr.LastUpdatedOn) AS DATE) AS TransactionDate
   --     , l.Fascia
   --     , l.SalesId AS LocationId
   --     , l.Name AS LocationName
   --     , 'Installation' AS SaleType
   --     , 'Installation and Delivery' AS Department
   --     , 'Installation and Delivery' AS Division
   --     , SUM(src.Value) AS Price
   --     , SUM(src.Value - src.Cost) AS GrossProfit
   --     FROM country AS c 
   --     CROSS JOIN Service.Request AS sr
   --     INNER JOIN branch AS b 
	  --      ON b.branchno = LEFT(sr.Account, 3)
   --     INNER JOIN Merchandising.Location l
   --         ON l.SalesId = b.branchno
   --     INNER JOIN Service.Charge src
   --         ON sr.Id = src.RequestId
   --     INNER JOIN Merchandising.Dates AS dates 
	  --      ON CONVERT(INT, CONVERT(VARCHAR(8), COALESCE(sr.FinaliseReturnDate, sr.ResolutionDate, sr.LastUpdatedOn), 112)) = dates.DateKey
   --     WHERE sr.[State] = 'Closed'
   --         AND sr.[Type] IN ('II', 'IE')
			--AND (convert(date,sr.FinaliseReturnDate)='2018-11-01' or
			-- convert(date,sr.ResolutionDate) = '2018-11-01' or
			--  convert(date,sr.LastUpdatedOn)  ='2018-11-01'   )
   --     GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), COALESCE(sr.FinaliseReturnDate, sr.ResolutionDate, sr.LastUpdatedOn), 112))
   --            , CAST(COALESCE(sr.FinaliseReturnDate, sr.ResolutionDate, sr.LastUpdatedOn) AS DATE)
   --            , l.Fascia
   --            , l.SalesId
   --            , l.Name

    )

    SELECT ROW_NUMBER() OVER(ORDER BY DateKey) AS Id,
           data.*
    FROM (
            SELECT DateKey,
                   TransactionDate,
                   Fascia,
                   LocationId,
                   LocationName,
                   SaleType,
                   Department,
                   Division,
                   SUM(Price) AS Price,
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey,
                     TransactionDate,
                     Fascia,
                     LocationId,
                     LocationName,
                     SaleType,
                     Department,
                     Division
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
            INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] F ON e.Fascia = f.FasciaName
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