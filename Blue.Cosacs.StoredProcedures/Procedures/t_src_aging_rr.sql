SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects where id = object_id(N'[dbo].t_src_aging_rr') 
            and OBJECTPROPERTY(id, N'IsProcedure') = 1)
BEGIN
    DROP PROCEDURE dbo.t_src_aging_rr
END
GO

-- =============================================
-- Author:		Iliya Zanev
-- Create date: 17 August 2012
-- Description:	Hyperion Extract files
--
-- 12/04/13 jec #12859 - UAT12602 
-- =============================================

CREATE PROCEDURE dbo.t_src_aging_rr
(
	@dateFrom DATETIME
	, @dateTo DATETIME
)

AS

DECLARE @directory VARCHAR(100),
		@filename VARCHAR(50),
		@bcpCommand VARCHAR(5000),
		@bcpPath VARCHAR(100),
        @dateFinYear DATETIME,
        @arrearsDate DATETIME

-- if EOD is pst midnight save the initial @dateTo to avoid further calculation
set @dateFinYear = DATEADD(SECOND, -1, @dateTo)

--set the @dateTo parameter to the last time Summary was run to ensure only the relevant data is included in the calculations 
select @dateTo = MAX(DateFinish)
from InterfaceControl i
where Interface in ('SUMRYDATA', 'SUMRYDATAFUL')
    and DateStart < DATEADD(HOUR, 5, @dateto)
    and Result in ('P', 'W')

--get the arrears calculation EOD date
select @arrearsDate = MAX(DateFinish)
from InterfaceControl
where Interface in ('ARREARS', 'ARREARSEOD')
    and DateStart < @dateTo
    and Result in ('P', 'W')

IF (select interface from InterfaceControl where DateFinish = @arrearsDate) = 'ARREARSEOD'
      AND
   (select DATEPART(HOUR, datestart) from interfacecontrol where DateFinish = @arrearsDate) >= 7
BEGIN        
        set @arrearsDate = DATEADD(day, 1, @arrearsDate)
END

--set export file name
SET @filename = 't_src_aging_rr.txt'		
		
select @directory = value from CountryMaintenance where CodeName = 'systemdrive'
set @directory += '\' + @filename

SELECT @bcpPath = VALUE FROM dbo.CountryMaintenance WHERE CodeName = 'BCPpath'
--Testing because our BCP directory is different
--SET @bcpPath = 'C:\Program Files\Microsoft SQL Server\100\Tools\binn'
SET @bcpPath += '\BCP'

SET @bcpCommand = '"' + @bcpPath + '" ..##tempExport out ' + @directory + ' -w -t^| -Usa -P'			-- #12859

--temporary data to get account aging as in Summary EOD
select a.acctno, 
       b.accttypegroup, 
       SUM(f.transvalue) as outstandingBalance,
       SUM(f.transvalue) as outstandingBalanceCorrectedOld,
       isnull(fiDel.amountDelivered, 0) as amountDelivered,
       cast(0 as money) as agreementTotal,
       isnull(fiOld.intcharges, 0) as intchargesOld,
       cast (NULL as datetime) as datedel1,
       cast(0 as int) as daysInArrears,
       isnull(fR.repoAmount, 0) as repoAmount,
       case 
           when a.accttype = 'T' then 'T'
           else coalesce(s.statuscode, a.currstatus, '') 
       end as currentStatus,
       a.accttype as accountType,
       'Y' as deliveryFlag,
       cast(0 as money) as arrears
into #data
from vw_Summary1_B b
inner join acct a
on b.acctno = a.acctno
inner join fintrans f
on f.acctno = a.acctno
left outer join (SELECT acctno, 
                   SUM(transvalue) AS amountDelivered
		         FROM fintrans
			     Where transtypecode in ('ADD', 'DEL', 'GRT', 'REP', 'RDL', 'CLD')
                    and datetrans < @dateTo
                 GROUP BY acctno
                ) as fiDel
