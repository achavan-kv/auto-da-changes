IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRWarrantyView]'))
	DROP VIEW [Merchandising].[WTRWarrantyView]

/****** Object:  View [Merchandising].[WTRWarrantyView]    Script Date: 8/24/2018 7:08:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRWarrantyView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, SaleType, Division, Price, GrossProfit)
    AS
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) AS DateKey
        , CAST(d.datetrans AS DATE) AS TransactionDate
        , l.Fascia
        , l.SalesId AS LocationId
        , l.Name AS LocationName
        , CASE 
	        WHEN SUBSTRING(d.acctno, 4, 1) = '5' THEN 'Cash'
	        WHEN SUBSTRING(d.acctno, 4, 1) = '4' THEN 'Cash'
	        ELSE 'Credit'
	      END AS SaleType
        , ISNULL(t.Name, 'Warranty') AS Division
        , SUM(CASE
	              WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E' ---Modified by Charudatt on 23Aug2018
				  THEN d .transvalue ---Modified by Charudatt on 23Aug2018
	              ELSE (d .transvalue * 100) / ( ISNULL(w.taxrate, 0) + 100) ---Modified by Charudatt on 23Aug2018
	                  END) AS Price
        , SUM((CASE 
	            WHEN c.agrmttaxtype = 'E' AND c.taxtype = 'E'  ---Modified by Charudatt on 23Aug2018
	            THEN d .transvalue  ---Modified by Charudatt on 23Aug2018
	            ELSE (d .transvalue * 100) / ( ISNULL(w.taxrate, 0) + 100)  ---Modified by Charudatt on 23Aug2018
	           END) - d.quantity * ISNULL(wm.CostPrice, 0)
               ) AS GrossProfit
        FROM country c
        CROSS JOIN delivery AS d 
        INNER JOIN Merchandising.Dates AS dates 
	        ON CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)) = dates.DateKey
        INNER JOIN branch AS b 
	        ON b.branchno = LEFT(d.acctno, 3)
        INNER JOIN Merchandising.Location l
            ON l.SalesId = b.branchno
        INNER JOIN StockInfo AS i 
	        ON i.Id = d.ItemID 
        INNER JOIN Warranty.Warranty AS w 
	        ON w.Number = COALESCE(i.SKU, i.itemno, i.IUPC)
        LEFT OUTER JOIN Financial.WarrantyMessage wm
            ON wm.ContractNumber = d.contractno
        LEFT OUTER JOIN warranty.WarrantyTags wt 
	        ON wt.warrantyId = w.Id
	        AND wt.LevelId = 
		        (
			        SELECT MIN(id) 
			        FROM warranty.[level]
		        )
        LEFT OUTER JOIN warranty.Tag t 
	        ON t.Id = wt.TagId
        WHERE d.delorcoll = 'D'
		and d.datetrans > DATEADD(month, -24, GETDATE())    --Added by Nilesh May/09/2018 because of timeout issue
		GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), d.datetrans, 112)), 
                 CAST(d.datetrans AS DATE), 
                 Fascia, 
                 l.SalesId, 
                 l.Name, 
                 ISNULL(t.Name, 'Warranty'), 
                 CASE 
	                WHEN SUBSTRING(d.acctno, 4, 1) = '5' THEN 'Cash'
	                WHEN SUBSTRING(d.acctno, 4, 1) = '4' THEN 'Cash'
	                ELSE 'Credit'
	             END

        UNION ALL

        SELECT CONVERT(INT, CONVERT(VARCHAR(8), wr.ReturnDate, 112)) AS DateKey
        , wr.ReturnDate AS TransactionDate
        , l.Fascia
        , l.SalesId AS LocationId
        , l.Name AS LocationName
        , wr.SaleType
        , CASE 
            WHEN wr.Department = 'PCE' THEN 'Electrical'
            ELSE 'Furniture'
          END AS Division
        , SUM(-wr.SalePrice + wr.ReturnValue) AS Price
        , SUM(-(wr.SalePrice - wr.CostPrice) + (wr.ReturnValue - wr.ReturnCost)) AS GrossProfit
        FROM [Merchandising].[WTRWarrantyReturnValuesView] wr
        INNER JOIN Merchandising.Location l
            ON l.SalesId = wr.BranchNo
		WHERE wr.ReturnDate > DATEADD(month, -24, GETDATE())    --Added by Charudatt Aug/24/2018 because of timeout issue
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), wr.ReturnDate, 112)),
                 wr.ReturnDate, 
                 l.Fascia, 
                 l.SalesId, 
                 l.Name, 
                 wr.SaleType, 
                 CASE 
                    WHEN wr.Department = 'PCE' THEN 'Electrical'
                    ELSE 'Furniture'
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
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData e
            INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] F ON e.Fascia = f.FasciaName
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
