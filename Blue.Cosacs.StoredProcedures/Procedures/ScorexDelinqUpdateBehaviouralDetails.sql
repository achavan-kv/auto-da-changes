IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='ScorexDelinqUpdateBehaviouralDetails')
DROP PROCEDURE ScorexDelinqUpdateBehaviouralDetails
GO 
CREATE PROCEDURE ScorexDelinqUpdateBehaviouralDetails
AS 
SET NOCOUNT ON
CREATE TABLE #AccountDates 
( Month3date DATETIME,
 MONTH9date DATETIME,
 Month12date DATETIME,
 currentdate DATETIME )
 
 DECLARE @months3date DATETIME
 DECLARE @months9date DATETIME
 DECLARE @months12date DATETIME
 -- what we need to do is to get the correct month for comparing history dates from 
 -- now the issue is that account months stores the data as at the end of the month
 -- with a date as at the first of the month so '1-jan-2010' shows data at 31st etc
 -- so 3 months ago's date will be '

 --#13917 - Go back 1 month
 SELECT @months3date = MAX(currentmonth) FROM accountMonths2
 WHERE currentmonth BETWEEN DATEADD(day,-4.8* 30.43,GETDATE())
 AND DATEADD(day,-3.8* 30.43,GETDATE())
 
 SELECT @months9date = MAX(currentmonth) FROM accountMonths2
 WHERE currentmonth BETWEEN DATEADD(day,-10.8* 30.43,GETDATE())
 AND DATEADD(day,-9.8* 30.43,GETDATE())
 
 SELECT @months12date = MAX(currentmonth) FROM accountMonths2
 WHERE currentmonth BETWEEN DATEADD(day,-13.8* 30.43,GETDATE())
 AND DATEADD(day,-12.8* 30.43,GETDATE())
 
 INSERT INTO #accountDAtes VALUES (@months3date,@months9date,@months12date, GETDATE()) 
  SELECT * FROM #AccountDates
DECLARE @currentDate DATETIME 
SET @currentDate = GETDATE() 
--SET @currentDate = '1-JUN-2010'

DECLARE @counter INT 
SET @counter = 1
SET @counter = @counter + 1 
CREATE TABLE #BehaveDetails 
(   custid varCHAR(20) PRIMARY KEY,
	numacctsarrears smallint,
	numactiveaccts	smallint,
    worstintsarrslast6months SMALLINT,
    arrearstot3 MONEY,
    Currentarrearstotal MONEY,
    arrearstotalPercent3months DECIMAL(20,5),
    arrearstot9 MONEY, 
    arrearstotalPercent9months DECIMAL(20,5),
    balancetot3 MONEY,
    balanceTotalPercent3months DECIMAL(20,5) ,  
    balancetot9 MONEY ,
	balanceTotalPercent9months DECIMAL(20,5), 
    monthssincelastGr1inarrears SMALLINT, 
    monthssincelastGr2inarrears SMALLINT,
    worstcurrentstatusChangelast9Months   DECIMAL(20,5), 
    worstcurrentstatusChangelast12Months   DECIMAL(20,5),
    currentbalancetotal MONEY,
    
    worstcurrentinstarrs float    )
    