on fiDel.acctno = a.acctno 
left outer join (SELECT acctno, 
                   SUM(transvalue) AS intcharges
		         FROM fintrans
			     Where transtypecode in ('INT', 'ADM','DDF')
                    and datetrans < @dateTo
                 GROUP BY acctno
                ) as fiOld
on fiOld.acctno = a.acctno
left outer join (Select f.acctno,
                        sum(f.transvalue) as repoAmount
		         From fintrans f
			     Where f.transtypecode in ('REP', 'RDL', 'RPO')
                    and f.datetrans <= @dateTo
                group by f.acctno
                ) as fR
on fR.acctno = a.acctno
left outer join [status] s 
on s.acctno = a.acctno
    and s.datestatchge = (select MAX(datestatchge) from [status] where acctno = s.acctno and datestatchge <= @dateTo)
where a.dateacctopen <= @dateTo
    and f.datetrans <= @dateTo
group by a.acctno, 
         b.accttypegroup, 
         isnull(fiOld.intcharges, 0), 
         isnull(fR.repoAmount, 0), 
         case 
            when a.accttype = 'T' then 'T'
            else coalesce(s.statuscode, a.currstatus, '') 
         end,
         a.accttype,
         isnull(fiDel.amountDelivered, 0)

--setting agreeement totals. This was way faster with a CTE
;with agreementsCTE(acctno, agrmtTotal)
AS (select a.acctno, case 
                        when @dateTo > (select MAX(datechange) from agreementAudit where acctno = a.acctno) then a.agrmttotal
                        else coalesce(agA.NewAgreementTotal, a.agrmttotal, cast(0 as money)) 
                     end as agrmttotal
    from acct a
    left outer join agreementAudit agA
    on a.acctno = agA.acctno
        and agA.datechange = (select max(datechange) from agreementAudit where acctno = agA.acctno and datechange <= @dateTo)
    where a.dateacctopen < @dateTo
)

 update d
 set d.agreementTotal = a.agrmttotal
 from #data d
 inner join agreementsCTE a
 on d.acctno = a.acctno 

--updating agreement total and account status - incorrect data
update d
set d.outstandingBalanceCorrectedOld = a.outstbal,
    d.outstandingBalance = a.outstbal,
    d.currentStatus = a.currstatus
from #data d 
inner join acct a
on d.acctno = a.acctno
where (d.agreementTotal != a.agrmttotal OR d.currentStatus != a.currstatus)
    AND a.datelastpaid < @dateTo


--outstanding balance corrected = outstanding balance - interest charges
update #data
set outstandingBalanceCorrectedOld = outstandingBalanceCorrectedOld - intchargesOld
where outstandingBalanceCorrectedOld != 0 

--update arrears
select d.acctno,
       coalesce(ad.arrears, a.arrears) as arrears,
       ad.datefrom as arrearsDateFrom,
       finMin.minDateTrans
into #arrears
from #data d
inner join acct a
on d.acctno = a.acctno
left outer join ArrearsDaily ad
on a.acctno = ad.Acctno
    and ad.datefrom = (select max(datefrom) from ArrearsDaily where acctno = ad.Acctno and datefrom <= @arrearsDate)
left outer join (select acctno, min(datetrans) as minDateTrans
                 from fintrans 
                 where datetrans > @dateTo
                    and transtypecode NOT IN ('DEL','GRT','REP','ADD','RPO','RDL','CLD') 
                 group by acctno) as finMin
on d.acctno = finMin.acctno
where d.accttypegroup in ('HP', 'RF', 'CLN', 'SGR', 'AST')
    AND d.outstandingBalanceCorrectedOld > 0

update a
set a.arrears = (select arrears 
                 from ArrearsDaily ad
                 where ad.Acctno = a.acctno 
                    and ad.dateto = a.arrearsDateFrom
                    and id = (select min(id) 
                              from ArrearsDaily 
                              where acctno = ad.acctno
                                and dateto = ad.dateto)
                )
from #arrears a
where a.arrearsDateFrom = dbo.StripTimeMinusSecond(minDateTrans)

