IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[Merchandising].[WTRPenaltyView]'))
	DROP VIEW [Merchandising].[WTRPenaltyView]

/****** Object:  View [Merchandising].[WTRPenaltyView]    Script Date: 11/22/2018 7:59:34 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO
-- ========================================================================
-- Version:		<002>
-- ========================================================================
CREATE VIEW [Merchandising].[WTRPenaltyView]
AS
    WITH ExportData(DateKey, TransactionDate, Fascia, LocationId, LocationName, Division, SaleType, Price, GrossProfit)
    AS
    (
        SELECT CONVERT(INT, CONVERT(VARCHAR(8), CASE 
                                                    WHEN DATEPART(HOUR, i.DateStart) > 7 THEN i.DateStart
                                                    ELSE DATEADD(DAY, -1, i.DateStart)
                                                END, 112)) AS DateKey,
               CAST(CASE 
                        WHEN DATEPART(HOUR, i.DateStart) > 7 THEN i.DateStart
                        ELSE DATEADD(DAY, -1, i.DateStart)
                    END AS DATE) AS TransactionDate,
               l.Fascia,
               l.SalesId AS LocationId,
               l.Name AS LocationName,
               CASE 
	               WHEN cl.AcctNo IS NOT NULL THEN 'Penalty Interest Loans'
                   WHEN a.accttype = 'T' THEN 'Penalty Interest Store Card'
	               ELSE 'Penalty Interest Credit'
	           END AS Division,
               CASE 
	               WHEN cl.AcctNo IS NOT NULL THEN 'Penalty Interest Loans'
                   WHEN a.accttype = 'T' THEN 'Penalty Interest Store Card'
	               ELSE 'Penalty Interest Credit'
	           END AS SaleType,
			   SUM(f.transvalue) AS Price,
               SUM(f.transvalue) AS GrossProfit
        FROM country AS c 
        CROSS JOIN fintrans f 
        INNER JOIN Merchandising.Location l
        ON l.SalesId = LEFT(f.acctno, 3)
        INNER JOIN acct a
        ON a.acctno = f.acctno
            AND a.accttype NOT IN ('C', 'S')
        INNER JOIN InterfaceControl i
        ON i.RunNo = f.runno
            AND i.Interface = 'COS FACT'
            AND i.Result = 'P'
        INNER JOIN custacct ca
        ON f.acctno = ca.acctno
            AND ca.hldorjnt = 'H'
        LEFT OUTER JOIN CashLoan cl
        ON f.acctno = cl.AcctNo
            AND ca.custid = cl.Custid
        WHERE f.transtypecode = 'INT'
		AND  i.DateStart > DATEADD(month, -24, GETDATE())    --Added by Charudatt Aug/24/2018 because of timeout issue
        GROUP BY CONVERT(INT, CONVERT(VARCHAR(8), CASE 
                                                      WHEN DATEPART(HOUR, i.DateStart) > 7 THEN i.DateStart
                                                      ELSE DATEADD(DAY, -1, i.DateStart)
                                                  END, 112)),
                 CAST(CASE 
                          WHEN DATEPART(HOUR, i.DateStart) > 7 THEN i.DateStart
                          ELSE DATEADD(DAY, -1, i.DateStart)
                      END AS DATE),
                 l.Fascia,
                 l.SalesId, 
                 l.Name, 
                 CASE 
	                 WHEN cl.AcctNo IS NOT NULL THEN 'Penalty Interest Loans'
                     WHEN a.accttype = 'T' THEN 'Penalty Interest Store Card'
	                 ELSE 'Penalty Interest Credit'
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
                   Division AS Division, 
                   SaleType AS SaleType,                    
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData e
            INNER JOIN [Merchandising].[Merchandising.WTRFasciaView] f on e.Fascia = f.FasciaName
            GROUP BY DateKey, 
                     TransactionDate, 
                     Fascia, 
                     f.FasciaId,
                     Division,
                     SaleType
            UNION ALL --Country
            SELECT DateKey, 
                   TransactionDate, 
                   'Company' AS Fascia, 
                   0 AS LocationId, 
                   'Company' AS LocationName, 
                   Division AS Division, 
                   SaleType AS SaleType,                    
                   SUM(Price) AS Price, 
                   SUM(GrossProfit) AS GrossProfit
            FROM ExportData
            GROUP BY DateKey, 
                     TransactionDate,
                     Division,
                     SaleType
        ) AS data


GO