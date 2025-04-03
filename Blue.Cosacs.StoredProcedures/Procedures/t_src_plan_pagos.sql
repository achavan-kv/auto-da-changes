SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_plan_pagos') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_plan_pagos
END
GO

CREATE PROCEDURE dbo.t_src_plan_pagos

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 26 June 2014
-- Description:	Hyperion Extract files

-- Formula to get the service charge percentage applied on an account
-- Service Percentage = (100 * Service Charge Amount * 12) / ((Agreement Total - Service Charge - Deposit) * Number Of Installments)

-- Formula for priciple part of instalment amount
-- Priciple part of instalment amount = 100 * Instalment Amount / (100 + Service Percentage * Number Of Installments / 12) -- number of installments / 12 because this value is a yearly service percentage
-- =============================================
(
	@dateFrom DATETIME
)

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100),
        @adminitem VARCHAR(10),
        @insitem VARCHAR(10),
        @noninterest VARCHAR(2),
        @deliveryThreshold INT,
        @tempDate DATETIME

SELECT  @noninterest = noninterestitem,
        @adminitem = adminitemno,
        @insitem = insitemno,
        @deliveryThreshold = ISNULL(globdelpcent, 75)
FROM    country

set @dateFrom = DATEADD(MONTH, 1, @dateFrom)

--Used in the loop for future data
SET @tempDate = @dateFrom

SET @filename = 't_src_plan_pagos.txt'		
		
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

