SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_sysde_claseprod') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_sysde_claseprod
END
GO

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 17 August 2012
-- Description:	Hyperion Extract files
--
-- 12/04/13 jec #12859 - UAT12602 
-- =============================================

CREATE PROCEDURE dbo.t_src_sysde_claseprod
(
	@dateFrom DATETIME
	, @dateTo DATETIME
)

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100)
		
set @dateto = dateadd(second, -1, @dateto)
SET @filename = 't_src_sysde_claseprod.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\110\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'			-- #12859

--Total deliveries by class and branch
SELECT LEFT(d.acctno, 3) AS branchno
	   , s.category
	   , SUM(d.quantity) AS classQuantity
	   , ISNULL(DATEPART(YEAR, d.datetrans), 0) AS [Year]
	   , ISNULL(LEFT(DATENAME(MONTH, d.datetrans), 3), '') AS [Month]
	   , CASE
		 WHEN DATEPART(MONTH, d.datetrans) < 4 THEN DATEPART(YEAR, d.datetrans)
	     ELSE DATEPART(YEAR, d.datetrans) + 1
	     END AS finYear	
INTO #classQuantities
FROM dbo.delivery d 
INNER JOIN dbo.StockInfo s 
	ON d.ItemID = s.ID 
WHERE d.datetrans between @dateFrom and @dateTo
    AND s.itemtype = 'S'
GROUP BY LEFT(d.acctno, 3), s.category, LEFT(DATENAME(MONTH, d.datetrans), 3), DATEPART(YEAR, d.datetrans), CASE
																										    WHEN DATEPART(MONTH, d.datetrans) < 4 THEN DATEPART(YEAR, d.datetrans)
																										    ELSE DATEPART(YEAR, d.datetrans) + 1
																										    END

--Total deliveried by branch																										 
SELECT LEFT(d.acctno, 3) AS branchno
	   , ISNULL(SUM(d.quantity), 0) AS totQuantity
	   , ISNULL(DATEPART(YEAR, d.datetrans), 0) AS [Year]
	   , ISNULL(LEFT(DATENAME(MONTH, d.datetrans), 3), '') AS [Month]	
	   , CASE
		 WHEN DATEPART(MONTH, d.datetrans) < 4 THEN DATEPART(YEAR, d.datetrans)
		 ELSE DATEPART(YEAR, d.datetrans) + 1
		 END AS finYear	
INTO #totalQuantities
FROM dbo.delivery d 
INNER JOIN dbo.StockInfo s 
	ON d.ItemID = s.ID
WHERE d.datetrans between @datefrom and @dateTo
    AND s.itemtype = 'S'
GROUP BY LEFT(d.acctno, 3), LEFT(DATENAME(MONTH, d.datetrans), 3), DATEPART(YEAR, d.datetrans), CASE
																								WHEN DATEPART(MONTH, d.datetrans) < 4 THEN DATEPART(YEAR, d.datetrans)
																								ELSE DATEPART(YEAR, d.datetrans) + 1
																								END



SELECT ent_Entidad,
	   cda_CDA, 
	   cdc_CDC, 
	   sde_CalAnio, 
	   sde_Periodo, 
	   clp_ClaseProducto, 
	   sde_FinAnio, 
	   sde_Porc_Participacion
 INTO ##tempExport FROM
(
SELECT 1 AS orderFlag,
       CAST('ent_Entidad' AS VARCHAR(MAX)) AS ent_Entidad,
	   CAST('cda_CDA' AS VARCHAR(MAX)) AS cda_CDA, 
	   CAST('cdc_CDC' AS VARCHAR(7)) AS cdc_CDC, 
	   CAST('sde_CalAnio' AS VARCHAR(MAX)) AS sde_CalAnio, 
	   CAST('sde_Periodo' AS VARCHAR(MAX)) AS sde_Periodo, 
	   CAST('clp_ClaseProducto' AS VARCHAR(MAX)) AS clp_ClaseProducto, 
	   CAST('sde_FinAnio' AS VARCHAR(MAX)) AS sde_FinAnio, 
	   CAST('sde_Porc_Participacion' AS VARCHAR(MAX)) AS sde_Porc_Participacion
		UNION ALL
SELECT 2 AS orderFlag,
       ISNULL((LTRIM(RTRIM(c.ISOCountryCode)) + LTRIM(RTRIM(b.StoreType))) , ' ') AS ent_Entidad,
	   ISNULL(cQ.branchno, ' ') AS cda_CDA, 
	   '0000' AS cdc_CDC, 
	   ISNULL(CAST(cQ.[Year] AS VARCHAR), ' ') AS sde_CalAnio, 
	   ISNULL(cQ.[Month], ' ') AS sde_Periodo, 
	   ISNULL(cast(cQ.category as varchar), ' ') AS clp_ClaseProducto, 
	   ISNULL(CAST(cQ.finYear AS VARCHAR), ' ') AS sde_FinAnio,
	   CAST(cQ.classQuantity/CAST(isnull(nullif(tq.totQuantity, 0), 1) AS DECIMAL(25,5)) AS VARCHAR) AS sde_Porc_Participacion
FROM dbo.country c,
		dbo.branch b 
		INNER JOIN #classQuantities AS cQ
		ON b.branchno = cQ.branchno
		INNER JOIN #totalQuantities AS tQ
		ON cQ.[Year] = tQ.[Year]
		AND cq.[Month] = tq.[Month]
		AND cq.finYear = tq.finYear
		AND cq.branchno = tq.branchno
) AS tmp	
ORDER BY orderFlag

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport
GO

--Testing
--DROP TABLE #classQuantities
--DROP TABLE #totalQuantities

