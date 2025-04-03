SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_cyc_indicadores') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_cyc_indicadores
END
GO

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 17 August 2012
-- Description:	Hyperion Extract files
--
-- 12/04/13 jec #12859 - UAT12602 
-- =============================================

CREATE PROCEDURE dbo.t_src_cyc_indicadores
(
    @dateTo DATETIME
)

AS

DECLARE @directory VARCHAR(100),
        @filename VARCHAR(50),
        @bcpCommand VARCHAR(5000),
        @bcpPath VARCHAR(100),
        @dateFrom DATETIME,
        @totOutstBal float,	
        @totReturnNumbers float,
        @totDeliveries FLOAT,
        @taxAmount FLOAT,
        @taxType VARCHAR(1)
        
        
SET @dateFrom = DATEADD(MONTH, -12, @dateTo)
set @dateto = dateadd(second,-1, @dateto)
SET @filename = 't_src_cyc_indicadores.txt'		

SELECT @taxType = Value FROM dbo.CountryMaintenance WHERE CodeName = 'agrmttaxtype'

--Set the tax rate based on the country parameter
SELECT @taxAmount = value FROM dbo.CountryMaintenance WHERE CodeName = 'taxrate'
        
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\110\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'		-- #12859

CREATE TABLE #totDeliveries
(
    branchno SMALLINT,
    transValue FLOAT
)	

CREATE TABLE #totDisDeliveries
(
    branchno SMALLINT,
    transValue FLOAT
)	

CREATE TABLE #totReturns
(
    branchno SMALLINT,
    transValue FLOAT
)	

CREATE TABLE #totDisReturns
(
    branchno SMALLINT,
    transValue FLOAT
)	

CREATE TABLE #totRepo
(
    branchno SMALLINT,
    repos FLOAT
)	