--set delivery date for cash and store card accounts
Update	d 
set	d.datedel1 = (select max(datetrans) 
                  from fintrans f 
			      where f.acctno = d.acctno 
                    and f.transtypecode in ('SCT', 'DEL','CLD')
                    and f.datetrans <= @dateTo
                 )
from #data d 
inner join acct a
on a.acctno = d.acctno
Where	a.accttype in ('T', 'C')

Update d  
set	d.datedel1 = ag.datedel 
from #data d
INNER JOIN acct a
on d.acctno = a.acctno
    AND	a.accttype	in ('T', 'C')
INNER JOIN agreement ag
on d.acctno = ag.acctno 
	AND	ag.agrmtno = 1 
	AND	ag.datedel <> '' 
	AND	ag.datedel IS NOT NULL 
	AND	ag.datedel != d.datedel1 

--set delivery date for non cash loan and store card accounts

Update d  
set	datedel1 = ag.datedel 
from #data d
INNER JOIN acct a 
on a.acctno = d.acctno
    AND	a.accttype not in ('T', 'C') 
INNER JOIN agreement ag  
ON ag.acctno = d.acctno  
	AND	ag.agrmtno = 1  

--update delivery flag for non cash and store card accounts 
Update d
set	deliveryFlag = 'N'
from #data d
INNER JOIN agreement ag
on d.acctno = ag.acctno
where d.accountType not in ('T', 'C') 
    and ag.agrmtno = 1
    and (d.amountDelivered / isnull(nullif(d.agreementTotal, 0), 1)) * 100 < (select globdelpcent from country)
    and d.datedel1 > @dateTo

Update d
Set	deliveryflag	= 'N'
From #data d
inner join custacct ca
on d.acctno = ca.acctno
    AND ca.hldorjnt = 'H'
    AND ca.custid not like 'PAID%'
Where (d.datedel1 	is null
		OR  d.datedel1  < '01-jan-1910'
		OR	d.datedel1 = '')
       AND ISNULL(d.datedel1, '1911-01-01') <= @dateTo	
       AND d.accountType not in ('T', 'C') 

--update delivery flag for cash and store card accounts 
Update d 
		Set	deliveryflag	= 'N'
From #data d
inner join custacct ca
on ca.acctno = d.acctno
	and ca.hldorjnt='H'
	and ca.custid not like 'PAID%'
	AND	(datedel1 	is null
		 OR  
         datedel1  < '01-jan-1910'
		 OR	
         datedel1 = '')
    AND d.accountType in ('T', 'C')
    --AND d.currentStatus != 'S'
    
--calculate days in arrears for cash accounts
Update d
	set	d.daysInArrears = isnull(cast(1.0 + datediff(month,datedel1, CONVERT(DATETIME, CONVERT(VARCHAR(10), @dateTo, 105), 105)) as money ), 0)* 30
from #data d
Where d.accttypegroup = 'C'  	
	AND	d.outstandingBalanceCorrectedOld > 0
	AND	isnull(datedel1,'') > '01-jan-1910' 

--calculate days in arrears for credit accounts
Update	d 
set d.daysInArrears = isnull(cast(((isnull(ar.arrears, 0) -isnull(d.repoAmount, 0)- ISNULL(d.intchargesOld, 0)) / coalesce(ia.Newinstalment, i.instalamount) ) as money), 0) * 30
From #data d
left outer join instalplanAudit ia
on d.acctno = ia.acctno
    and ia.datechange = (select max(datechange) from instalplanAudit where acctno = ia.acctno and datechange <= @dateTo)
    and ia.agrmtno = 1
inner join instalplan i
on i.acctno = d.acctno
    and i.agrmtno = 1
left outer join #arrears ar
on ar.Acctno = d.acctno
Where coalesce(ia.Newinstalment, i.instalamount) > 1
    AND	d.accttypegroup in ('HP', 'RF', 'CLN', 'SGR', 'AST')
    AND d.outstandingBalanceCorrectedOld > 0
    AND cast(0.5 + ((isnull(ar.arrears, 0) -isnull(d.repoAmount, 0)- ISNULL(d.intchargesOld, 0)) / coalesce(ia.Newinstalment, i.instalamount)) as float) between -1000 and 1000

