IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'BehaveScoreGetParameters')
	DROP PROCEDURE BehaveScoreGetParameters
GO 	
-- Procedure to return behavioural scoring information for a customer. Called from Dn_getscoredetails
-- AA 18 Feb 2010 Created
CREATE PROCEDURE BehaveScoreGetParameters 
--CR 1034 Behavioural scoring parameters
    @numacctsarrears					SMALLINT			OUTPUT,
    @numactiveaccts					SMALLINT		OUTPUT,
    @worstintsarrslast6months	 DECIMAL(20,5)  OUTPUT,
    @arrearstotalPercent3months	 DECIMAL(20,5)  OUTPUT,
    @arrearstotalPercent9months	 DECIMAL(20,5)  OUTPUT,
    @balanceTotalPercent3months  DECIMAL(20,5)   OUTPUT,  
    @balanceTotalPercent9months  DECIMAL(20,5)  OUTPUT, 
    @monthssincelastGr1inarrears DECIMAL(20,5)  OUTPUT, 
    @monthssincelastGr2inarrears DECIMAL(20,5)  OUTPUT,
    @worstcurrentstatusChangelast9Months   DECIMAL(20,5)  OUTPUT, 
    @worstcurrentstatusChangelast12Months   DECIMAL(20,5)  OUTPUT,
--CR 1034 End of Behavioural scoring parameters
    @custid VARCHAR(20) , 
    @countpcent FLOAT = 0 -- delivery percentage threshold 