IF @taxType = 'I'
BEGIN

    --Getting totals for deliveries
    INSERT INTO #totDeliveries
    SELECT temp.branchno, SUM(temp.transValue)
    FROM(
        SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) as branchno-- branchno
               , SUM((100 * d.transvalue) / (100 + COALESCE(t.SpecialRate, w.TaxRate, @taxAmount))) as transValue-- transValue
        FROM dbo.delivery d 
        INNER JOIN  Warranty.Warranty w  
            ON d.itemno = w.Number
                AND d.delorcoll = 'D'
                AND d.datetrans between @dateFrom and  @dateTo
                AND d.quantity > 0
        LEFT OUTER JOIN dbo.taxitemhist t
        ON t.ItemID = d.ItemID
            AND d.datetrans BETWEEN t.datefrom AND isnull(t.dateto, getdate())
        GROUP BY LEFT(d.acctno, 3)
        UNION
        SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) as branchno-- branchno
               , SUM((100 * d.transvalue) / (100 + ISNULL(t.SpecialRate, @taxAmount))) as transValue-- transValue
        FROM dbo.delivery d 
        INNER JOIN  dbo.StockInfo s  
            ON d.ItemID = s.ID
                AND d.delorcoll = 'D'
                AND d.datetrans between @dateFrom and  @dateTo
                AND d.quantity > 0
                AND (s.itemtype = 'S'
                     OR 
                     s.category IN (SELECT code FROM code WHERE category = 'PCDIS'))
        LEFT OUTER JOIN dbo.taxitemhist t
        ON t.ItemID = d.ItemID
            AND d.datetrans BETWEEN t.datefrom AND isnull(t.dateto, getdate())
        GROUP BY LEFT(d.acctno, 3)
       ) as temp
    GROUP BY temp.branchno
        
    --Getting totals for returns
    INSERT INTO #totReturns
    select temp.branchno, SUM(temp.transValue)
    FROM( 
        SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) as branchno --branchno
               , ABS(SUM((100 * d.transvalue) / (100 + ISNULL(t.SpecialRate, @taxAmount)))) as transValue --transValue
        FROM dbo.delivery d 
        INNER JOIN  dbo.StockInfo s  
            ON d.ItemID = s.ID
        LEFT OUTER JOIN dbo.taxitemhist t
        ON t.ItemID = d.ItemID
            AND d.datetrans BETWEEN t.datefrom AND isnull(t.dateto, getdate())
        WHERE d.delorcoll = 'C'
            AND d.quantity < 0
            AND d.datetrans between @dateFrom and  @dateTo
            AND (s.itemtype = 'S'
                 OR 
                 s.category IN (SELECT code FROM code WHERE category = 'PCDIS'))
        GROUP BY LEFT(d.acctno, 3)
        UNION
        SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) as branchno --branchno
               , ABS(SUM((100 * d.transvalue) / (100 + COALESCE(t.SpecialRate, w.TaxRate ,@taxAmount)))) as transValue --transValue
        FROM dbo.delivery d 
        INNER JOIN  Warranty.Warranty w  
            ON d.itemno = w.Number
        LEFT OUTER JOIN dbo.taxitemhist t
        ON t.ItemID = d.ItemID
            AND d.datetrans BETWEEN t.datefrom AND isnull(t.dateto, getdate())
        WHERE d.delorcoll = 'C'
            AND d.quantity < 0
            AND d.datetrans between @dateFrom and  @dateTo
        GROUP BY LEFT(d.acctno, 3)
        ) as temp
    GROUP BY temp.branchno
        
    --Total sum of repossessions(stock and warranty items)
    INSERT INTO #totRepo
    SELECT temp.branchno, SUM(temp.transValue)
    FROM(
        SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) as branchno --branchno
               , ABS(SUM((100 * d.transvalue) / (100 + ISNULL(t.SpecialRate, @taxAmount)))) as transValue --repos
        FROM dbo.delivery d 
        INNER JOIN  dbo.StockInfo s  
            ON d.ItemID = s.ID 
        LEFT OUTER JOIN dbo.taxitemhist t
        ON t.ItemID = d.ItemID
            AND d.datetrans BETWEEN t.datefrom AND isnull(t.dateto, getdate())
        WHERE d.delorcoll = 'R'
                AND d.quantity < 0
                AND d.datetrans between @dateFrom and @dateTo
                AND (s.itemtype = 'S'
                     OR 
                     s.category IN (SELECT code FROM code WHERE category = 'PCDIS'))
        GROUP BY LEFT(d.acctno, 3)	
        UNION
        SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) as branchno --branchno
               , ABS(SUM((100 * d.transvalue) / (100 + COALESCE(t.SpecialRate, w.TaxRate, @taxAmount)))) as transValue --repos
        FROM dbo.delivery d 
        INNER JOIN  Warranty.Warranty w
            ON d.itemno = w.Number
        LEFT OUTER JOIN dbo.taxitemhist t
        ON t.ItemID = d.ItemID
            AND d.datetrans BETWEEN t.datefrom AND isnull(t.dateto, getdate())
        WHERE d.delorcoll = 'R'
                AND d.quantity < 0
                AND d.datetrans between @dateFrom and @dateTo
        GROUP BY LEFT(d.acctno, 3)	
        ) as temp
    GROUP BY temp.branchno
