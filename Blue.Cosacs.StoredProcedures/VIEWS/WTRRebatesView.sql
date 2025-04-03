IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRRebatesView]'))
	DROP VIEW [Merchandising].[WTRRebatesView]

/****** Object:  View [Merchandising].[WTRRebatesView]    Script Date: 8/24/2018 4:59:06 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE VIEW [Merchandising].[WTRRebatesView]
AS 

WITH Data(TransactionDate, Fascia, LocationId, LocationName, Price, GrossProfit)
AS 
(
    SELECT CAST(d.datetrans AS DATE) AS TransactionDate
           , l.Fascia
           , l.SalesId AS LocationId
           , l.Name AS LocationName
		   , SUM(CASE
	              WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' ---Modified by Charudatt on 23Aug2018
				  THEN d .transvalue ---Modified by Charudatt on 23Aug2018
				  ELSE (d .transvalue * 100) / ( isnull(ns.taxrate,0) + 100) ---Modified by Charudatt on 23Aug2018
	              END) AS Price
				, SUM((CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'  ---Modified by Charudatt on 23Aug2018
	            THEN d .transvalue  ---Modified by Charudatt on 23Aug2018
	            ELSE (d .transvalue * 100) / (isnull(ns.taxrate,0) + 100)   ---Modified by Charudatt on 23Aug2018
	           END) ) AS GrossProfit
           --, SUM(d.transvalue) AS Price
           --, SUM(d.transvalue) AS GrossProfit
    FROM  country AS c 
    CROSS JOIN delivery AS d 
    INNER JOIN StockInfo AS i 
	    ON i.Id = d.ItemID 
    INNER JOIN Merchandising.Location l
        ON l.SalesId = LEFT(d.acctno, 3)
    INNER JOIN NonStocks.NonStock ns
        ON i.SKU = ns.SKU
        AND ns.SKU = 'RB'
        AND ns.Active != 0
	WHERE d.datetrans >DATEADD(month, -24, GETDATE())    --Added by Charudatt Aug/24/2018 because of timeout issue
    GROUP BY CAST(d.datetrans AS DATE), 
             l.Fascia, 
             l.SalesId, 
             l.Name
UNION ALL
    SELECT CAST(f.datetrans AS DATE) AS TransactionDate
           , l.Fascia
           , l.SalesId AS LocationId
           , l.Name AS LocationName
           , SUM(f.transvalue) AS Price
           , SUM(f.transvalue) AS GrossProfit
    FROM  country AS c 
    CROSS JOIN fintrans AS f
    INNER JOIN Merchandising.Location l
        ON l.SalesId = LEFT(f.acctno, 3)
    WHERE f.transtypecode = 'BDU'
	AND  f.datetrans > DATEADD(month, -24, GETDATE())    --Added by Charudatt Aug/24/2018 because of timeout issue
    GROUP BY CAST(f.datetrans AS DATE), 
             l.Fascia, 
             l.SalesId, 
             l.Name
)

SELECT ROW_NUMBER() OVER(ORDER BY DateKey) AS Id,
           data.*
    FROM (
            SELECT CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)) AS DateKey
                   , TransactionDate
                   , Fascia
                   , LocationId
                   , LocationName
                   , 'Rebate and BDU' AS SaleType      --BDU - Bad Debt Unearned credit
                   , 'Rebate and BDU' AS Division
                   , 'Rebate and BDU' AS Department 
                   , CAST(SUM(Price) AS DECIMAL(38, 17)) AS Price
                   , CAST(SUM(GrossProfit) AS DECIMAL(38, 17)) AS GrossProfit
            FROM Data
            INNER JOIN Merchandising.Dates AS dates
                ON CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)) = dates.DateKey
            GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)),
                   TransactionDate, 
                   Fascia, 
                   LocationId, 
                   LocationName
            UNION ALL --Fascia
            SELECT CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)) AS DateKey
                   , TransactionDate
                   , Fascia
                   ,f.FasciaId AS LocationId
                   , Fascia AS LocationName
                   , 'Rebate and BDU' AS SaleType      --BDU - Bad Debt Unearned credit
                   , 'Rebate and BDU' AS Division
                   , 'Rebate and BDU' AS Department 
                   , CAST(SUM(Price) AS DECIMAL(38, 17)) AS Price
                   , CAST(SUM(GrossProfit) AS DECIMAL(38, 17)) AS GrossProfit
            FROM Data d
            INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] f on d.Fascia = f.FasciaName
            INNER JOIN Merchandising.Dates AS dates
                ON CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)) = dates.DateKey
            GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)),
                   TransactionDate, 
                   Fascia, 
                   f.FasciaId
            UNION ALL --Country
            SELECT CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)) AS DateKey
                   , TransactionDate
                   , 'Company' AS Fascia
                   , 0 AS LocationId
                   , 'Company' AS LocationName
                   , 'Rebate and BDU' AS SaleType      --BDU - Bad Debt Unearned credit
                   , 'Rebate and BDU' AS Division
                   , 'Rebate and BDU' AS Department 
                   , CAST(SUM(Price) AS DECIMAL(38, 17)) AS Price
                   , CAST(SUM(GrossProfit) AS DECIMAL(38, 17)) AS GrossProfit
            FROM Data
            INNER JOIN Merchandising.Dates AS dates
                ON CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)) = dates.DateKey
            GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), TransactionDate, 112)),
                   TransactionDate
        ) AS data

GO




