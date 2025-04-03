SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_cyc_plazos_cartera') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_cyc_plazos_cartera
END
GO

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 17 August 2012
-- Description:	Hyperion Extract files
--
-- 12/04/13 jec #12859 - UAT12602 
-- =============================================

CREATE PROCEDURE dbo.t_src_cyc_plazos_cartera
(
	@dateTo DATETIME
)

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100),
		@term12Total float
		
SET @filename = 't_src_cyc_plazos_cartera.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\100\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'		-- #12859

--Testing
--SET @dateTo = '2012-07-01'

--Get all accounts considered in a temp table
SELECT ag.acctno
INTO #tempAccts 
FROM dbo.agreement ag
INNER JOIN dbo.ACCOUNTMONTHS2 am
	ON ag.acctno = am.acctno 
		AND am.Currentmonth = dateadd(month, -1, @dateTo)
WHERE ag.datedel BETWEEN DATEADD(month, -1, @dateTo) AND DATEADD(second, -1, @dateTo)
	  AND RIGHT(LEFT(ag.acctno, 4), 1) = '0'	
	  AND am.outstbal > 0
	  AND am.currstatus NOT IN ('S', '9')


---------------------------------------
-- Total Credit accounts
---------------------------------------

SELECT b.storetype,
       CASE 
		   WHEN l.acctno IS NULL THEN 'Merchandise' 
		   ELSE 'Cash'
	   END AS product,
	   COUNT(DISTINCT ta.acctno) AS totalCredits		
INTO #totals
from #tempAccts ta 
INNER JOIN dbo.branch b
	 ON LEFT(ta.acctno, 3) = b.branchno
LEFT OUTER JOIN cashloan l ON l.acctno = ta.acctno	 
GROUP BY b.storetype, CASE 
		                  WHEN l.acctno IS NULL THEN 'Merchandise' 
		                  ELSE 'Cash'
	                  END

PRINT 'Finished Temp accts'		


--Get interest Rates for accounts considered
-- if scoring band on proposal is null than take the customer band
SELECT a.acctno
	   , (ih.intrate) AS intrate
	   , CASE
		 WHEN p.ScoringBand IS NULL THEN c.scoringBand	
		 ELSE p.ScoringBand 
		 END AS ScoringBand
	   , a.termstype
       ,a.agrmttotal
       ,i.instalno
INTO #intrates
FROM dbo.acct a 
	 INNER JOIN #tempAccts ta
	 ON a.acctno = ta.acctno
	 INNER JOIN dbo.custacct ca 
	 ON ca.acctno = ta.acctno
		AND ca.hldorjnt = 'H'
	 INNER JOIN dbo.customer c
	 ON c.custid = ca.custid
		 INNER JOIN dbo.proposal p
	 ON p.acctno = ta.acctno
		AND p.custid = ca.custid
		AND p.dateprop = (SELECT MAX(dateprop) FROM proposal WHERE acctno = p.acctno AND custid = p.custid)
	 INNER JOIN dbo.instalplan i 
	 ON ta.acctno = i.acctno
	 LEFT OUTER JOIN dbo.intratehistory ih
	 ON a.termstype = ih.termstype 
		AND ISNULL(p.ScoringBand, c.scoringBand) = ih.Band
		AND a.dateacctopen BETWEEN ih.datefrom AND isnull(nullif(ih.dateto, '1900-01-01 00:00:00'), getdate())

