IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRIGenericServicesView]'))
	DROP VIEW [Merchandising].[WTRIGenericServicesView]

/****** Object:  View [Merchandising].[WTRIGenericServicesView]    Script Date: 11/22/2018 6:15:42 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE VIEW [Merchandising].[WTRIGenericServicesView]
AS 
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Department, Division, DepartmentCode, Price, GrossProfit)
    AS
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) AS DateKey
        , CAST(d.datetrans AS DATE) AS TransactionDate
        , l.Fascia
        , l.SalesId AS LocationId
        , l.Name AS LocationName
        , 'Other Generic Services' AS SaleType
        , ISNULL(department.LevelName, 'Other Generic Services') AS Department
        , ISNULL(division.LevelName, 'Other Generic Services') AS Division
        , CAST(ISNULL(department.LevelId, 0) AS VARCHAR) AS DepartmentCode
        , SUM(CASE
				WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'   -- Exclude the tax
	            THEN d .transvalue 
				ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100) 
	          END) AS Price
        , SUM((CASE 
				 WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'   -- Exclude the tax 
				 THEN  d .transvalue 
				 ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100) 
	           --END) - d.quantity * COALESCE (branchprice.CostPrice, fasciaprice.CostPrice, allprice.CostPrice, 0)
			   END) - d.quantity * COALESCE (branchprice.CostPrice, fasciaprice.CostPrice, 0)
             ) AS GrossProfit
        FROM country AS c 
        CROSS JOIN delivery AS d 
        INNER JOIN branch AS b 
	        ON b.branchno = left(d.acctno,3)
        INNER JOIN StockInfo AS i 
	        ON i.Id = d.ItemID 
        INNER JOIN Merchandising.Location l
            ON l.SalesId = b.branchno
        INNER JOIN NonStocks.NonStock AS ns 
	        ON ns.SKU = i.SKU 
            AND ns.[Type] = 'generic'
            AND ns.Active != 0
        LEFT OUTER JOIN [NonStocks].[HierarchyView] AS department
            ON department.NonStockId = ns.Id
            AND department.[Level] = 'Department'
        LEFT OUTER JOIN [NonStocks].[HierarchyView] AS division
            ON division.NonStockId = ns.Id
            AND division.[Level] = 'Division'
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
        --LEFT OUTER JOIN NonStocks.NonStockPrice AS allprice 
	       -- ON allprice.NonStockId = ns.Id 
	       -- AND allprice.Fascia IS NULL 
	       -- AND allprice.BranchNumber IS NULL 
	       -- AND allprice.EffectiveDate = (SELECT MAX(EffectiveDate) 
        --                                     FROM NonStocks.NonStockPrice
        --                                     WHERE NonStockId = allprice.NonStockId
        --                                        AND Fascia IS NULL
        --                                        AND BranchNumber IS NULL
        --                                        AND EffectiveDate <= d.datetrans)
        INNER JOIN Merchandising.Dates AS dates 
	        ON CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) = dates.DateKey
        WHERE ns.SKU NOT IN ('DT', 'RB', 'LOAN', 'STAX')
		AND  d.datetrans > DATEADD(month, - 24 , GETDATE())   --Added by Charudatt Aug/24/2018 because of timeout issue
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)), 
                 CAST(d.datetrans AS DATE), 
                 l.Fascia, 
                 l.SalesId, 
                 l.Name, 
                 ISNULL(department.LevelName, 'Other Generic Services'), 
                 ISNULL(division.LevelName, 'Other Generic Services'), 
                 CAST(ISNULL(department.LevelId, 0) AS VARCHAR)
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
                     Department,
                     Division,
                     DepartmentCode
            UNION ALL --Country
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName, 
                   SaleType AS SaleType, 
                   Department AS Department, 
                   Division AS Division, 
                   DepartmentCode AS DepartmentCode, 
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey, 
                     TransactionDate,
                     SaleType,
                     Department,
                     Division,
                     DepartmentCode
        ) AS data


GO