--get temporary data to calculate days in arrears for store card accounts
;with storecardArrears(acctno, dateto, datedue, currminpay, prevminpay, outstminpay, payments)
AS
(
	SELECT this.acctno, this.[dateto], DATEADD(DAY,CONVERT(INT, value), this.[dateto]) AS datedue, this.StmtMinPayment AS currminpay, 
			this.prevstmtminpayment AS prevminpay,
			this.prevstmtminpayment + SUM(ISNULL(f2.transvalue,0)) AS outstminpay, SUM(ISNULL(f.transvalue, 0)) AS payments
	FROM StoreCardStatement this
	INNER JOIN countrymaintenance 
		ON codename = 'SCardInterestFreeDays'
	INNER JOIN (SELECT acctno, MAX(datefrom) as datefrom
				FROM StoreCardStatement
				GROUP BY acctno
                ) as m 
	ON m.acctno = this.acctno
		AND m.datefrom = this.datefrom
	LEFT OUTER JOIN fintrans f
		ON datetrans > this.[DateTo]
		AND f.acctno = this.Acctno
		AND f.transtypecode IN ('pay', 'ref', 'cor', 'str')
	LEFT OUTER JOIN fintrans f2
		ON  f2.acctno = this.acctno
		AND f2.datetrans between this.DateFrom and this.DateTo
		AND f2.transtypecode IN ('pay', 'ref', 'cor', 'str')
	GROUP BY  this.acctno, this.[dateto], this.stmtminpayment, this.prevstmtminpayment, value
)

--calculate days in arrears for store card accounts
Update d 
set daysInArrears = isnull(CASE WHEN sa.datedue> GETDATE() THEN (sa.outstminpay+sa.payments) / sa.prevminpay * 30
				           ELSE (sa.currminpay + sa.payments) / sa.prevminpay * 30
				           END, 0)
From #data d
inner join storecardArrears sa	
on d.acctno = sa.acctno
WHERE sa.prevminpay > 0
	AND	d.accttypegroup = 'SC'

--last update on days in arrears as per Summary
Update	#data 
set daysInArrears = 0
where daysInArrears < 0


--Arrears buckets totals
CREATE TABLE #arrearsTotals(
    countryCode	char(2),
    storeType char(1),
    arrearsGroup varchar(7),
    daysInArrears int,
    outstandingBalanceCorrectedOld	money,
    currentStatus char(1)
)

--As done in Summary4_New - AOB_New_Aging Reconciliation

--All unsettled accounts
INSERT INTO #arrearsTotals
SELECT	c.ISOCountryCode, 
        b.StoreType, 
        '661-999', 
        d.daysInArrears, 
        SUM(d.outstandingBalanceCorrectedOld),
        d.currentStatus
FROM country c, 
#data d
inner join branch b
on cast(LEFT(d.acctno, 3) as int) = b.branchno
WHERE d.currentStatus IN ('1', '2', '3', '4', '5', '6', '7', '8', '9', 'T')
    AND	d.outstandingBalanceCorrectedOld > 0
    AND	d.accountType != 'S'
    AND	d.deliveryFlag = 'Y'
    AND d.outstandingBalance not between -0.00999999 and 0.009999999
GROUP BY
	c.ISOCountryCode, b.StoreType, d.daysInArrears, d.currentStatus

--=================================================================================
--Below might be requested after testing
--=================================================================================

----Settled accounts with balances 
--INSERT INTO #arrearsTotals
--SELECT	c.ISOCountryCode, 
--        b.StoreType, 
--        'St', 
--        0, 
--        SUM(d.outstandingBalanceCorrectedOld),
--        d.currentStatus
--FROM country c, 
--#data d
--inner join branch b
--on cast(LEFT(d.acctno, 3) as int) = b.branchno
--WHERE d.currentStatus = 'S'
--    AND	d.accountType != 'S'
--    AND d.outstandingBalance not between -0.00999999 and 0.009999999
--GROUP BY
--	c.ISOCountryCode, b.StoreType, d.currentStatus

