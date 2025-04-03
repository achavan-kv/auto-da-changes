SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_tipo_producto') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_tipo_producto
END
GO

CREATE PROCEDURE dbo.t_src_tipo_producto

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 30 June 2014
-- Description:	Hyperion Extract files
-- =============================================
(
	@dateTo DATETIME
)

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100),
        @adminitem VARCHAR(10),
        @insitem VARCHAR(10),
        @noninterest VARCHAR(2),
        @deliveryThreshold INT

SELECT  @noninterest = noninterestitem,
        @adminitem = adminitemno,
        @insitem = insitemno,
        @deliveryThreshold = ISNULL(globdelpcent, 75)
FROM    country

SET @filename = 't_src_tipo_producto.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\100\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'		

--drop temporary table if it exists
IF OBJECT_ID('tempdb..##tempExport') IS NOT NULL
    DROP TABLE ##tempExport


--getting all data for extract to siplify final query
;WITH accounts (acctno, accttype, agrementTotal, serviceAmount, outstandingBalance)
AS
(
    SELECT a.acctno, 
           a.accttype,
           COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) AS agrementTotal,                                    -- most up to date agreement total based on the time the export is run
           COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) - (la.ValueAfter - la.ValueBefore) AS serviceAmount,  -- service charge value - agreement total - cash price of the items
           ISNULL(fin.transvalue, 0)
    FROM acct a
    INNER JOIN agreement ag
        ON ag.acctno = a.acctno
    --outstanding balance on an account
    LEFT OUTER JOIN (SELECT acctno, SUM(transvalue) AS transvalue
                     FROM fintrans 
                     WHERE datetrans < @dateTo
                     GROUP BY acctno) AS fin
        ON a.acctno = fin.acctno
    --Delivered amount on the account
    LEFT OUTER JOIN (SELECT acctno, SUM(transvalue) as deliveredAmount
                     FROM delivery
                     WHERE datetrans < @dateTo
                     GROUP BY acctno) as d
    ON d.acctno = a.acctno
    --priciple part of the agreement/cash price of goods
    INNER JOIN (SELECT la.acctno, SUM(la.ValueBefore) AS ValueBefore, SUM(la.ValueAfter) AS ValueAfter
                FROM LineitemAudit la
                INNER JOIN StockInfo si
                    ON la.ItemID = si.Id
                WHERE Datechange <= @dateTo
                    AND la.ItemID NOT IN (SELECT DISTINCT k.ItemID from kitproduct k) 
                    AND si.IUPC NOT IN('DT', @adminitem, @insitem)
                    AND (si.IUPC NOT LIKE @noninterest + '%' OR @noninterest = '')
                GROUP BY la.acctno
                ) AS la 
    ON la.acctno = a.acctno
    LEFT OUTER JOIN instalplan i
    ON i.acctno = ag.acctno
    LEFT OUTER JOIN agreementAudit aa
        ON a.acctno = aa.acctno
        AND aa.datechange = (SELECT MAX(datechange) from agreementAudit where acctno = aa.acctno and aa.datechange < @dateTo)
    LEFT OUTER JOIN instalplanAudit ia
        ON ia.acctno = ag.acctno
        AND ia.datechange = (SELECT MAX(datechange) from instalplanAudit where acctno = ia.acctno and ia.datechange < @dateTo)
    WHERE a.dateacctopen <= @dateTo
        AND a.accttype != 'S'
        AND ISNULL(fin.transvalue, 0) > 0
        AND @deliveryThreshold * COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) / 100 <= d.deliveredAmount 
        AND (a.accttype = 'C' OR COALESCE(ia.Newinstalment, i.instalamount, 0) != 0)
        AND (a.accttype = 'C' OR COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) != 0)
)


--Getting export data as per the specifications
--Insert warranty value lines for all accounts
SELECT c.ISOCountryCode + b.StoreType AS ENT_ENTIDAD,
       CAST(b.branchno AS VARCHAR) AS CDA_CDA,
       a.acctno AS VENTA_ID,
       CASE
           WHEN ca.AcctNo IS NOT NULL THEN 'CASH'
           ELSE 'MERCHANDISE'
       END AS TIPO_PRODUCTO,
       CAST('WARRANTY' AS VARCHAR(13)) AS PRODUCTO,
       ISNULL(war.warrantyAmount, 0) AS MONTO_NETO,
       UPPER(LEFT(DATENAME(MONTH, DATEADD(DAY, -1, @dateTo)), 3)) + '-' + RIGHT(DATEPART(YEAR, DATEADD(DAY, -1, @dateTo)), 2) AS PERIODO,
       a.serviceAmount,
       a.agrementTotal,
       a.accttype,
       ISNULL(war.warrantyAmount, 0) as warrantyAmount
INTO #export
FROM country c,
     accounts a
     INNER JOIN branch b
     ON b.branchno = CAST(LEFT(a.acctno, 3) AS SMALLINT)
     INNER JOIN custacct cAcct
     ON cAcct.acctno = a.acctno
        AND cAcct.hldorjnt = 'H'
     LEFT OUTER JOIN CashLoan ca
     ON a.acctno = ca.AcctNo
        AND ca.LoanStatus = 'D'
        AND ca.Custid = cAcct.custid
     --Value of delivered warranties
     LEFT OUTER JOIN (SELECT la.acctno, SUM(la.ValueAfter) - SUM(la.ValueBefore) as warrantyAmount
                      FROM LineitemAudit la
                      WHERE la.Datechange < @dateTo
                         AND la.itemno IN (SELECT DISTINCT Number FROM Warranty.Warranty)
                      GROUP BY la.acctno) as war
     ON war.acctno = a.acctno

--Insert the goods/merchandise value for all accounts
INSERT INTO #export
SELECT e.ENT_ENTIDAD,
       e.CDA_CDA,
       e.VENTA_ID,
       e.TIPO_PRODUCTO,
       'MERCHANDISE' AS PRODUCTO,
       CASE
           WHEN e.accttype = 'C' THEN e.agrementTotal - e.warrantyAmount
           ELSE e.agrementTotal - e.serviceAmount - e.warrantyAmount
       END AS MONTO_NETO,
       e.PERIODO,
       e.serviceAmount,
       e.agrementTotal,
       e.accttype,
       warrantyAmount
FROM #export e
WHERE e.PRODUCTO = 'WARRANTY'

--final select and file export
SELECT ENT_ENTIDAD,
       CDA_CDA,
       VENTA_ID,
       TIPO_PRODUCTO,
       PRODUCTO,
       MONTO_NETO,
       PERIODO 
INTO ##tempExport FROM
(
		SELECT 1 AS orderFlag, 
               'ENT_ENTIDAD' AS ENT_ENTIDAD, 
               'CDA_CDA' AS CDA_CDA, 
               'VENTA_ID' AS VENTA_ID,
               'TIPO_PRODUCTO' AS TIPO_PRODUCTO,
               'PRODUCTO' AS PRODUCTO,
               'MONTO_NETO' AS MONTO_NETO,
               'PERIODO' AS PERIODO
		UNION ALL
		SELECT 2 AS orderFlag, 
               ENT_ENTIDAD, 
               CDA_CDA, 
               VENTA_ID,
               TIPO_PRODUCTO,
               PRODUCTO,
               CAST(MONTO_NETO AS VARCHAR),
               PERIODO
        FROM #export
) AS tmp
ORDER BY 
    orderFlag, VENTA_ID

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport

GO