AS 

	SET NOCOUNT ON 
	DECLARE @currentdate DATETIME 
	SET @currentdate = GETDATE()
	-- number of accounts in arrears with arrears and balance >0

	
	
	DECLARE @arrearstotal3months MONEY, @balancetotal3months MONEY , 
	@arrearstotal9months MONEY, @balancetotal9months MONEY  ,
	@arrearsat MONEY , @balanceat MONEY , @datefrom DATETIME ,
	@cacctno CHAR(12),@currentarrearstotal MONEY,
	@currentbalancetotal MONEY ,@instalment MONEY, @worstmonthsinarrears FLOAT 
	SET @arrearstotal3months=0 		SET @balancetotal3months=0
	SET @arrearstotal9months=0		SET @balancetotal9months=0
	SET @currentarrearstotal=0      SET @currentbalancetotal=0
	SET @worstmonthsinarrears =0

	IF @countpcent = 0 
		SELECT @countpcent = globdelpcent FROM country -- delivery percentage for arrears 
	
		-- create a table to hold temporary details - we will use this later
	DECLARE @accts TABLE (acctno CHAR(12), instalment MONEY, worststatus9mths FLOAT, 
	worststatus12mths FLOAT, worststatuscurrent FLOAT,DATEFIRST DATETIME,datesettled DATETIME, balance MONEY )
	-- loading up details for credit accounts but only if active in the past year...
	INSERT INTO @accts (		acctno,		instalment, DATEFIRST, datesettled, balance)  
	SELECT a.acctno ,i.instalamount , i.DATEFIRST , NULL,outstbal 
	FROM custacct ca , acct a ,instalplan i 
	WHERE ca.custid = @custid AND a.acctno= ca.acctno 
	AND ca.hldorjnt ='H' AND i.acctno= a.acctno AND i.instalamount >0  
	AND ( a.outstbal <>0 
	OR EXISTS (SELECT * FROM fintrans f WHERE f.acctno= a.acctno AND f.datetrans > DATEADD(MONTH,-13, @currentdate ))
	OR EXISTS (SELECT * FROM status s WHERE s.acctno= a.acctno AND s.datestatchge> DATEADD(MONTH,-13, @currentdate ))
	)

	UPDATE a 
	SET datesettled = (SELECT MAX(s.datestatchge)
	FROM status s 
	WHERE a.acctno = s.acctno AND s.statuscode = 'S')
	FROM @accts a WHERE  a.balance = 0
 
	DECLARE bhs_cursor CURSOR FAST_FORWARD READ_ONLY FOR
	SELECT acctno ,instalment  
	FROM @accts 
	OPEN bhs_cursor
	FETCH NEXT FROM bhs_cursor INTO @cacctno, @instalment 
	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @datefrom = DATEADD(MONTH,-12,@currentdate)
		EXEC DbArrearsCalc
		 @Arrears = @Arrearsat OUTPUT,	@Noupdates=1 , -- just gettting back the values not updating database
		    @datefrom =@datefrom,@OutStBal=@balanceat  OUTPUT,
			@AcctNo = @cAcctNo, @CountPcent = @CountPcent, --  float
			@NoDates = 0, @Return =0
		UPDATE @accts SET worststatus12mths = @arrearsat WHERE acctno= @cacctno 
		 			

		SET @arrearsat =0 SET @balanceat =0
		SET @datefrom = DATEADD(MONTH,-3,@currentdate)
		EXEC DbArrearsCalc
		 @Arrears = @Arrearsat OUTPUT,	@Noupdates=1 , -- just gettting back the values not updating database
		    @datefrom =@datefrom,@OutStBal=@balanceat  OUTPUT,
			@AcctNo = @cAcctNo, @CountPcent = @CountPcent, --  float
			@NoDates = 0, 	@Return =0
		SET @arrearstotal3months = @arrearstotal3months + ISNULL(@arrearsat,0)
		SET @balancetotal3months = @balancetotal3months + ISNULL(@balanceat,0)
		
		SET @datefrom = DATEADD(MONTH,-9,@currentdate) -- repeating for 9 months ago
		SET @arrearsat =0 SET @balanceat =0
		EXEC DbArrearsCalc
		 @Arrears = @Arrearsat OUTPUT,	@Noupdates=1 , -- just gettting back the values not updating database
		    @datefrom =@datefrom,@OutStBal=@balanceat  OUTPUT,
			@AcctNo = @cAcctNo, @CountPcent = @CountPcent, --  float
			@NoDates = 0, 	@Return =0
		SET @arrearstotal9months = @arrearstotal9months + ISNULL(@arrearsat,0)
		SET @balancetotal9months = @balancetotal9months + ISNULL(@balanceat,0)
		UPDATE @accts SET worststatus9mths = @arrearsat WHERE acctno= @cacctno 
		
		SET @datefrom = @currentdate  -- now doing current date
		SET @arrearsat =0 SET @balanceat =0
		EXEC DbArrearsCalc 
		 @Arrears = @Arrearsat OUTPUT,	@Noupdates=0 , --update database this time.
		    @datefrom =@datefrom,@OutStBal=@balanceat  OUTPUT,
			@AcctNo = @cAcctNo, @CountPcent = @CountPcent, --  float
			@NoDates = 0, @Return =0
		SET @currentarrearstotal = @currentarrearstotal + ISNULL(@arrearsat,0)
		SET @currentbalancetotal = @currentbalancetotal + ISNULL(@balanceat,0)
		UPDATE @accts SET worststatuscurrent = @arrearsat WHERE acctno= @cacctno 
		IF @arrearsat* 1.1 / @instalment > 1 --
		BEGIN
			IF @worstmonthsinarrears< @arrearsat* 1.1 / @instalment 
				SET @worstmonthsinarrears =@arrearsat/ @instalment  
		END
	FETCH NEXT FROM bhs_cursor INTO @cacctno, @instalment 

	END
	CLOSE bhs_cursor
	DEALLOCATE bhs_cursor

		SELECT @numacctsarrears = COUNT(*)
	FROM custacct ca, acct a ,instalplan i 
	WHERE ca.custid = @custid 
	AND ca.hldorjnt = 'H' AND a.Arrears > 1
	AND i.instalamount >0 AND i.acctno= a.acctno
	AND a.acctno= ca.acctno AND a.outstbal>0 
	-- active accounts have to have a delivery and have a balance outstanding
	SELECT @numactiveaccts	= COUNT(*)
	FROM custacct ca, acct a ,instalplan i 
	WHERE ca.custid = @custid AND i.acctno= a.acctno
	AND ca.hldorjnt = 'H' AND ca.acctno = a.acctno 
	AND a.outstbal >0 
	

		
	-- worst status number of instalments in arrears in last six months. 
	SELECT @worstintsarrslast6months=  	CEILING((ISNULL(MAX((d.arrears)/i.instalamount) * 1,-1000)))
	FROM @accts a ,instalplan i , agreement g ,ArrearsDaily d
	WHERE 
	g.acctno = i.acctno AND  a.acctno = g.Acctno AND d.Acctno = a.acctno
	AND g.datedel IS NOT NULL AND g.datedel < DATEADD(MONTH,1,@currentdate)
	AND d.datefrom > DATEADD(MONTH,-6,@currentdate)
	AND i.instalamount >0 
	
	-- Setting to 0 if only in advance
	IF @worstintsarrslast6months BETWEEN -999 AND 0 
		SET @worstintsarrslast6months=0
	IF @worstintsarrslast6months = -1000
		SET @worstintsarrslast6months = -1		
	IF @worstintsarrslast6months > 32000 -- going to store in smallint in end of day. 
		SET @worstintsarrslast6months = 30000
	-- now going to calculate arerars and balance its going to be awesome
	
	IF EXISTS (SELECT * FROM status s , custacct ca 
	WHERE s.acctno = ca.acctno AND ca.hldorjnt = 'H' AND ca.custid = @custid 
	AND s.datestatchge > DATEADD(MONTH,-6,GETDATE()) AND s.statuscode ='7')
		 SET @worstintsarrslast6months=999
		 
	
	IF EXISTS (SELECT * FROM fintrans s , custacct ca 
	WHERE s.acctno = ca.acctno AND ca.hldorjnt = 'H'
	AND s.datetrans > DATEADD(MONTH,-6,GETDATE()) AND s.transtypecode ='BDW'
	AND ca.custid= @custid )
		 SET @worstintsarrslast6months=999



	/*Arrears/Balance total percentages - Here we going to use -3 for insufficient exposure, -2 if currently in advance/up to date and -1 if currently in arrears*/ 
	IF @arrearstotal3months >0 AND @currentarrearstotal >0 
		BEGIN
			SET @arrearstotalPercent3months	=   @currentarrearstotal/@arrearstotal3months  * 100 
			--SELECT @currentarrearstotal AS 'curarrtot', @arrearstotal3months AS '3monthstot'
		END
    ELSE
		IF @currentarrearstotal > 0 -- currently in arrears but not previously 
			SET @arrearstotalPercent3months = -1
		ELSE IF EXISTS (SELECT * FROM @accts WHERE DATEFIRST < DATEADD(MONTH,-3,@currentdate) )
				SET @arrearstotalPercent3months = -2 
			ELSE -- insufficient exposure
				SET @arrearstotalPercent3months = -3 
	IF @currentarrearstotal <0
		SET @currentarrearstotal = 0	
	IF @arrearstotal9months >0 	AND @currentarrearstotal >=0 
		SET @arrearstotalPercent9months	 =   @currentarrearstotal/@arrearstotal9months  * 100 
	ELSE
		IF @currentarrearstotal > 0 -- currently in arrears
			SET @arrearstotalPercent9months = -1
		ELSE IF EXISTS (SELECT * FROM @accts WHERE DATEFIRST < DATEADD(MONTH,-9,@currentdate) )
				SET @arrearstotalPercent9months = -2 -- advance
			ELSE -- insufficient exposure
				SET @arrearstotalPercent9months = -3 
		
	
	IF @balancetotal3months >0 
	BEGIN
		IF @currentbalancetotal <0
			SET @currentbalancetotal = 0
		SET @balanceTotalPercent3months  = @currentbalancetotal/@balancetotal3months * 100 
	END
	ELSE 	
		SET @balanceTotalPercent3months  = -3  -- insufficent exposure
	
	IF @balancetotal9months >0
	BEGIN
		IF @currentbalancetotal <0
				SET @currentbalancetotal = 0 -- need to prevent this being negative 
		SET @balanceTotalPercent9months  = @currentbalancetotal/@balancetotal9months * 100 	
	END
    ELSE 
        SET @balancetotal9months= -3		-- insufficent exposure
	

	IF @worstmonthsinarrears <2 -- so currently not in  2 instalments arrears get the last period when 2 inst arrears
	BEGIN -- we are gettting the date the status was reduced to 1 or 2 after having been higher.
		SELECT @monthssincelastGr2inarrears = DATEDIFF(DAY,MIN(datefrom),@currentdate)/30.44  -- average number of days in month
		FROM ArrearsDaily d ,custacct ca ,instalplan i 
		WHERE ca.custid = @custid AND ca.hldorjnt = 'H' 
		AND d.acctno= ca.acctno AND  d.arrears/2 > i.instalamount 
		AND i.acctno= d.acctno 
		
		-- Issue is that we currently are only storing 3 months worth of data in the arrears daily table - so we need to look at status codes instead. 
		IF ISNULL(@monthssincelastGr2inarrears,0) <12 AND EXISTS (SELECT * FROM custacct ca, acct a WHERE ca.custid = @custid AND ca.hldorjnt='H' AND dateacctopen < DATEADD(MONTH,-13,@currentdate)
			AND NOT EXISTS (SELECT * FROM ArrearsDaily d WHERE d.acctno= a.acctno AND d.datefrom < DATEADD(MONTH,-12,@currentdate )))
		BEGIN
			SELECT @monthssincelastGr2inarrears = DATEDIFF(DAY,MIN(datestatchge),@currentdate)/30.44  -- average number of days in month
			FROM status s ,custacct ca 
			WHERE ca.custid = @custid AND ca.hldorjnt = 'H' 
			AND s.acctno= ca.acctno AND  s.statuscode IN ('1','2') 
			AND s.datestatchge >= 
			(SELECT MAX(s3.datestatchge) FROM status s3 
			  WHERE s.acctno= s3.acctno AND s3.statuscode IN ('3','4','5') )
			  SET @monthssincelastGr2inarrears = FLOOR((@monthssincelastGr2inarrears))
		END   
		
		  --need to have a look at the arrears daily table as going forward we will be looking at that... 
	  
		  IF @worstmonthsinarrears =0  -- not currently in arrears so get months since > 1 instalment in arrears
		  BEGIN
				SELECT @monthssincelastGr1inarrears = DATEDIFF(DAY,MIN(datefrom),@currentdate)/30.44  -- average number of days in month
				FROM ArrearsDaily d ,custacct ca ,instalplan i 
				WHERE ca.custid = @custid AND ca.hldorjnt = 'H' 
				AND d.acctno= ca.acctno AND  d.arrears> i.instalamount 
				AND i.acctno= d.acctno 
			-- again checking if missing arrears daily records in which case checking status	
			IF ISNULL(@monthssincelastGr1inarrears,0) <12 AND EXISTS (SELECT * FROM custacct ca, acct a WHERE ca.custid = @custid AND ca.hldorjnt='H' AND dateacctopen < DATEADD(MONTH,-13,@currentdate)
				AND NOT EXISTS (SELECT * FROM ArrearsDaily d WHERE d.acctno= a.acctno AND d.datefrom < DATEADD(MONTH,-12,@currentdate ) ))
			BEGIN 
				SELECT @monthssincelastGr1inarrears = DATEDIFF(DAY,MIN(datestatchge),@currentdate)/30.44
				FROM status s ,custacct ca 
				WHERE ca.custid = @custid AND ca.hldorjnt = 'H' 
				AND s.acctno= ca.acctno AND  s.statuscode in ('1','S') 
				AND s.datestatchge >= 
				(SELECT MAX(s3.datestatchge) FROM status s3 
				  WHERE s.acctno= s3.acctno AND s3.statuscode IN ('2','3','4','5') )
				  SET @monthssincelastGr1inarrears = FLOOR((@monthssincelastGr1inarrears))
			END 	  	
		  END
		  ELSE 
		  BEGIN -- is in arrears so set this to 0. 
		  		SET @monthssincelastGr1inarrears =0
		  END
			
	END
	ELSE 
	BEGIN
		SET @monthssincelastGr2inarrears =0
		SET @monthssincelastGr1inarrears =0
	END
		
		
	IF ISNULL(@monthssincelastGr2inarrears,1000) = 1000
		SET @monthssincelastGr2inarrears = 1000
	
	IF ISNULL(@monthssincelastGr1inarrears,1000) = 1000
		SET @monthssincelastGr1inarrears = 1000
				
	-- To do do we need to floor or raise these - awaiting feedback from Scorex. 
	--UPDATE @accts SET worststatus12mths = floor (worststatus12mths ), etc...
	-- worst status change insufficient exposure = -3, -2 if status decrease  
	IF EXISTS (SELECT * FROM @accts WHERE DATEFIRST < DATEADD(MONTH,-9,@currentdate) )
	BEGIN
		SELECT  @worstcurrentstatusChangelast9Months =   MAX(worststatuscurrent/instalment- worststatus9mths/instalment)
		FROM @accts 
		IF @worstcurrentstatusChangelast9Months < 0 
			SET  @worstcurrentstatusChangelast9Months = -2
	END
	ELSE 
		SET  @worstcurrentstatusChangelast9Months = -3
	-- check whether outstanding accounts existed. 
	IF EXISTS (SELECT * FROM @accts WHERE DATEFIRST < DATEADD(MONTH,-12,@currentdate) AND ISNULL(datesettled,@currentdate) >DATEADD(MONTH,-12,@currentdate)   )
	BEGIN
		SELECT  @worstcurrentstatusChangelast12Months =   MAX( worststatuscurrent/instalment-worststatus12mths/instalment)
		FROM @accts 
		IF @worstcurrentstatusChangelast12Months < 0  
				SET  @worstcurrentstatusChangelast12Months = -2
	END
	ELSE 
		SET  @worstcurrentstatusChangelast12Months = -3

GO 
