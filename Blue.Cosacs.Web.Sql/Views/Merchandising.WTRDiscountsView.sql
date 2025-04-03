IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRDiscountsView]'))
	DROP VIEW [Merchandising].[WTRDiscountsView]

/****** Object:  View [Merchandising].[WTRDiscountsView]    Script Date: 11/27/2018 8:45:58 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRDiscountsView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Division, Department, DepartmentCode, Price, GrossProfit)
    AS 
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) AS DateKey
        , CAST(d.datetrans AS DATE) AS TransactionDate
        , l.Fascia
        , l.SalesId AS LocationId
        , l.Name AS LocationName 
        , CASE 
            WHEN a.accttype IN ('C', 'S') THEN 'Discounts Cash'
            ELSE 'Discounts Credit'
          END AS SaleType
        , ISNULL(division.LevelName ,'Discounts') AS Division
        , ISNULL(department.LevelName ,'Discounts') AS Department
        , CAST(ISNULL(department.LevelId, 0) AS VARCHAR) AS DepartmentCode
        , SUM(CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'  ---Modified by Charudatt on 23Aug2018
	            THEN d .transvalue   ---Modified by Charudatt on 23Aug2018
	            ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100)   ---Modified by Charudatt on 23Aug2018
              END) AS Price
        , SUM((CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' ---Modified by Charudatt on 23Aug2018
	            THEN d .transvalue ---Modified by Charudatt on 23Aug2018
	            --ELSE (d .transvalue * 100) / ( ns.taxrate + 100)   END) - d.quantity * COALESCE (branchprice.CostPrice, fasciaprice.CostPrice, allprice.CostPrice, 0) ---Modified by Charudatt on 23Aug2018
				 ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100)   END) - d.quantity * COALESCE (branchprice.CostPrice, fasciaprice.CostPrice, 0) 
             )AS GrossProfit
        FROM country AS c 
        CROSS JOIN delivery AS d 
		 INNER JOIN Merchandising.Dates AS dates 
	        ON CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) = dates.DateKey
        INNER JOIN branch AS b 
	        ON b.branchno = LEFT(d.acctno, 3) 
        INNER JOIN Merchandising.Location l
            ON l.SalesId = b.branchno
        INNER JOIN acct a
            ON a.acctno = d.acctno
        INNER JOIN StockInfo AS i 
	        ON i.Id = d.ItemID 
        INNER JOIN NonStocks.NonStock ns
            ON ns.SKU = i.SKU
            AND ns.[Type] = 'discount'
            AND ns.Active != 0
       
        LEFT OUTER JOIN NonStocks.HierarchyView department
	        ON department.NonStockId = ns.Id
            AND department.[Level] = 'Department'        
        LEFT OUTER JOIN NonStocks.HierarchyView division
	        ON division.NonStockId = ns.Id
            AND division.[Level] = 'Division'
        LEFT OUTER JOIN NonStocks.NonStockPrice AS branchprice 
	        ON branchprice.NonStockId = ns.Id 
	        AND branchprice.BranchNumber = b.branchno 
	        AND branchprice.EffectiveDate <= d.datetrans 
        LEFT OUTER JOIN  NonStocks.NonStockPrice AS fasciaprice 
	        ON fasciaprice.NonStockId = ns.Id 
	        AND fasciaprice.Fascia = b.StoreType 
	        AND fasciaprice.EffectiveDate <= d.datetrans 
	        AND fasciaprice.BranchNumber IS NULL 
        --LEFT OUTER JOIN NonStocks.NonStockPrice AS allprice 
	       -- ON allprice.NonStockId = ns.Id 
	       -- AND allprice.Fascia IS NULL 
	       -- AND allprice.BranchNumber IS NULL 
	       -- AND allprice.EffectiveDate <= d.datetrans 
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
	        --AND (
			      --  allprice.EffectiveDate IS NULL 
			      --  OR allprice.EffectiveDate IN
				     --   (
					    --    SELECT MAX(EffectiveDate) AS Expr1
					    --    FROM NonStocks.NonStockPrice AS m
					    --    WHERE (Id = allprice.Id) 
					    --    AND (EffectiveDate <= d.datetrans)
				     --   )
    		   -- ) 
        and d.datetrans > DATEADD(month, -24, GETDATE())   --Added by Nilesh May/09/2018 because of timeout issue
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)), 
                 CAST(d.datetrans AS DATE), 
                 l.Fascia, 
                 l.SalesId, 
                 l.Name, 
                 ISNULL(division.LevelName ,'Discounts'), 
                 ISNULL(department.LevelName ,'Discounts'), 
                 CAST(ISNULL(department.LevelId, 0) AS VARCHAR), 
                 CASE 
                     WHEN a.accttype IN ('C', 'S') THEN 'Discounts Cash'
                     ELSE 'Discounts Credit'
                 END

        UNION ALL

        SELECT CONVERT(INT, CONVERT(VARCHAR(8), f.datetrans, 112)) AS DateKey
        , CAST(f.datetrans AS DATE) AS TransactionDate
        , l.Fascia
        , l.SalesId AS LocationId
        , l.Name AS LocationName 
        , CASE 
            WHEN  f.transtypecode = 'DDC' THEN 'Discounts Cash'
            ELSE 'Discounts Credit'
          END AS SaleType
        , 'Discounts' AS Division
        , 'Discounts' AS Department
        , '0' AS DepartmentCode
        , SUM(f.transvalue) AS Price
        , 0 GrossProfit
        FROM country AS c 
        CROSS JOIN fintrans AS f 
		    INNER JOIN Merchandising.Dates AS dates 
	        ON CONVERT(INT, CONVERT(VARCHAR(8), f.datetrans, 112)) = dates.DateKey
        INNER JOIN Merchandising.Location l
            ON l.SalesId = LEFT(f.acctno, 3)
        INNER JOIN acct a
            ON a.acctno = f.acctno
    
        WHERE f.transtypecode IN ('DDH', 'DDC')
	and f.datetrans > DATEADD(month,-24, GETDATE())
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), f.datetrans, 112)),
                 CAST(f.datetrans AS DATE),
                 l.Fascia,
                 l.SalesId,
                 l.Name,
                 CASE 
                    WHEN  f.transtypecode = 'DDC' THEN 'Discounts Cash'
                    ELSE 'Discounts Credit'
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
                   Division AS Division, 
                   Department AS Department, 
                   DepartmentCode AS DepartmentCode, 
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
                     Department, 
                     DepartmentCode
            UNION ALL --Country
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName, 
                   SaleType AS SaleType, 
                   Division AS Division, 
                   Department AS Department, 
                   DepartmentCode AS DepartmentCode, 
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey, 
                     TransactionDate,
                     SaleType,
                     Division,
                     Department,
                     DepartmentCode
        ) AS data




GO