----Special Accounts
--INSERT INTO #arrearsTotals
--SELECT	c.ISOCountryCode, 
--        b.StoreType, 
--        'Sp', 
--        0, 
--        SUM(d.outstandingBalanceCorrectedOld),
--        d.currentStatus
--FROM country c, 
--#data d
--inner join branch b
--on cast(LEFT(d.acctno, 3) as int) = b.branchno
--WHERE d.accountType = 'S'
--    AND d.outstandingBalance not between -0.00999999 and 0.009999999
--GROUP BY
--	c.ISOCountryCode, b.StoreType, d.currentStatus

----INT and ADM for unsettled and settled accounts Accounts
--INSERT INTO #arrearsTotals
--SELECT	c.ISOCountryCode, 
--        b.StoreType, 
--        'AD', 
--        0, 
--        SUM(d.outstandingBalance - d.outstandingBalanceCorrectedOld),
--        d.currentStatus
--FROM country c, 
--#data d
--inner join branch b
--on cast(LEFT(d.acctno, 3) as int) = b.branchno
--WHERE d.outstandingBalance not between -0.00999999 and 0.009999999
--GROUP BY
--	c.ISOCountryCode, b.StoreType, d.currentStatus

----Accounts with no status
--INSERT INTO #arrearsTotals
--SELECT	c.ISOCountryCode, 
--        b.StoreType, 
--        'Sc', 
--        0, 
--        SUM(d.outstandingBalanceCorrectedOld),
--        d.currentStatus
--FROM country c, 
--#data d
--inner join branch b
--on cast(LEFT(d.acctno, 3) as int) = b.branchno
--WHERE ISNULL(d.currentStatus, '¬') IN ('', 'U', '0', '¬')
--    AND	d.deliveryFlag = 'Y'    
--    AND	d.outstandingBalanceCorrectedOld > 0
--    AND	d.accountType != 'S'
--    AND d.outstandingBalance not between -0.00999999 and 0.009999999
--GROUP BY
--	c.ISOCountryCode, b.StoreType, d.currentStatus

----Undelivered accounts
--INSERT INTO #arrearsTotals
--SELECT	c.ISOCountryCode, 
--        b.StoreType, 
--        'Nd', 
--        0, 
--        SUM(d.outstandingBalanceCorrectedOld),
--        d.currentStatus
--FROM country c, 
--#data d
--inner join branch b
--on cast(LEFT(d.acctno, 3) as int) = b.branchno
--WHERE d.currentStatus != 'S'
--    AND	d.deliveryFlag != 'Y'    
--    AND	d.accountType != 'S'
--    AND d.outstandingBalance not between -0.00999999 and 0.009999999
--GROUP BY
--	c.ISOCountryCode, b.StoreType, d.currentStatus

----Outstanding balance corrected less than 0
--INSERT INTO #arrearsTotals
--SELECT	c.ISOCountryCode, 
--        b.StoreType, 
--        'Cr', 
--        0, 
--        SUM(d.outstandingBalanceCorrectedOld),
--        d.currentStatus
--FROM country c, 
--#data d
--inner join branch b
--on cast(LEFT(d.acctno, 3) as int) = b.branchno
--WHERE d.currentStatus != 'S'
--    AND	d.deliveryFlag = 'Y'    
--    AND	d.accountType != 'S'
--    AND d.outstandingBalance not between -0.00999999 and 0.009999999
--    AND d.outstandingBalanceCorrectedOld <= 0
--GROUP BY
--	c.ISOCountryCode, b.StoreType, d.currentStatus
--===========================================================================

