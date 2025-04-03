IF EXISTS (SELECT * FROM sysobjects WHERE NAME ='BlockandRescoreBehaviouralCredit') 
DROP PROCEDURE BlockandRescoreBehaviouralCredit 
GO 
CREATE PROCEDURE BlockandRescoreBehaviouralCredit @rescoredate DATETIME 
AS 
SET NOCOUNT ON 
DECLARE @datefrom DATETIME , @dateto DATETIME
, @previousrunno INT 

SELECT @previousrunno=ISNULL(MAX(runno),0) FROM 
interfacecontrol 
WHERE interface = 'BHSRescore' AND result = 'P'

SELECT @datefrom = datestart 
FROM interfacecontrol 
WHERE interface = 'BHSRescore'
AND runno= @previousrunno 

IF @datefrom = NULL 
	SET @datefrom = GETDATE()
	-- actually lets get any account with a repo or bdw in the past month -- they should be blocked
	SET @datefrom = DATEADD(MONTH,-1,@datefrom)
DECLARE @blockmonths SMALLINT
	SELECT @blockmonths = CONVERT(SMALLINT,value) FROM CountryMaintenance WHERE 	codename ='blockcreditmonths'
	
	SELECT c.custid
	INTO #blockaccts 
	FROM customer c
	join custacct ca ON c.custid = ca.custid 
	JOIN acct a ON a.acctno= ca.acctno 
	JOIN instalplan i ON i.acctno= ca.acctno 
	WHERE ca.hldorjnt = 'H'
	AND c.creditblocked = 0 AND i.instalamount >0 
	AND a.arrears /i.instalamount >= @blockmonths 
	
	INSERT INTO #blockaccts (custid) 
	SELECT c.custid
	FROM customer c
	join custacct ca ON c.custid = ca.custid 
	JOIN acct a ON a.acctno= ca.acctno 
	JOIN instalplan i ON i.acctno= ca.acctno 
	WHERE ca.hldorjnt = 'H'
	AND c.creditblocked = 0 AND i.instalamount >0 
	AND EXISTS (SELECT * FROM fintrans f WHERE f.datetrans >@datefrom 
	AND transtypecode IN ('bdw','rep') AND f.acctno= a.acctno)
	GROUP BY c.custid 	
	
	
	INSERT INTO customer_rescore (
		custid,
		date_rescore
	) 
	SELECT custid ,@rescoredate FROM #blockaccts b
	WHERE  NOT EXISTS (SELECT * FROM customer_rescore r 
	WHERE r.custid = b.custid)
	GROUP BY custid
	
	UPDATE customer SET creditblocked = 1 WHERE EXISTS 
	(SELECT * FROM #blockaccts b WHERE b.custid = customer.custid)
	
	-- now lets unblock those customers who don't have arrears...
	SELECT custid INTO #unblock FROM customer
	WHERE creditblocked = 1 
	AND NOT EXISTS -- so no accounts in arrears
	(SELECT ca.custid FROM 
	 custacct ca
	JOIN acct a ON a.acctno= ca.acctno 
	JOIN instalplan i ON i.acctno= ca.acctno 
	WHERE ca.hldorjnt = 'H'
	AND i.instalamount >0 AND a.outstbal >0
	AND a.arrears /i.instalamount >= @blockmonths 
	AND ca.custid = customer.custid)
	AND NOT EXISTS -- exclude those accounts which have had a repo or BDW ever
	(SELECT * FROM  custacct ca WHERE ca.hldorjnt = 'H'
	AND ca.custid = customer.custid
	AND EXISTS (SELECT SUM(transvalue ) FROM fintrans f WHERE /*f.datetrans >@datefrom  really shouldn't ever unblock accounts which have had BDW's and Repos
	AND*/ transtypecode IN ('bdw','rep','rdl') AND f.acctno= ca.acctno
	GROUP BY f.acctno 
	HAVING SUM(f.transvalue ) <0  )) -- But if reversed then allow this according to BH spec
	

	INSERT INTO customer_rescore (
		custid,
		date_rescore
	) 
	SELECT custid ,@rescoredate FROM #unblock u
	WHERE  NOT EXISTS (SELECT * FROM customer_rescore r 
	WHERE r.custid = u.custid)
	GROUP BY custid 
	
	UPDATE customer SET creditblocked = 0 
	WHERE EXISTS (SELECT * FROM #unblock b
	WHERE b.custid = customer.custid) 
	AND creditblocked = 1 and not exists
	(SELECT * FROM custacct ca JOIN acct t ON ca.acctno = t.acctno
	WHERE ca.hldorjnt ='H' AND t.accttype ='T' AND t.outstbal >0
	AND ca.custid = customer.custid)



GO 