BEGIN TRY
    --getting all data for extract to simplify final query
    SELECT a.acctno, 
            a.accttype, 
            COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) AS agrementTotal,
            COALESCE(aa.Newdeposit, ag.deposit, 0) AS depositAmount,
            COALESCE(ia.Newinstalno, i.instalno, 0) AS instalmentNumber,
            COALESCE(ia.Newinstalment, i.instalamount, 0) AS instalmentAmount,
            la.ValueAfter - la.ValueBefore AS itemsValue,
            COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) - (la.ValueAfter - la.ValueBefore) AS serviceCharge,
            i.dueday AS dueDay,
            CAST(i.datelast AS DATE) AS lastInstalment,
            CAST(i.[datefirst] AS DATE) AS firstInstalment,
            fin.outstandingBalance
    INTO #accounts
    FROM acct a
    INNER JOIN agreement ag
    ON ag.acctno = a.acctno
    INNER JOIN (SELECT acctno, SUM(transvalue) AS outstandingBalance
                FROM fintrans 
                WHERE datetrans < @dateFrom
                GROUP BY acctno
                ) AS fin
    ON a.acctno = fin.acctno
    --priciple part of the agreement
    INNER JOIN (SELECT la.acctno, SUM(la.ValueBefore) AS ValueBefore, SUM(la.ValueAfter) AS ValueAfter
                FROM LineitemAudit la
                INNER JOIN StockInfo si
                    ON la.ItemID = si.Id
                WHERE Datechange < @dateFrom
                    AND la.ItemID NOT IN (SELECT DISTINCT k.ItemID from kitproduct k) 
                    AND si.IUPC NOT IN('DT', @adminitem, @insitem)
                    AND (si.IUPC NOT LIKE @noninterest + '%' OR @noninterest = '')
                GROUP BY la.acctno
                ) AS la 
    ON la.acctno = a.acctno
    --Delivered amount on the account
    INNER JOIN (SELECT acctno, SUM(transvalue) as deliveredAmount
                FROM delivery
                WHERE datetrans < @dateFrom
                GROUP BY acctno
                ) as d
    ON d.acctno = a.acctno
    LEFT OUTER JOIN instalplan i
    ON i.acctno = ag.acctno
    LEFT OUTER JOIN agreementAudit aa
        ON a.acctno = aa.acctno
        AND aa.datechange = (SELECT MAX(datechange) from agreementAudit where acctno = aa.acctno and datechange < @dateFrom)
    LEFT OUTER JOIN instalplanAudit ia
        ON ia.acctno = ag.acctno
        AND ia.datechange = (SELECT MAX(datechange) from instalplanAudit where acctno = ia.acctno and datechange < @dateFrom)
    WHERE a.dateacctopen < @dateFrom
        AND fin.outstandingBalance > 0
        AND a.accttype != 'S'
        AND @deliveryThreshold * COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) / 100 <= d.deliveredAmount 
        AND (a.accttype = 'C' OR COALESCE(ia.Newinstalment, i.instalamount, 0) != 0)
        AND COALESCE(aa.NewAgreementTotal, ag.agrmttotal, 0) != 0
        AND NOT EXISTS (SELECT 'a' FROM fintrans f WHERE f.acctno = a.acctno AND ag.agrmtno = f.agrmtno AND f.transtypecode IN ('BDW', 'BDR', 'BDC') and f.datetrans < @dateFrom)
        

    --Initial select Cash accounts + Credit accounts with outstanding balance
   SELECT * INTO #orders 
   FROM 
   (
    	    SELECT 0 AS orderBy,
                   'ENT_ENTIDAD' AS ENT_ENTIDAD, 
                   'CDA_CDA' AS CDA_CDA, 
                   'VENTA_ID' AS VENTA_ID,
                   'TIPO_PRODUCTO' AS TIPO_PRODUCTO,
                   'FECHA_VENC_CUOTA' AS FECHA_VENC_CUOTA,
                   'NUMERO_CUOTA' AS NUMERO_CUOTA,
                   'SALDO_CUOTA' AS SALDO_CUOTA,
                   'SALDO_CAPITAL' AS SALDO_CAPITAL,
                   'SALDO_INTERESES' AS SALDO_INTERESES,
                   'CARTERA_DEP' AS CARTERA_DEP,
                   'PERIODO' AS PERIODO
		    UNION ALL
		    SELECT 1 AS orderBy,
                   c.ISOCountryCode + b.StoreType AS ENT_ENTIDAD,
                   CAST(b.branchno AS VARCHAR)AS CDA_CDA,
                   a.acctno AS VENTA_ID,
                   CASE
                       WHEN EXISTS(SELECT 'a' FROM CashLoan cl where cl.AcctNo = a.acctno) THEN 'CASH'
                       ELSE 'MERCHANDISE'
                   END AS TIPO_PRODUCTO,
                   CASE
                       WHEN a.accttype = 'C' THEN CONVERT(VARCHAR, DATEADD(DAY, -1, @dateFrom), 103)                                       -- Cash Accounts 
                       WHEN a.accttype != 'C' AND a.firstInstalment >= @dateFrom 
                            THEN CONVERT(VARCHAR, a.firstInstalment, 103)                                                                  -- Payment holidays and if instalments begin in the future
                       WHEN a.accttype != 'C' AND a.lastInstalment < @dateFrom                                                             -- Credit Accounts; -1 day because the @dataFrom is the 1st of the month. If due day is the 5th adding it will result in the 6th
                            THEN CONVERT(VARCHAR, DATEADD(MONTH, DATEDIFF(MONTH, a.firstInstalment, DATEADD(DAY, -1, @dateFrom)), a.firstInstalment), 103)   -- Credit Accounts not in arrears
                       ELSE CONVERT(VARCHAR, DATEADD(DAY, -1, @dateFrom), 103)                                                             -- Credit Accounts in arrears
                   END AS FECHA_VENC_CUOTA,
                   CAST(CASE
                            WHEN a.accttype = 'C' THEN '1'                                                                      -- Cash Accounts
                            WHEN a.accttype != 'C' AND a.firstInstalment > @dateFrom THEN 0                                     -- Payment holidays and if instalments begin in the future      
                            WHEN a.accttype != 'C' AND a.lastInstalment <= @dateFrom THEN a.instalmentNumber                    -- If its the last day a customer has to pay or the contract has matured and he still has outstanding balance then show instalment number
                            ELSE CASE                                                                                           -- Credit Accounts not in arrears
                                     WHEN DATEPART(DAY, a.firstInstalment) <= DATEPART(DAY, @dateFrom)                           -- If the current DAY hasn't passed the installment DAY then the account is still in the previous instalment period
                                        THEN DATEDIFF(MONTH, a.firstInstalment, @dateFrom) + 1
                                     ELSE DATEDIFF(MONTH, a.firstInstalment, @dateFrom)
                                 END
                        END AS VARCHAR) AS NUMERO_CUOTA,
                   CAST(CASE 
                            WHEN a.accttype != 'C' AND a.lastInstalment > @dateFrom THEN a.instalmentAmount                     -- Credit Accounts
                            ELSE a.outstandingBalance                                                                           -- Cash Accounts and last installment of credit accounts(this would include arrears)
                        END AS VARCHAR) AS SALDO_CUOTA,
                   CAST(CASE 
                            WHEN a.accttype = 'C' THEN a.outstandingBalance
                            WHEN a.accttype != 'C' AND a.lastInstalment > @dateFrom AND a.serviceCharge != 0
                                THEN (100 * a.instalmentAmount) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))          --Formulas on top of the file
                            WHEN a.accttype != 'C' AND a.lastInstalment > @dateFrom AND a.serviceCharge = 0
                                THEN a.instalmentAmount
                            ELSE (100 * a.outstandingBalance) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))          --Formulas on top of the file
                        END AS VARCHAR) AS SALDO_CAPITAL,
                   CAST(CASE 
                            WHEN a.accttype = 'C' THEN '0'
                            WHEN a.accttype != 'C' AND a.lastInstalment > @dateFrom AND a.serviceCharge != 0
                                THEN a.instalmentAmount - (100 * a.instalmentAmount) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))               --instalment amount - part which goes to priciple
                            WHEN a.accttype != 'C' AND a.lastInstalment > @dateFrom AND a.serviceCharge = 0
                                THEN '0'
                            ELSE a.outstandingBalance - (100 * a.outstandingBalance) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))     --final instalment amount - part which goes to priciple
                        END AS VARCHAR) AS SALDO_INTERESES,
                   'NORMAL' AS CARTERA_DEP,
                   UPPER(LEFT(DATENAME(MONTH, DATEADD(DAY, -1, @dateFrom)), 3)) + '-' + RIGHT(DATEPART(YEAR, DATEADD(DAY, -1, @dateFrom)), 2) AS PERIODO
            FROM country c,
                 branch b 
                 INNER JOIN #accounts a
                    ON b.branchno = CAST(LEFT(a.acctno, 3) AS SMALLINT)
            WHERE a.firstInstalment <= @dateFrom
    ) AS ord
    ORDER BY ord.orderBy ASC

    --Remove all accounts which have reached maturity
    DELETE FROM #accounts
    WHERE accttype = 'C'
          OR
          (accttype != 'C' AND lastInstalment <= @dateFrom)
    
    --loop to get all future data for credit accounts
    WHILE EXISTS (SELECT 'a' FROM #accounts)
    BEGIN

    

        SET @tempDate = DATEADD(MONTH, 1, @tempDate)
        --update outstanding balance
        UPDATE #accounts
        SET outstandingBalance = outstandingBalance - instalmentAmount
        WHERE accttype != 'C'
            AND firstInstalment <= DATEADD(DAY, -1, @tempDate)
            AND lastInstalment > DATEADD(DAY, -1, @tempDate)

        INSERT INTO #orders
        SELECT 1 AS orderBy,
               c.ISOCountryCode + b.StoreType AS ENT_ENTIDAD,
               CAST(b.branchno AS VARCHAR)AS CDA_CDA,
               a.acctno AS VENTA_ID,
               CASE
                   WHEN EXISTS(SELECT 'a' FROM CashLoan cl where cl.AcctNo = a.acctno) THEN 'CASH'
                   ELSE 'MERCHANDISE'
               END AS TIPO_PRODUCTO,
               CASE
                   WHEN a.firstInstalment >= @tempDate THEN CONVERT(VARCHAR, a.firstInstalment, 103) 
                   WHEN a.lastInstalment > @tempDate 
                       THEN CONVERT(VARCHAR, DATEADD(MONTH, DATEDIFF(MONTH, a.firstInstalment, DATEADD(DAY, -1, @tempDate)), a.firstInstalment), 103)      -- Credit Accounts; -1 day because the @dataFrom is the 1st of the month. If due day is the 5th adding it will result in the 6th
                   ELSE CONVERT(VARCHAR, a.lastInstalment, 103)                                                                           -- last instalment date
               END AS FECHA_VENC_CUOTA,
               CAST(CASE
                        WHEN a.firstInstalment > @tempDate THEN 0                                                               -- Payment holidays and if instalments begin in the future      
                        ELSE CASE                                                                                               -- Credit Accounts not in arrears
                                 WHEN DATEPART(DAY, a.firstInstalment) <= DATEPART(DAY, @tempDate)                               -- If the current DAY hasn't passed the installment DAY then the account is still in the previous instalment period
                                 THEN DATEDIFF(MONTH, a.firstInstalment, @tempDate) + 1
                                 ELSE DATEDIFF(MONTH, a.firstInstalment, @tempDate)
                             END
                    END AS VARCHAR) AS NUMERO_CUOTA,
               CAST(CASE 
                       WHEN a.lastInstalment > @tempDate THEN a.instalmentAmount                                            -- Credit Accounts
                       ELSE a.outstandingBalance                                                                            -- Cash Accounts and last installment of credit accounts(this would include arrears)
                   END AS VARCHAR) AS SALDO_CUOTA,
               CAST(CASE 
                       WHEN a.lastInstalment > @tempDate AND a.serviceCharge != 0
                           THEN (100 * a.instalmentAmount) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))          --Formulas on top of the file
                       WHEN a.lastInstalment > @tempDate AND a.serviceCharge = 0
                           THEN a.instalmentAmount
                       ELSE (100 * a.outstandingBalance) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))            --Formulas on top of the file
                   END AS VARCHAR) AS SALDO_CAPITAL,
               CAST(CASE 
                       WHEN a.lastInstalment > @tempDate AND a.serviceCharge != 0
                           THEN a.instalmentAmount - (100 * a.instalmentAmount) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))     --instalment amount - part which goes to priciple
                       WHEN a.lastInstalment > @tempDate AND a.serviceCharge = 0
                           THEN '0'
                       ELSE a.outstandingBalance - (100 * a.outstandingBalance) / (100 + (ROUND((100 * a.serviceCharge * 12) / ((a.agrementTotal - a.serviceCharge - a.depositAmount) * a.instalmentNumber), 2) * a.instalmentNumber / 12))     --final instalment amount - part which goes to priciple
                   END AS VARCHAR) AS SALDO_INTERESES,
               'NORMAL' AS CARTERA_DEP,
               UPPER(LEFT(DATENAME(MONTH, DATEADD(DAY, -1, @dateFrom)), 3)) + '-' + RIGHT(DATEPART(YEAR, DATEADD(DAY, -1, @dateFrom)), 2) AS PERIODO
    FROM country c,
            branch b 
    INNER JOIN #accounts a
    ON b.branchno = CAST(LEFT(a.acctno, 3) AS SMALLINT)
    WHERE a.accttype != 'C'
        AND a.lastInstalment > DATEADD(month, -1, @tempDate)   
        AND a.firstInstalment <= @tempDate

    --Remove all accounts which have reached maturity
    DELETE FROM #accounts
    WHERE lastInstalment <= @tempDate
        
    END

    SELECT ord.ENT_ENTIDAD, 
          ord.CDA_CDA, 
          ord.VENTA_ID,
          ord.TIPO_PRODUCTO,
          ord.FECHA_VENC_CUOTA,
          ord.NUMERO_CUOTA,
          ord.SALDO_CUOTA,
          ord.SALDO_CAPITAL,
          ord.SALDO_INTERESES,
          ord.CARTERA_DEP,
          ord.PERIODO
    INTO ##tempExport 
    FROM #orders ord
    Order by ord.orderBy ASC

