SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_cyc_vigente') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_cyc_vigente
END
GO

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 17 August 2012
-- Last date updated: 21 November 2012
-- Description:	Hyperion Extract files
--
-- 12/04/13 jec #12859 - UAT12602 
-- =============================================

CREATE PROCEDURE dbo.t_src_cyc_vigente
(
	@dateTo DATETIME
)

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100)
		
SET @filename = 't_src_cyc_vigente.txt'		
		
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

;with tempaccts as(

SELECT am.acctno, am.outstbal AS outstBal
FROM  dbo.accountmonths2 am
WHERE am.currentMonth = dateadd(month, -1, @dateTo) 
    AND am.outstbal > 0
	AND am.currstatus NOT IN ('S', '9')
	AND SUBSTRING(am.acctno, 4, 1) = '0'	)


    -------------------------------------
    --Should we exclude -ve balance
    -------------------------------------

--Get interest Rates for accounts considered
-- if scoring band on proposal is null than take the customer band
SELECT a.acctno
	   , (ih.intrate / 12) AS intrate
	   , CASE 
			 WHEN i.instalno - (DATEDIFF(MONTH, i.datefirst, DATEADD(SECOND, -1, @dateTo))) < 0 THEN 0
			 ELSE i.instalno - (DATEDIFF(MONTH, i.datefirst, DATEADD(SECOND, -1, @dateTo)))
		 End AS remainingTerm
	   , CASE
		 WHEN p.ScoringBand IS NULL THEN c.scoringBand	
		 ELSE p.ScoringBand 
		 END AS ScoringBand
	   , a.termstype
       ,ta.outstbal
INTO #intrates
FROM tempAccts ta
	 INNER JOIN dbo.acct a 
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
		SET i.intrate = (ih.intrate / 12)
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
		SET i.intrate = (ih.intrate / 12)
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
		
--Get all terms for merchandise calculations by branch		
SELECT LEFT(ir.acctno, 3) AS branch
	   , ir.remainingTerm
	   , SUM(ir.remainingTerm * ir.outstBal * ir.intrate) AS term11	
	   , SUM(ir.remainingTerm * ir.outstBal) AS term12
	   , SUM(ISNULL(ir.outstBal, 0)) AS outstBalSum
INTO #merchandiseBranch			
FROM #intrates ir
WHERE NOT EXISTS(SELECT 'a' FROM dbo.CashLoan c WHERE c.acctno = ir.acctno)
GROUP BY LEFT(ir.acctno, 3), ir.remainingTerm

PRINT 'Done Merchandise Branch Calculations'

--Get all terms for loan calculations by branch		
SELECT LEFT(ir.acctno, 3) AS branch
	   , ir.remainingTerm
	   , SUM(ir.remainingTerm * ir.outstBal * ir.intrate) AS term11		
	   , SUM(ir.remainingTerm * ir.outstBal) AS term12
	   , SUM(ISNULL(ir.outstBal, 0)) AS outstBalSum
INTO #loanBranch			
FROM  #intrates ir
	 INNER JOIN dbo.CashLoan cl
	 ON cl.AcctNo = ir.acctno
GROUP BY LEFT(ir.acctno, 3), ir.remainingTerm

PRINT 'Done Loan Branch Calculations'

set @dateTo = DATEADD(second, -1, @dateTo)

--Retrieving data for export
SELECT * INTO ##tempExport FROM
(
	SELECT 'ent_Entidad' AS ent_Entidad,
           'cad_Cadena' AS cad_Cadena, 
           'cvi_CDA' AS cvi_CDA, 
           'cvi_TipoProducto' AS cvi_TipoProducto,
           'cvi_Calanio' AS cvi_Calanio,
           'cvi_FinAnio' AS cvi_FinAnio, 
           'cvi_Plazo' AS cvi_Plazo,
		   'cvi_TasaPonderada' AS cvi_TasaPonderada, 
           'cvi_Monto' AS cvi_Monto
	UNION ALL
	SELECT DISTINCT c.ISOCountryCode AS ent_Entidad
           , b.StoreType AS cad_Cadena
		   , CAST(b.branchno AS VARCHAR) AS cvi_CDA
		   , 'Merchandise' AS cvi_TipoProducto
           , CAST(DATEPART(YEAR, @dateTo) AS VARCHAR) AS cvi_Calanio
		   , CAST(CASE
				  WHEN DATEPART(MONTH, @dateTo) < 4 THEN DATEPART(YEAR, @dateTo)
				  ELSE DATEPART(YEAR, @dateTo) + 1
				  END AS varchar) AS cvi_FinAnio
		   , CAST(ISNULL(mb.remainingTerm, 0) AS VARCHAR) AS cvi_Plazo
		   , CAST(ISNULL(mb.term11, 0)/CAST(ISNULL(NULLIF(mb.term12, 0), 1) AS DECIMAL(25,5)) AS VARCHAR) AS cvi_TasaPonderada	  
		   , CAST(ISNULL(mb.outstBalSum, 0) AS VARCHAR) AS cvi_Monto
	FROM dbo.country c
		 , dbo.branch b LEFT OUTER JOIN #merchandiseBranch mB
		 ON b.branchno = mB.branch
	UNION ALL
	SELECT DISTINCT c.ISOCountryCode AS ent_Entidad
           , b.StoreType AS cad_Cadena
		   , CAST(b.branchno AS VARCHAR) AS cvi_CDA
		   , 'Cash' AS cvi_TipoProducto
           , CAST(DATEPART(YEAR, @dateTo) AS VARCHAR) AS cvi_Calanio
		   , CAST(CASE
				  WHEN DATEPART(MONTH, @dateTo) < 4 THEN DATEPART(YEAR, @dateTo)
				  ELSE DATEPART(YEAR, @dateTo) + 1
				  END AS VARCHAR) AS cvi_FinAnio
		   , CAST(ISNULL(lb.remainingTerm, 0) AS VARCHAR) AS cvi_Plazo
		   , CAST(ISNULL(lb.term11, 0)/CAST(ISNULL(NULLIF(lb.term12, 0), 1) AS DECIMAL(25,5)) AS VARCHAR) AS cvi_TasaPonderada
		   , CAST(ISNULL(lb.outstBalSum, 0) AS VARCHAR) AS cvi_Monto
	FROM dbo.country c
		 , dbo.branch b LEFT OUTER JOIN #loanBranch lB
		 ON b.branchno = lB.branch
) AS tmp

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport
DROP TABLE #intrates
DROP TABLE #loanBranch
DROP TABLE #merchandiseBranch
GO