--update the arrears group based on the buckets breakdown
update a 
set a.arrearsGroup = CASE 
                    WHEN daysInArrears <= 0 THEN '0-0'
                    WHEN daysInArrears BETWEEN 1 AND 30 THEN '1-30'
                    WHEN daysInArrears BETWEEN 31 AND 60 THEN '31-60'
                    WHEN daysInArrears BETWEEN 61 AND 90 THEN '61-90'
                    WHEN daysInArrears BETWEEN 91 AND 120 THEN '91-120'
                    WHEN daysInArrears BETWEEN 121 AND 150 THEN '121-150'
                    WHEN daysInArrears BETWEEN 151 AND 180 THEN '151-180'
                    WHEN daysInArrears BETWEEN 181 AND 210 THEN '181-210'
                    WHEN daysInArrears BETWEEN 211 AND 240 THEN '211-240'
                    WHEN daysInArrears BETWEEN 241 AND 270 THEN '241-270'
                    WHEN daysInArrears BETWEEN 271 AND 300 THEN '271-300'
                    WHEN daysInArrears BETWEEN 301 AND 330 THEN '301-330'
                    WHEN daysInArrears BETWEEN 331 AND 360 THEN '331-360'
                    WHEN daysInArrears BETWEEN 361 AND 390 THEN '361-390'
                    WHEN daysInArrears BETWEEN 391 AND 420 THEN '391-420'
                    WHEN daysInArrears BETWEEN 421 AND 450 THEN '421-450'
                    WHEN daysInArrears BETWEEN 451 AND 480 THEN '451-480'
                    WHEN daysInArrears BETWEEN 481 AND 510 THEN '481-510'
                    WHEN daysInArrears BETWEEN 511 AND 540 THEN '511-540'
                    WHEN daysInArrears BETWEEN 541 AND 570 THEN '541-570'
                    WHEN daysInArrears BETWEEN 571 AND 600 THEN '571-600'
                    WHEN daysInArrears BETWEEN 601 AND 630 THEN '601-630'
                    WHEN daysInArrears BETWEEN 631 AND 660 THEN '631-660'
                    ELSE '661-999'
                   END
FROM #arrearsTotals a
WHERE a.currentStatus in ('1', '2', '3', '4', '5', '6', '7', '8', '9', 'T')
    --and a.arrearsGroup not in('St', 'Sp', 'AD', 'Sc', 'Nd', 'Cr') -- part of the upper commented code


;WITH initialData(countryCode, storeType, daysInArrears, outstandingBalanceCorrected)
AS
(
	SELECT a.countryCode AS countryCode
           , a.storeType as storeType
		   , a.arrearsGroup AS daysInArrears
		   , SUM(a.outstandingBalanceCorrectedOld) as outstandingBalanceCorrected
	FROM #arrearsTotals a
    GROUP BY a.countryCode
           , a.storeType
		   , a.arrearsGroup
)

SELECT * INTO ##tempExport FROM
(
		SELECT 'ent_Entidad' AS ent_Entidad, 
               'cad_Cadena' AS cad_Cadena, 
               'agr_CalAnio' AS agr_CalAnio, 
               'agr_Periodo' AS agr_Periodo, 
               'agr_Metodo' AS agr_Metodo, 
               'agr_FinAnio' AS agr_FinAnio, 
               'agr_Plazo' AS agr_Plazo, 
               'agr_Monto' AS agr_Monto
		UNION ALL
		SELECT ISNULL(countryCode, ''), 
               ISNULL(storeType, ''),
               ISNULL(CAST(DATEPART(Year, @dateFinYear) AS VARCHAR), ''), 
               ISNULL(CAST(LEFT(DATENAME(MONTH, @dateFinYear), 3) AS VARCHAR), ''), 
               'ROLL RATE/AGING' AS agr_Metodo, 
               CAST(CASE
		                WHEN DATEPART(MONTH, @dateFinYear) < 4 THEN DATEPART(YEAR, @dateFinYear)
		                ELSE DATEPART(YEAR, @dateFinYear) + 1
		            END AS VARCHAR), 
               CAST(daysInArrears AS VARCHAR), 
               CAST(outstandingBalanceCorrected AS VARCHAR)
		FROM initialData
) AS tmp	

EXEC master..xp_cmdshell @bcpCommand

DROP TABLE ##tempExport
DROP TABLE #data
DROP TABLE #arrearsTotals
DROP TABLE #arrears

GO