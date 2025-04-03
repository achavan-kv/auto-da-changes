IF EXISTS (SELECT * FROM sysobjects WHERE NAME='GetMaxCustArrearsLevel')
DROP PROCEDURE GetMaxCustArrearsLevel 
GO 
CREATE PROCEDURE GetMaxCustArrearsLevel @custid  VARCHAR(20) , @maxarrearslevel FLOAT OUT ,
 @maxsettledarrearslevel float OUT,@settledmonthstocheck INT , @livemonthsHistory INT ,
 @maxhistoricarrearslevel FLOAT OUT, @datefrom DATETIME 
--------------------------------------------------------------------------------
-- 
-- 18/03/11  IP  Sprint 5.12 - #3335 - Balance may be cleared so do not check for a balance
-- 
--------------------------------------------------------------------------------
AS 
SELECT a.arrears,i.instalamount, a.acctno,CONVERT(MONEY,0) AS outstandingcheque,
CONVERT(MONEY,0) AS historiccurrentarrearslevel
INTO #arrslevel
FROM acct a JOIN   custacct ca ON ca.acctno= a.acctno
JOIN instalplan i ON a.acctno= i.acctno
WHERE  ca.hldorjnt = 'H' AND i.instalamount >0
--AND a.outstbal >1 --IP - 18/03/11 - #3335
AND ca.custid = @custid
DECLARE @chequecleardays INT 
SELECT @chequecleardays = CONVERT(INT,value) FROM CountryMaintenance
WHERE NAME ='Cheque Clearance Days'

UPDATE #arrslevel 
SET historiccurrentarrearslevel=
ISNULL((SELECT MAX(d.arrears)/instalamount
FROM ArrearsDaily d 
WHERE d.acctno= #arrslevel.acctno 
AND d.datefrom > DATEADD(MONTH,@livemonthsHistory,@datefrom)),0)


UPDATE a 
SET outstandingcheque=isnull(( SELECT -SUM(f.transvalue) 
FROM fintrans f WHERE RIGHT(CONVERT(VARCHAR,paymethod),1)='2'
AND transtypecode IN ('ret','pay','cor','ref')
AND datetrans > DATEADD(DAY,-@chequecleardays,@datefrom)
 AND a.acctno = f.acctno),0)
FROM #arrslevel a

UPDATE #arrslevel SET arrears = arrears + outstandingcheque 
WHERE outstandingcheque >0
  --SELECT * FROM #ARRSLEVEL
SELECT @maxarrearslevel=MAX(ISNULL( arrears/instalamount,0))
from #arrslevel

IF @maxarrearslevel IS  NULL
	SET @maxarrearslevel = 0

select @maxsettledarrearslevel = 
ISNULL(max( d.arrears/i.instalamount),1000)
FROM acct a JOIN   custacct ca ON ca.acctno= a.acctno
JOIN instalplan i ON a.acctno= i.acctno
join arrearsdaily d on d.acctno= a.acctno 
WHERE  ca.hldorjnt = 'H' AND i.instalamount >0
AND a.outstbal =0 AND a.currstatus = 'S' 
AND ca.custid = @custid AND d.datefrom > DATEADD(MONTH,-@settledmonthstocheck ,@datefrom)

SELECT @maxhistoricarrearslevel = ISNULL(MAX(historiccurrentarrearslevel),0) FROM #arrslevel

IF @maxarrearslevel > @maxhistoricarrearslevel
	SET @maxhistoricarrearslevel = @maxarrearslevel
GO 