BEGIN 
	-- get all customers
	INSERT INTO #BehaveDetails (
		custid, 	numactiveaccts
	) SELECT ca.custid, 		COUNT(*)
		 
		FROM custacct ca, acct a ,instalplan i 
		WHERE i.acctno= a.acctno
		AND ca.hldorjnt = 'H' AND ca.acctno = a.acctno 
		AND a.agrmttotal >1 AND ca.custid !=''
		GROUP BY ca.custid 
	-- now only get active ones. 
	UPDATE #BehaveDetails 
	SET numactiveAccts= (SELECT COUNT(*)
	FROM custacct ca, acct a ,instalplan i 
	WHERE ca.custid = #BehaveDetails.custid AND i.acctno= a.acctno
	AND ca.hldorjnt = 'H' AND ca.acctno = a.acctno 
	AND a.outstbal >0)

	
	UPDATE #BehaveDetails 
	SET numacctsarrears= (SELECT COUNT(*)
	FROM custacct ca, acct a ,instalplan i 
	WHERE ca.custid = #BehaveDetails.custid AND i.acctno= a.acctno
	AND ca.hldorjnt = 'H' AND ca.acctno = a.acctno 
	AND a.outstbal >0 AND a.arrears >1)


	UPDATE #BehaveDetails 
	SET numactiveaccts=-1,
	numacctsarrears=-1
	WHERE NOT exists (SELECT ca.custid
	FROM custacct ca, acct a ,instalplan i 
	WHERE ca.custid = #BehaveDetails.custid AND i.acctno= a.acctno
	AND ca.hldorjnt = 'H' AND ca.acctno = a.acctno 
	AND a.outstbal >1 )
	--OR numactiveaccts IS NULL OR numacctsarrears IS NULL 
 
	----UPDATE #BehaveDetails 
	----SET  worstintsarrslast6months
	----=(select 	CEILING((ISNULL(MAX((i.arrears)/i.instalamount) * 1,-1000)))
	----FROM accountmonths2 i,custacct ca
	----WHERE 
	----Ca.acctno = i.acctno AND ca.hldorjnt = 'H' AND ca.custid= #BehaveDetails.custid
	----AND i.datedel IS NOT NULL AND i.datedel < DATEADD(MONTH,1,@currentdate)
	----AND i.currentMonth > DATEADD(MONTH,-7,@currentdate)
	----AND i.instalamount >0 )

    ;WITH worstrarrearscheck
	AS
	(
		select ca.custid,CEILING((ISNULL(MAX((i.arrears)/i.instalamount) * 1,-1000))) as InstArrears
		FROM accountmonths2 i,custacct ca, #BehaveDetails b
		WHERE 
		Ca.acctno = i.acctno AND ca.hldorjnt = 'H' AND ca.custid= b.custid
		AND i.datedel IS NOT NULL AND i.datedel < DATEADD(MONTH,1,@currentdate)
		AND i.currentMonth > DATEADD(MONTH,-7,@currentdate)
		AND i.instalamount >0 
		group by ca.custid
	)
 
	UPDATE #BehaveDetails 
	SET  
		worstintsarrslast6months = 
			case 
				when w.InstArrears < -1000
					then -1000
				else
					w.InstArrears
			end
	FROM 
		worstrarrearscheck w
	INNER JOIN 
		#BehaveDetails b on w.custid = b.custid 
	
	UPDATE #BehaveDetails SET worstintsarrslast6months = 0 
	WHERE worstintsarrslast6months BETWEEN -999 and 0
	
	UPDATE #BehaveDetails SET worstintsarrslast6months = -1 
	WHERE worstintsarrslast6months =-1000
	
	UPDATE #BehaveDetails SET worstintsarrslast6months = 30000 
	WHERE worstintsarrslast6months >32000
	
	UPDATE #BehaveDetails SET worstintsarrslast6months=999
	WHERE  EXISTS (SELECT * FROM status s , custacct ca 
	WHERE s.acctno = ca.acctno AND ca.hldorjnt = 'H' AND ca.custid = #BehaveDetails.custid 
	AND s.datestatchge > DATEADD(MONTH,-6,GETDATE()) AND s.statuscode ='7')

	UPDATE #BehaveDetails SET worstintsarrslast6months=999
	WHERE EXISTS (SELECT * FROM fintrans s , custacct ca 
	WHERE s.acctno = ca.acctno AND ca.hldorjnt = 'H'
	AND s.datetrans > DATEADD(MONTH,-6,GETDATE()) AND s.transtypecode ='BDW'
	AND ca.custid=#BehaveDetails.custid )
END 	
	
UPDATE #behaveDetails SET Currentarrearstotal  
= ISNULL((SELECT SUM(arrears) FROM acct a, instalplan m , custacct ca 
WHERE ca.hldorjnt= 'H' AND m.acctno= ca.acctno
AND m.acctno= a.acctno AND a.outstbal >0 
AND ca.custid = #behavedetails.custid AND ca.hldorjnt = 'H'),0)

UPDATE #behaveDetails SET Currentarrearstotal  =0 WHERE currentarrearstotal <0


UPDATE #BehaveDetails SET currentbalancetotal
= (SELECT SUM(outstbal) FROM acct m , custacct ca 
WHERE ca.hldorjnt= 'H' AND m.acctno= ca.acctno 
AND ca.custid = #behavedetails.custid )

EXEC DelinquencyBalArrsDynamic @column ='Balance',@period ='3'

EXEC DelinquencyBalArrsDynamic @column ='Balance',@period ='9'

EXEC DelinquencyBalArrsDynamic @column ='Arrears',@period ='3'

EXEC DelinquencyBalArrsDynamic @column ='Arrears',@period ='9'

UPDATE #BehaveDetails SET monthssincelastGr1inarrears =
(SELECT DATEDIFF(day,max(currentmonth),@currentDate)/30.33 FROM dbo.custacct ca, accountMonths2 b 
WHERE ca.custid  =#BehaveDetails.Custid AND ca.acctno= b.acctno
AND b.outstbal >1 AND b.arrears >=b.instalamount AND b.instalamount >0
AND ca.hldorjnt ='H')

UPDATE #BehaveDetails SET monthssincelastGr1inarrears =0
WHERE  EXISTS (SELECT * FROM dbo.custacct ca, acct b, instalplan i 
WHERE ca.custid  =#BehaveDetails.Custid AND ca.acctno= b.acctno
AND i.acctno=b.acctno  AND ca.hldorjnt ='H'
AND b.outstbal >1 AND b.arrears >=i.instalamount AND i.instalamount >0)

UPDATE #BehaveDetails SET monthssincelastGr2inarrears =
(SELECT DATEDIFF(day,max(currentmonth),@currentDate)/30.33 FROM dbo.custacct ca, accountMonths2 b 
WHERE ca.custid  =#BehaveDetails.Custid AND ca.acctno= b.acctno
AND b.outstbal >1 AND b.arrears >=b.instalamount*2 AND b.instalamount >0
AND ca.hldorjnt ='H')

UPDATE #BehaveDetails SET monthssincelastGr2inarrears =0
WHERE  EXISTS (SELECT * FROM dbo.custacct ca, acct b, instalplan i 
WHERE ca.custid  =#BehaveDetails.Custid AND ca.acctno= b.acctno
AND i.acctno=b.acctno AND ca.hldorjnt ='H' 
AND b.outstbal >1 AND b.arrears >=i.instalamount*2 AND i.instalamount >0)

UPDATE #BehaveDetails SET monthssincelastGr1inarrears =1000
WHERE  ISNULL(monthssincelastGr1inarrears,100) >12

UPDATE #BehaveDetails SET monthssincelastGr2inarrears =1000
WHERE  ISNULL(monthssincelastGr2inarrears,100) >12

UPDATE #BehaveDetails SET worstcurrentinstarrs 
= (SELECT MAX(a.arrears/i.instalamount) FROM acct a, instalplan i, custacct ca 
WHERE ca.custid =#BehaveDetails.custid  AND ca.hldorjnt ='H'
AND i.acctno= ca.acctno AND a.acctno = i.acctno 
AND ca.hldorjnt = 'H' AND i.instalamount >1 ) -- using 1 to be safe...

UPDATE #BehaveDetails SET worstcurrentinstarrs 
= (SELECT MAX(a.arrears/i.instalamount) 
FROM acct a, instalplan i, custacct ca 
WHERE ca.custid =#BehaveDetails.custid  AND ca.hldorjnt ='H'
AND i.acctno= ca.acctno AND a.acctno = i.acctno 
AND ca.hldorjnt = 'H' AND i.instalamount >1 ) -- using 1 to be safe...

UPDATE #BehaveDetails SET worstcurrentstatusChangelast9Months 
= worstcurrentinstarrs-ISNULL((SELECT MAX(a.arrears/a.instalamount) 
FROM accountmonths2 a, custacct ca 
WHERE ca.custid =#BehaveDetails.custid AND ca.hldorjnt ='H'
AND a.acctno= ca.acctno 
AND ca.hldorjnt = 'H' AND a.instalamount >1 AND a.arrears >=0
AND a.currentmonth = @months9date),0)-- using 1 to be safe...

UPDATE #BehaveDetails SET worstcurrentstatusChangelast12Months 
= worstcurrentinstarrs-ISNULL((SELECT MAX(a.arrears/a.instalamount) 
FROM accountmonths2 a, custacct ca 
WHERE ca.custid =#BehaveDetails.custid 
AND a.acctno= ca.acctno 
AND ca.hldorjnt = 'H' AND a.instalamount >1 AND a.arrears >=0
AND a.currentmonth = @months12date),0)-- using 1 to be safe...

UPDATE #BehaveDetails SET worstcurrentstatusChangelast12Months =-2 
WHERE  worstcurrentstatusChangelast12Months <0

UPDATE #BehaveDetails SET worstcurrentstatusChangelast9Months =-2 
WHERE  worstcurrentstatusChangelast9Months <0

UPDATE #BehaveDetails SET worstcurrentstatusChangelast9Months =-3 
WHERE  NOT EXISTS (SELECT *
FROM accountmonths2 a, custacct ca 
WHERE ca.custid =#BehaveDetails.custid AND ca.hldorjnt ='H'
AND a.acctno= ca.acctno 
AND ca.hldorjnt = 'H' AND a.instalamount >1 AND a.outstbal >1
AND a.currentmonth =@months9date )-- using 1 to be safe...

UPDATE #BehaveDetails SET worstcurrentstatusChangelast12Months =-3 
WHERE  NOT EXISTS (SELECT * 
FROM accountmonths2 a, custacct ca 
WHERE ca.custid =#BehaveDetails.custid AND ca.hldorjnt ='H'
AND a.acctno= ca.acctno 
AND ca.hldorjnt = 'H' AND a.instalamount >1 AND a.outstbal >1
AND a.currentmonth = @months12date)-- using 1 to be safe...
SELECT * FROM #BehaveDetails --WHERE CustId = 'dt011060'
WHERE custid = 'LA160378' --SELECT TOP 3 * FROM ACCOUNTMONTHS2
UPDATE Delinquency 
SET numacctsarrears = b.numacctsarrears,
numactiveaccts=b.numactiveaccts,
arrearstotalPercent3months= b.arrearstotalPercent3months,
arrearstotalPercent9months= b.arrearstotalPercent9months,
balanceTotalPercent3months= b.balanceTotalPercent3months,
balanceTotalPercent9months= b.balanceTotalPercent9months,
monthssincelastGr1inarrears= b.monthssincelastGr1inarrears,
monthssincelastGr2inarrears = b.monthssincelastGr2inarrears,
worstcurrentstatusChangelast12Months= b.worstcurrentstatusChangelast12Months,
worstcurrentstatusChangelast9Months= b.worstcurrentstatusChangelast9Months,
worstintsarrslast6months= b.worstintsarrslast6months
FROM #BehaveDetails b 
WHERE b.custid = Delinquency.custid 
PRINT CONVERT(VARCHAR,@@ROWCOUNT) + ' Delinquency Rows updated'

--UPDATE Delinquency 
--SET BehaviouralScore = p.points,
--BScoreband = p.ScoringBand
--FROM proposal p 
--WHERE scorecard ='B' -- scored behaviourally
--AND  p.custid = delinquency.custid 
--AND p.dateprop = (SELECT MAX(pp.dateprop) FROM proposal pp 
--WHERE pp.custid = p.custid AND p.scorecard='B') 

--#13014 - Update from ProposalBs (Behavioural Score)
UPDATE Delinquency 
SET BehaviouralScore = pb.points,
BScoreband = pb.ScoringBand
FROM proposalbs pb 
where  pb.custid = delinquency.custid 
and pb.acctno = Delinquency.acctno
AND pb.dateprop = (SELECT MAX(pb1.dateprop) FROM proposalbs pb1 
WHERE pb1.custid = pb.custid AND pb1.acctno = pb.acctno) 
	
	print 'updated' + convert(varchar,@@rowcount) + ' behavioural rows'
GO 


 