END
ELSE
BEGIN

    --Getting totals for deliveries
    INSERT INTO #totDeliveries
    SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) -- branchno
           , SUM(d.transvalue) -- transValue
    FROM dbo.delivery d 
    INNER JOIN  dbo.StockInfo s  
        ON d.ItemID = s.ID
            AND d.delorcoll = 'D'
            AND d.datetrans between @dateFrom and  @dateTo
            AND d.quantity > 0
            AND (itemtype = 'S'
                 OR 
                 d.itemno in (select Number from Warranty.Warranty)
                 OR 
                 s.category IN (SELECT code FROM code WHERE category = 'PCDIS')
                )
    GROUP BY LEFT(d.acctno, 3)
        
    --Getting totals for returns
    INSERT INTO #totReturns
    SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) -- branchno
           , ABS(SUM(d.transvalue)) AS transValue
    FROM dbo.delivery d 
    INNER JOIN  dbo.StockInfo s  
        ON d.ItemID = s.ID
    WHERE d.delorcoll = 'C'
        AND d.quantity < 0
        AND d.datetrans between @dateFrom and  @dateTo
        AND (itemtype = 'S'
             OR 
             d.itemno in (select Number from Warranty.Warranty)
             OR 
             s.category IN (SELECT code FROM code WHERE category = 'PCDIS')
            )
    GROUP BY LEFT(d.acctno, 3)

    --Total sum of repossessions 
    INSERT INTO #totRepo
    SELECT CAST(LEFT(d.acctno, 3) AS SMALLINT) -- branchno
           , ABS(SUM(d.transvalue)) -- repos
    FROM dbo.delivery d 
    INNER JOIN  dbo.StockInfo s  
        ON d.ItemID = s.ID 
    WHERE d.delorcoll = 'R'
            AND d.quantity < 0
            AND d.datetrans between @dateFrom and @dateTo
            AND (itemtype = 'S'
                 OR 
                 d.itemno in (select Number from Warranty.Warranty)
                 OR 
                 s.category IN (SELECT code FROM code WHERE category = 'PCDIS')
                )
    GROUP BY LEFT(d.acctno, 3)	

END		
        
--Total BDW		
SELECT LEFT(f.acctno, 3) AS branchno
       , ABS(SUM(f.transvalue)) AS BDWTotal
INTO #totBDW	
FROM dbo.fintrans f 
WHERE f.transtypecode in ('BDW', 'BDR')
    AND RIGHT(LEFT(f.acctno, 4), 1) = '0'
    AND f.datetrans between @datefrom and @dateTo
GROUP BY LEFT(f.acctno, 3)

--Getting Maximum fintrans date linked to all accounts considered 
SELECT f.acctno,
       max(f.datetrans) as maxDateTrans
INTO #finMaxDate
FROM fintrans f
INNER JOIN agreement ag
ON f.acctno = ag.acctno
    AND ag.datedel BETWEEN @dateFrom and @dateTo
    AND RIGHT(LEFT(f.acctno, 4), 1) = '0'
GROUP BY f.acctno

--Total Loans
SELECT LEFT(am.acctno, 3) as branchno,
       SUM(am.outstbal) as totOutstBal
INTO #totLoans
FROM dbo.accountmonths2 am
inner join CashLoan cl
on cl.AcctNo = am.acctno
inner join acct a
on a.acctno = am.acctno
inner join #finMaxDate fin
on a.acctno = fin.acctno
WHERE am.Currentmonth BETWEEN CASE 
                                  WHEN a.currstatus = 'S' AND a.dateacctopen > @dateFrom THEN (DATEADD(day, -datepart(day, a.dateacctopen), a.dateacctopen)) 
                                  ELSE @dateFrom 
                              END 
                              AND
                              CASE 
                                  WHEN a.currstatus = 'S' AND fin.maxDateTrans < DATEADD(MONTH, 11, @dateFrom) THEN (DATEADD(day, -datepart(day, fin.maxDateTrans), fin.maxDateTrans)) 
                                  ELSE DATEADD(MONTH, 11, @dateFrom) 
                              END
GROUP BY LEFT(am.acctno, 3)

--Total Portfolio Balance By Branch
SELECT LEFT(am.acctno, 3) as branchno,
       SUM(am.outstbal) as totOutstBal
INTO #portfolioBalance
FROM dbo.accountmonths2 am
inner join acct a
on a.acctno = am.acctno
inner join #finMaxDate fin
on a.acctno = fin.acctno
WHERE am.Currentmonth BETWEEN CASE 
                                  WHEN a.currstatus = 'S' AND a.dateacctopen > @dateFrom THEN (DATEADD(day, -datepart(day, a.dateacctopen), a.dateacctopen)) 
                                  ELSE @dateFrom 
                              END 
                              AND
                              CASE 
                                  WHEN a.currstatus = 'S' AND fin.maxDateTrans < DATEADD(MONTH, 11, @dateFrom) THEN (DATEADD(day, -datepart(day, fin.maxDateTrans), fin.maxDateTrans)) 
                                  ELSE DATEADD(MONTH, 11, @dateFrom) 
                              END
