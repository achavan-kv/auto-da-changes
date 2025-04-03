IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRServiceChargeView]'))
	DROP VIEW [Merchandising].[WTRServiceChargeView]

/****** Object:  View [Merchandising].[WTRServiceChargeView]    Script Date: 8/24/2018 4:37:03 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ========================================================================
-- Version:		<001> 
-- ========================================================================
CREATE VIEW [Merchandising].[WTRServiceChargeView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Department, Division, Price, GrossProfit)
    AS
    (    
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) AS DateKey
               , CAST(d.datetrans AS DATE) AS TransactionDate
               , loc.Fascia
               , loc.SalesId AS LocationId
               , loc.Name AS LocationName
               , CASE 
                    WHEN l.AcctNo IS NOT NULL THEN 'Loan Service Charge' 
                    ELSE 'Credit Service Charge' 
                 END AS SaleType 
               , CASE 
                    WHEN l.AcctNo IS NOT NULL THEN 'Loan Service Charge' 
                    ELSE 'Credit Service Charge' 
                 END AS Department
               , CASE 
                    WHEN l.AcctNo IS NOT NULL THEN 'Loan Service Charge' 
                    ELSE 'Credit Service Charge' 
                 END AS Division 
				 , SUM(CASE
	              WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' ---Modified by Charudatt on 23Aug2018
				  THEN d .transvalue ---Modified by Charudatt on 23Aug2018
				  ELSE (d .transvalue * 100) / ( ISNULL(ns.taxrate,0) + 100) ---Modified by Charudatt on 23Aug2018
	                END) AS Price
				, SUM((CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'  ---Modified by Charudatt on 23Aug2018
	            THEN d .transvalue  ---Modified by Charudatt on 23Aug2018
	            ELSE  (d .transvalue * 100) / ( ISNULL(ns.taxrate,0) + 100)   ---Modified by Charudatt on 23Aug2018
	           END) ) AS GrossProfit
			   --, SUM(d.transvalue) AS Price
      --         , SUM(d.transvalue) AS GrossProfit
        FROM  country AS c 
        CROSS JOIN delivery AS d 
        INNER JOIN custacct AS ca 
	        ON ca.acctno = d.acctno 
	        AND ca.hldorjnt = 'H' 
        INNER JOIN Merchandising.Location loc
            ON loc.SalesId = d.stocklocn
        INNER JOIN StockInfo AS i 
	        ON i.Id = d.ItemID 
        INNER JOIN NonStocks.NonStock ns
            ON ns.SKU = i.SKU
            AND ns.Active != 0
        INNER JOIN Merchandising.Dates AS dates 
            ON CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) = dates.DateKey
        LEFT OUTER JOIN CashLoan AS l 
	        ON l.AcctNo = d.acctno AND ca.custid = l.Custid 
        WHERE i.SKU = 'DT'
			and  d.datetrans > DATEADD (month, -24, GETDATE())   --Added by Charudatt Aug/24/2018 because of timeout issue
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)), 
                 CAST(d.datetrans AS DATE), 
                 loc.Fascia, 
                 loc.SalesId, 
                 loc.Name, 
                 CASE 
                    WHEN l.AcctNo IS NOT NULL THEN 'Loan Service Charge' 
                    ELSE 'Credit Service Charge' 
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