END TRY
BEGIN CATCH
SELECT * INTO ##tempExportError FROM
(
    SELECT 'ENT_ENTIDAD' AS ENT_ENTIDAD, 
               'CDA_CDA' AS CDA_CDA, 
               'VENTA_ID' AS VENTA_ID,
               'TIPO_PRODUCTO' AS TIPO_PRODUCTO,
               'FECHA_VENC_CUOTA' AS FECHA_VENC_CUOTA,
               'NUMERO_CUOTA' AS NUMERO_CUOTA,
               'SALDO_CUOTA' AS SALDO_CUOTA,
               'SALDO_CAPITAL' AS SALDO_CAPITAL,
               'SALDO_INTERESES' AS SALDO_INTERESES,
               'CARTERA_DEP' AS CARTERA_DEP,
               'PERIODO' AS PERIODO
) AS tmp

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExportError out ' + @directory + ' -w -q -t^| -Usa -P'		

print 'msg: ERROR with date: ' + CAST(@dateFrom AS VARCHAR) + ' ' + ' t_src_plan_pagos ' + ERROR_MESSAGE()

END CATCH

EXEC master..xp_cmdshell @bcpCommand

IF OBJECT_ID('tempdb..##tempExport') IS NOT NULL
DROP TABLE ##tempExport

IF OBJECT_ID('tempdb..##tempExportError') IS NOT NULL
DROP TABLE ##tempExportError

IF OBJECT_ID('tempdb..#accounts') IS NOT NULL
    DROP TABLE #accounts

IF OBJECT_ID('tempdb..#orders') IS NOT NULL
    DROP TABLE #orders

GO