GROUP BY LEFT(am.acctno, 3)

--Outstanding balances
SELECT @totOutstBal = SUM(am.outstbal)
FROM dbo.accountmonths2 am
WHERE SUBSTRING(am.acctno, 4, 1) = '0'
    AND am.Currentmonth = (SELECT MAX(Currentmonth) FROM dbo.accountmonths2 a WHERE am.acctno = a.acctno AND a.Currentmonth >= @datefrom AND a.Currentmonth <= DATEADD(MONTH, 11, @dateFrom) AND a.currstatus != 'S')

--Getting Data for Extract

SELECT * INTO ##tempExport FROM
(
    SELECT 'ent_Entidad' AS ent_Entidad, 
           'cda_CDA' AS cda_CDA, 
           'cdc_CDC' AS cdc_CDC,
           'cyc_FinAnio' AS cyc_FinAnio,
           'cyc_PorcDevoluciones' AS cyc_PorcDevoluciones, 
           'cyc_PorcDepreciacion' AS cyc_PorcDepreciacion,
           'cyc_PorcCxD' AS cyc_PorcCxD, 
           'cyc_PorcAjustes' AS cyc_PorcAjustes, 
           'cyc_PorcImpSInteres' AS cyc_PorcImpSInteres, 
           'cyc_PorcCashCredito' AS cyc_PorcCashCredito
    UNION ALL
    SELECT ISNULL(LTRIM(RTRIM(c.ISOCountryCode)) + LTRIM(RTRIM(b.StoreType)), ' ') AS ent_Entidad
           , CAST(ISNULL(b.branchno, ' ') AS VARCHAR) AS cda_CDA
           , '0000' AS cdc_CDC
           , CAST(ISNULL(CASE
                         WHEN DATEPART(MONTH, @dateTo) < 4 THEN DATEPART(YEAR, @dateTo)		-- #12859
                         ELSE DATEPART(YEAR, @dateTo) + 1						-- #12859
                         END, '') AS VARCHAR) AS cyc_FinAnio
           , CAST(((ISNULL(tRet.transValue, 0) + ISNULL(tR.repos, 0))/CAST(ISNULL(NULLIF(tDel.transValue, 0), 1)  as DECIMAL(25,5))) AS VARCHAR) AS cyc_PorcDevoluciones
           , CAST(tB.BDWTotal/CAST(ISNULL(NULLIF(@totOutstBal, 0), 1) as DECIMAL(25,5)) AS VARCHAR) AS cyc_PorcDepreciacion
           , CAST((tR.repos/CAST(ISNULL(NULLIF(pb.totOutstBal, 0), 1) as DECIMAL(25,5))) AS VARCHAR) AS cyc_PorcCxD
           , cast(0 as varchar) AS cyc_PorcAjustes
           , CAST(0 AS VARCHAR) AS cyc_PorcImpSInteres
           , CAST(tl.totOutstBal/CAST(ISNULL(NULLIF(pb.totOutstBal, 0), 1) as DECIMAL(25,5)) AS VARCHAR) AS cyc_PorcCashCredito
    FROM dbo.country c, 
         dbo.branch b 
         LEFT OUTER JOIN #portfolioBalance pb
         ON b.branchno = pb.branchno
         LEFT OUTER JOIN #totDeliveries tDel
         ON b.branchno = tDel.branchno
         LEFT OUTER JOIN #totReturns tRet
         ON b.branchno = tRet.branchno
         LEFT OUTER JOIN #totBDW tB
         ON b.branchno = tB.branchno 
         LEFT OUTER JOIN #totRepo tR
         ON b.branchno = tR.branchno		 	 
         LEFT OUTER JOIN #totLoans tL
         ON b.branchno = tL.branchno
         
) AS tmp
     
EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport
GO
     
--Testing
--DROP TABLE #totDeliveries
--DROP TABLE #totReturns
--DROP TABLE #totBDW
--DROP TABLE #totRepo
--DROP TABLE #totLoans 
--DROP TABLE #totAgreementCl