--if interest rate was not set based on the date the account was open take the last revise date of the agreement
IF EXISTS (SELECT 'a' FROM #intrates WHERE intrate IS NULL)
BEGIN	

	UPDATE i 
		SET i.intrate = (ih.intrate)
	FROM #intrates i 
	INNER JOIN dbo.RevisedHist rh
	ON rh.acctno = i.acctno 
		AND rh.dateagrmtrevised = (SELECT MAX(DateAgrmtRevised) FROM dbo.revisedhist WHERE acctno = rh.acctno)
	INNER JOIN intratehistory ih
	ON i.termstype = ih.termstype 
		AND i.ScoringBand = ih.Band 
		AND rh.dateagrmtrevised BETWEEN ih.datefrom AND isnull(nullif(ih.dateto, '1900-01-01 00:00:00'), getdate())
	WHERE i.intrate IS NULL
END	

--if interest rate was not set based on the last revise date of the agreement, take the most recent one
IF EXISTS (SELECT 'a' FROM #intrates WHERE intrate IS NULL)
BEGIN	

	UPDATE i 
		SET i.intrate = (ih.intrate)
	FROM #intrates i 
	INNER JOIN intratehistory ih
	ON i.termstype = ih.termstype 
		AND ih.datefrom = (SELECT MAX(datefrom) FROM dbo.intratehistory WHERE termstype = i.termstype AND band = i.ScoringBand AND datefrom < GETDATE())
		AND i.ScoringBand = ih.Band 
	WHERE i.intrate IS NULL
	
END	
	
--if interest rate was not set in any of the previous statements set it to 0
IF EXISTS (SELECT 'a' FROM #intrates WHERE intrate IS NULL)
BEGIN	

	UPDATE #intrates 
	SET intrate = 0
	WHERE intrate IS NULL
	
END		  
			
PRINT 'Done Interest Rates'



--Get all terms for loan calculations	
SELECT b.StoreType,
       CASE 
			WHEN c.acctno IS NULL THEN 'Merchandise' 
			ELSE 'Cash'
	   END AS product
	   , ir.instalno AS instalno
	   , SUM(ir.instalno) AS instalnoSum
       , SUM(ir.agrmttotal) AS agreementTotalSum
       , SUM(ir.intrate) AS intRateSum
	   , COUNT(DISTINCT ir.acctno) AS totalCreditAccts
INTO #data				
FROM #intrates ir 
     INNER JOIN dbo.branch b
	 ON b.branchno = LEFT(ir.acctno, 3)
     left outer JOIN dbo.CashLoan c 
	 ON ir.acctno = c.acctno
GROUP BY b.StoreType, ir.instalno, CASE 
						               WHEN c.acctno IS NULL THEN 'Merchandise' 
						               ELSE 'Cash'
					               END
	
PRINT 'Done Loan Calculations'	


--get data for extract
set @dateTo = DATEADD(second, -1, @dateTo)

SELECT * INTO ##tempExport FROM
(
	SELECT 'ent_Entidad' AS ent_Entidad, 
           'pzc_Calanio' AS pzc_Calanio,
           'pzc_FinAnio' AS pzc_FinAnio, 
           'pzc_Producto' AS pzc_Producto, 
	       'pzc_Plazo' AS pzc_Plazo, 
           'pzc_TasaPP' AS pzc_TasaPP, 
           'pzc_PorcComposicion' AS pzc_PorcComposicion
	UNION ALL
	SELECT DISTINCT ISNULL((LTRIM(RTRIM(c.ISOCountryCode)) + LTRIM(RTRIM(t.StoreType))), ' ') AS ent_Entidad
           , CAST(DATEPART(YEAR, @dateTo) AS VARCHAR) AS pzc_Calanio
		   , CAST(CASE
				  WHEN DATEPART(MONTH, @dateTo) < 4 THEN DATEPART(YEAR, @dateTo)
				  ELSE DATEPART(YEAR, @dateTo) + 1
				  END AS varchar) AS pzc_FinAnio
		   , d.product AS pzc_Producto
		   , CAST(ISNULL(d.instalno, 0) AS VARCHAR) AS pzc_Plazo
		   , CAST( 
                  (ISNULL(d.instalnoSum, 0) * ISNULL(d.agreementTotalSum, 0) * ISNULL(d.intRateSum, 0)) /
                  (CAST(ISNULL(NULLIF(d.instalnoSum, 0), 1) AS DECIMAL(25,5)) * CAST(ISNULL(NULLIF(d.agreementTotalSum, 0), 1) AS DECIMAL(25,5))) /
                  d.totalCreditAccts AS VARCHAR
                 ) AS pzc_TasaPP
		   , CAST((d.totalCreditAccts/CAST(ISNULL(NULLIF(t.totalCredits, 0), 1) AS DECIMAL(25,5))) AS VARCHAR) as pzc_PorcComposicion
	FROM dbo.country c
		 , #totals t
         INNER JOIN #data d 
         ON d.storetype = t.storetype
            AND d.product = t.product
) AS tmp

EXEC master..xp_cmdshell @bcpCommand

IF OBJECT_ID('tempdb..##tempExport') IS NOT NULL
    DROP TABLE ##tempExport
GO


--Testing
--DROP TABLE #intrates
--DROP TABLE #tempAccts
--DROP TABLE #totals
--DROP TABLE #data

--SELECT * FROM ##tempExport