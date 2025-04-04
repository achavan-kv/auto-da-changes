
IF EXISTS (SELECT * FROM sysobjects 
		   WHERE NAME = 'usp_SCDE_Calculate_Arrears_Balance_Days'
		   AND xtype = 'p')
BEGIN
	DROP PROCEDURE usp_SCDE_Calculate_Arrears_Balance_Days
END
GO
CREATE PROCEDURE [dbo].[usp_SCDE_Calculate_Arrears_Balance_Days]
@CUSTID VARCHAR(20),
@PERIOD  INT
AS
BEGIN  TRY
--DECLARE @CUSTID VARCHAR(20) ='780829-0048',
--@PERIOD  INT =17

IF OBJECT_ID('tempdb..#TEMPEXPORT') IS NOT NULL
		 BEGIN 
			DROP TABLE #TEMPEXPORT
		 END 
DECLARE @FIRSTDATEOFCURRENTMONTH DATE =  DATEADD(MONTH, DATEDIFF(MONTH, 1, GETDATE()), 0)	--2019-07-01 00:00:00.000
DECLARE @ONEMONTHBACKDATE		 DATE=	 DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-1, 0)	--2019-06-01 00:00:00.000 (1 Month back)
DECLARE @TWOMONTHSBACKDATE		 DATE=	 DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-2, 0)	--2019-06-01 00:00:00.000 (2 Month back)
DECLARE @THREEMONTSBACKDATE		 DATE=   DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-3, 0)	--2019-04-01 00:00:00.000 (3 Month back)
DECLARE @FOURMONTHSBACKDATE		 DATE=	 DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-4, 0)	--2019-06-01 00:00:00.000 (4 Month back)
DECLARE @FIVEMONTHSBACKDATE		 DATE=	 DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-5, 0)	--2019-06-01 00:00:00.000 (5 Month back)
DECLARE @SIXMONTHSBACKDATE		 DATE=   DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-6, 0)	--2019-01-01 00:00:00.000 (6 Month back)
DECLARE @TWELVEMONTHSBACKDATE	 DATE=   DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE())-12, 0)--2019-04-01 00:00:00.000 (12 Month back)

DECLARE @DATESTART DATE =  CAST(EOMONTH(DATEADD(MONTH, - (@PERIOD+1), @FIRSTDATEOFCURRENTMONTH))AS DATE)  --'2017-03-31'
DECLARE @DATEEND DATE = @FIRSTDATEOFCURRENTMONTH  --'2018-08-01'
DECLARE @TABLEVAR TABLE
	 (EXTRACT_DATE DATE,
	  CUSTOMER_ID VARCHAR(20),
	  ACCOUNT_NUMBER CHAR(12),
	  DATE_ACCOUNT_OPENED DATE,
	  OUTSTANDING_BALANCE MONEY,
	  DATE_OBS DATE,
	  BALANCE_ARREARS MONEY,
	  DAYS_ARREARS INT,
	  AGREEMENT_TOTAL MONEY,
	  PERC_OUTS DECIMAL(25,15),
	  PERC_OUTSARREARS  DECIMAL(25,15)
	  )
DECLARE @NUMBEROFMONTHS SMALLINT

SELECT	CUSTOMERID,
		ACCOUNTNUMBER,
		DATE_ACCOUNT_OPENED,
		PERIOD_CLOSING_DATE,
		NUMINSTAL,
		AGREEMENT_TOTAL,
		OUTSTANDING_BALANCE,
		PASTDUE_BALANCE,
		DAYS_IN_ARREARS,
		CANCELATION_DATE,
		INSTALMENT,
		PAID_INSTALLMENTS,
		OUTSTANFING_INSTALLMENTS,
		BRANCHNO,
		TERMS_TYPE,
		LAST_PAYMENT_DATE
INTO #TEMPEXPORT
FROM (
		SELECT CA.CUSTID AS CUSTOMERID,
			   A.ACCTNO AS ACCOUNTNUMBER,
			   CAST(A.DATEACCTOPEN AS DATE) AS DATE_ACCOUNT_OPENED,
			   CASE 
				   WHEN DATEPART(MONTH, DATEADD(DAY, DATEPART(DAY, I.[DATEFIRST]) - 1, @DATESTART)) = DATEPART(MONTH, @DATESTART)    --DUEDAY IS NOT CORRECT FOR MOST ACCOUNTS...
						THEN DATEADD(DAY, DATEPART(DAY, I.[DATEFIRST]) - 1, @DATESTART)
				   ELSE DATEADD(DAY, -1, @DATEEND)
			   END AS PERIOD_CLOSING_DATE,
			   I.INSTALNO AS NUMINSTAL,
			   A.AGRMTTOTAL AS AGREEMENT_TOTAL,
			   CAST(0 AS MONEY) AS OUTSTANDING_BALANCE,
			   CAST(0 AS MONEY) AS PASTDUE_BALANCE,
			   0 AS DAYS_IN_ARREARS,
			   CAST(NULL AS DATE) AS CANCELATION_DATE,
			   I.INSTALAMOUNT AS INSTALMENT,
			   CAST(0.00 AS MONEY) AS PAID_INSTALLMENTS,
			   CAST(0.00 AS MONEY) AS OUTSTANFING_INSTALLMENTS,
			   LEFT(A.ACCTNO, 3) AS BRANCHNO,
			   A.TERMSTYPE AS TERMS_TYPE,
			   CAST(NULL AS DATE) AS LAST_PAYMENT_DATE,
			   COALESCE(S.STATUSCODE, A.CURRSTATUS) AS CURRENTSTATUS
		FROM CUSTACCT CA 
		INNER JOIN ACCT A
		ON CA.ACCTNO = A.ACCTNO 
		INNER JOIN AGREEMENT AG  
		ON AG.ACCTNO = A.ACCTNO 
		AND	CA.HLDORJNT = 'H'
		AND	CA.CUSTID = @CUSTID
		AND (AG.DATEDEL >= @DATESTART OR A.DATELASTPAID>= @DATESTART)
		AND (AG.DATEDEL < @DATEEND OR AG.DATEDEL < @DATEEND)
		AND A.ACCTTYPE NOT IN ('C', 'S') --AND A.CURRSTATUS != 'S'
		INNER JOIN INSTALPLAN I
		ON A.ACCTNO = I.ACCTNO
		LEFT OUTER JOIN [STATUS] S
		ON S.ACCTNO = CA.ACCTNO
		AND S.DATESTATCHGE = (SELECT TOP 1 DATESTATCHGE 
								  FROM [STATUS] 
								  WHERE ACCTNO = S.ACCTNO 
									AND CAST(DATESTATCHGE AS DATE) <= @DATESTART ORDER BY DATESTATCHGE DESC)									
		 ) AS A
   -- WHERE A.CURRENTSTATUS != 'S'
------------------------------------------------------------   


DECLARE @ENDDATEOFLASTMONTH DATE  =  EOMONTH(@FIRSTDATEOFCURRENTMONTH,-1)  --30 -APRIL 2019
SET @FIRSTDATEOFCURRENTMONTH  =  DATEADD(MONTH, DATEDIFF(MONTH, 1, GETDATE()), 0) --1 JUNE 2019
SET @DATEEND  =   DATEADD(MONTH, -17, @ENDDATEOFLASTMONTH)  --17 MONTHS BACK DATE
		
DECLARE  
		@COUNTPCENT     FLOAT       = 75,   
		@NODATES        SMALLINT    = 0,  
		@ARREARS        MONEY       = 0.0   ,  
		@NOUPDATES       BIT = 0 ,   
		@DATEFROM  DATETIME = '1-JAN-1900',  
		@OUTSTBAL       MONEY = 0 ,  
		@RETURN         INTEGER     = 0  
		
DECLARE @ACCTNO CHAR(12),
		@CUSTOMERID VARCHAR(20), 
		@DATE_ACCOUNT_OPENED DATE,
		@AGREEMENT_TOTAL MONEY 

DECLARE RESULTS_CURSOR CURSOR FORWARD_ONLY FOR 
SELECT  CUSTOMERID, 
		ACCOUNTNUMBER,
		DATE_ACCOUNT_OPENED,
		AGREEMENT_TOTAL 
FROM #TEMPEXPORT
	 OPEN RESULTS_CURSOR
		FETCH NEXT FROM RESULTS_CURSOR 
		INTO 
		@CUSTOMERID, 
		@ACCTNO,
		@DATE_ACCOUNT_OPENED,
		@AGREEMENT_TOTAL 

	WHILE @@FETCH_STATUS = 0    
		BEGIN  		
----------------------------NK------------------------------------------	
---ARREARS CALCULATION 
		WHILE ( @ENDDATEOFLASTMONTH > @DATEEND)
			BEGIN
				SET @DATEFROM = @ENDDATEOFLASTMONTH  ---GETDATE()   --LAST DATE 
					-- LOCAL VARIABLES  
					DECLARE     
					@AGRMTNO        INT,  
					@I              INTEGER,   
					@DTE            DATETIME,		@DI             INTEGER,   
					@DN             INTEGER,		@DB             INTEGER,   
					@MI             INTEGER,		@YI             INTEGER,   
					@MN             INTEGER,		@YN             INTEGER,   
					@NSD            INTEGER,		@TOT            INTEGER,   
					@MON            MONEY,			@OWED           MONEY,   
					@PAID           MONEY,			@D1             DATETIME,   
					@D2             DATETIME,		@BNO            SMALLINT,   
					@ATYPE          CHAR(1),   
					@ACCOUNTNO      VARCHAR(12),    @DATEFIRST      DATETIME,   
					@TTYPE          VARCHAR(2),     @DEPOSIT        MONEY,   
					@DATELAST       DATETIME,		@INSTALNO       INTEGER,   
					@AS400BAL       MONEY,			@INSTALAMOUNT   MONEY,   
					@DATEAGRMT      DATETIME,		@AGRMTTOTAL     MONEY,   
					@TRANSVALUE     MONEY,			@DATEACCTOPEN   DATETIME,   
					@DPAID          DATETIME,		@INSTALPREDEL   CHAR(1),   
					@CURRSTATUS     CHAR(1),		@DATEDEL        DATETIME,   
					@DATENEXTDUE    DATETIME,		@DND            DATETIME,   
					@STATE          INTEGER,		@ZERO           TINYINT,  
				    @MTHSDEFERRED   SMALLINT,		@DELTOT   MONEY,
				    @TODAY			DATETIME,
				    @ISREADYASSIST  BIT		--#19284 - CR15594
	  
					DECLARE @INSTALMENTWAIVED BIT  --CR1090  
   
					SET NOCOUNT ON  
					SET @AGRMTNO        = 0;  
					SET @I              = 0;  
					SET @DTE            = '1900-01-01';  
					SET @DI             = 0;  
					SET @DN             = 0;  
					SET @DB             = 0;  
					SET @MI             = 0;  
					SET @YI             = 0;  
					SET @MN      = 0;  
					SET @YN             = 0;  
					SET @NSD            = 0;  
					SET @TOT            = 0;  
					SET @MON            = 0;  
					SET @OWED           = 0;  
					SET @PAID           = 0;  
					SET @D1             = '1900-01-01';  
					SET @D2             = '1900-01-01';  
					SET @BNO            = 0;  
					SET @OUTSTBAL       = 0;  
					SET @ATYPE          = ' ';  
					SET @TTYPE          = ' ';  
					SET @ARREARS        = 0;  
					SET @DEPOSIT        = 0;  
					SET @DATEFIRST      = '1900-01-01';  
					SET @DATELAST       = '1900-01-01';  
					SET @INSTALNO       = 0;  
					SET @AS400BAL       = 0;  
					SET @DATEAGRMT      = '1900-01-01';  
					SET @AGRMTTOTAL     = 0;  
					SET @TRANSVALUE     = 0;  
					SET @DATEACCTOPEN   = '1900-01-01';  
					SET @DPAID          = '1900-01-01';  
					SET @CURRSTATUS     = ' ';  
					SET @DATEDEL        = '1900-01-01';  
					SET @DATENEXTDUE    = '1900-01-01';  
					SET @DND            = '1900-01-01';  
					SET @STATE = 0;  
					SET @ZERO           = 0;  
					SET @TODAY =  DBO.STRIPTIME(@DATEFROM)
  
					SELECT  @BNO = BRANCHNO    
					FROM    SERVER;   
  
					SELECT  @ATYPE          = ACCTTYPE,   
							@TTYPE          = ISNULL(TERMSTYPE, '00'),  
							@DATEACCTOPEN   = ISNULL(DATEACCTOPEN, ''),  
							@AS400BAL       = ISNULL(AS400BAL, 0),  
							@DPAID          = ISNULL(DATELASTPAID, ''),  
							@AGRMTTOTAL     = AGRMTTOTAL,   
							@CURRSTATUS     = CURRSTATUS   
					FROM    ACCT   
					WHERE   @ACCTNO = ACCTNO   
					SET @RETURN = 0  
					IF @ACCTNO LIKE '___5%' -- SPECIAL ACCOUNTS DON'T HAVE ARREARS  
					   -- RETURN 0  
					SET @MTHSDEFERRED = 0 					 
	 
					SELECT @MTHSDEFERRED =ISNULL (MTHSDEFERRED,0) 
						FROM ACCTTYPE WHERE ACCTTYPE =@ATYPE  					 
	 
					SELECT  @OUTSTBAL = ISNULL(SUM(TRANSVALUE), 0)   
						FROM    FINTRANS   
						WHERE   ACCTNO = @ACCTNO   
						AND DATETRANS <=DATEADD(SECOND,5,@DATEFROM) --IP - 21/01/11 - #2926 - OUTSTBAL ON ACCT TABLE NOT BEING UPDATED - REMOVED CODE  
 
				    /*  
				    CALCULATE FOR STORE CARD ACCOUNTS  
				    */  				  
					IF @ACCTNO LIKE    '___9%'  
					BEGIN  
	
					    SELECT @DELTOT = SUM(TRANSVALUE)  
							FROM FINTRANS  
							WHERE ACCTNO = @ACCTNO  
							AND TRANSTYPECODE IN ('SCT')  
							AND DATETRANS <=DATEADD(SECOND,5,@DATEFROM)  			  
						/*  
						CALCULATE ARREARS FOR STORE CARD ACCOUNTS  
						*/ 
						SELECT @ARREARS =  ISNULL(CASE WHEN DATEDUE > GETDATE()
							THEN ISNULL(OUTSTMINPAY, 0)	+ PAYMENTS						--IP - 20/03/12 - #9805
							ELSE CURRMINPAY + PAYMENTS
							END , 0)
							FROM VW_STORECARD_ARREARS 
							WHERE ACCTNO=@ACCTNO			-- JEC 16/02/12 
  
						IF @ARREARS < 0  
							SET @ARREARS = 0 
					END  

						SELECT  @DELTOT = SUM(TRANSVALUE)  
						FROM  FINTRANS   
						WHERE  TRANSTYPECODE IN ('ADD',   
						'DEL',   
						'GRT','CLD')  --JEC CR1232 #3291  
						AND ACCTNO = @ACCTNO   
						AND DATETRANS <=@DATEFROM
					IF @CURRSTATUS ='S' AND  @OUTSTBAL= 0  
						 BEGIN  
							  IF SUBSTRING(@ACCTNO, 4, 1) = N'4' AND @NOUPDATES = 0   
								   BEGIN  
										DECLARE @MAXDATETRANS DATETIME  
										SET @MAXDATETRANS = (SELECT MAX(ISNULL(DATETRANS,'1900-01-01')) FROM FINTRANS F WHERE F.ACCTNO = @ACCTNO AND TRANSTYPECODE IN ('DEL')   
										AND DATETRANS <=@DATEFROM )
										SET @RETURN = 0  
									   -- RETURN 0  
								   END  
							  ELSE  
								   BEGIN  
										SET @RETURN = 0  
									   -- RETURN 0  
								   END  
						 END 				  
					SELECT  @DEPOSIT        = DEPOSIT,  
							@DATEDEL        = ISNULL(DATEDEL, ''),  
							@DATEAGRMT      = DATEAGRMT,   
							@DATENEXTDUE    = DATENEXTDUE,   
							@AGRMTNO        = AGRMTNO   
					FROM    AGREEMENT   
					WHERE   ACCTNO = @ACCTNO;
	
					-- CASH ACCOUNTS OR AGREEMENT TOTAL 0  
					IF  @AGRMTTOTAL = 0 OR SUBSTRING(@ACCTNO, 4, 1) = '4'   
					BEGIN    
						IF SUBSTRING(@ACCTNO, 4, 1) = '4'  
							BEGIN  
								IF @AS400BAL > 0   
									BEGIN   
										SELECT  @PAID = ISNULL(SUM(TRANSVALUE), 0)   
										FROM    FINTRANS    
										WHERE   ACCTNO = @ACCTNO AND DATETRANS <=@DATEFROM   
										AND     TRANSTYPECODE NOT IN  
											('DEL',   
											'GRT',   
											'REP',   
											'ADD',  
											'RFN',   -- CR976   
											'CLD',  --JEC CR1232 #3291 LOAN DISBURSEMENT  
											'RPO',   
											'RDL');   
									SET @ARREARS = @AS400BAL + @PAID;   
							END    
							ELSE  
								BEGIN    
									SET @ARREARS = 0;   
								END 							 
						END    
						ELSE  
							BEGIN    
								 IF @OUTSTBAL= 0     
									BEGIN  
										SET @ARREARS = @OUTSTBAL;   
									END  
								 ELSE  
									BEGIN  
										SET @ARREARS = 0;   
									END  
							END 			   
						-- ** FINISHED **  
						SET @RETURN = 0  
						--RETURN 0;  
		  
					END  /* IF @AGRMTTOTAL = 0 OR SUBSTRING(@ACCTNO, 4, 1) = '4' */  
					-- VARIABLE INSTALMENTS - UPDATE CORRECT INSTALAMOUNT
	
					SELECT  @DATEFIRST      = DATEFIRST,   
							@DATELAST       = DATEADD(MONTH,INSTALNO-1,DATEFIRST) , -- 72010 MATCHING END OF DAY ARREARS CALC DATELAST   
							@INSTALNO       = INSTALNO,   
							@INSTALAMOUNT   = INSTALAMOUNT,  
							@INSTALMENTWAIVED = INSTALMENTWAIVED  -- CR1090   
					FROM    INSTALPLAN   
					WHERE   ACCTNO = @ACCTNO;   

					/* IF DELIVERY DATE LESS THAN THESE DATES THEN NOT FULLY DELIVERED */  
					IF @DATEDEL    <'1-JAN-1910'    
					OR @DATEDEL     IS NULL  
					OR @DATEFIRST   <'1-JAN-1910'      
					OR @DATELAST    <'1-JAN-1910'  
					OR @DATELAST    IS NULL  
						BEGIN   
							IF @OUTSTBAL < 0  
								BEGIN  
									SET @ARREARS = @OUTSTBAL;   
								END  
							ELSE  
								BEGIN  
									SET @ARREARS = 0;   
								END   
							SET @RETURN = 0 
						END 		    				  
  
					IF @INSTALAMOUNT IS NULL OR @INSTALAMOUNT = 0  
						BEGIN 
							IF @CURRSTATUS != 'U' AND @CURRSTATUS != 'S'  
								BEGIN   
									SET @RETURN = 0 
								END  
						END  
  
					-- IF LESS THAN 7 AM USE PREVIOUS DAY  
					IF (DATEPART(HOUR,GETDATE()) <7 )  
						SELECT @D1 = DATEADD(HOUR,-1-DATEPART(HOUR,GETDATE()),GETDATE());  
					ELSE -- CURRENT DAY  
						SELECT @D1 = GETDATE()  
	   
					-- @?I ARE FROM INSTALPLAN.DATEFIRST  
					SET @DI = DATEPART(DAY,   @DATEFIRST);   
					SET @MI = DATEPART(MONTH, @DATEFIRST);   
					SET @YI = DATEPART(YEAR,  @DATEFIRST);  				  
  
					 DECLARE @REPOS MONEY -- WE ARE GOING TO CHECK REPOS - IF THESE PUT THE ACCOUNT INTO CREDIT THEN WE WILL NOT ZEROISE THE ARREARS.  
					 -- REPOS AND REDELIVERIES AFTER REPO  
					 SELECT @REPOS = ISNULL(SUM(TRANSVALUE ),0) FROM FINTRANS WHERE ACCTNO = @ACCTNO AND TRANSTYPECODE IN ('REP','RDL')   
					 -- WILL BE -VE CREDIT VALUE SO NEED TO INCREASE REAL BALANCE -- WILL BE PLUS 

					-- IS CURRENT DATE< DATE OF LAST INSTALMENT  
					IF @D1 < @DATELAST OR @DATELAST = ''  
						BEGIN  
						-- @?N ARE FROM CURRENT DATE  
							SET @DN = DATEPART(DAY,   @DATEFROM);   
							SET @MN = DATEPART(MONTH, @DATEFROM);   
							SET @YN = DATEPART(YEAR,  @DATEFROM);   
  
						-- IF DAY NOW IS AFTER DATEFIRST DAY SET DATE BEFORE (@DB) TO 1  
							IF @DN - @DI > 0  
								BEGIN  
								   SET @DB = 1;   
								END  
							ELSE  
										BEGIN  
								SET @DB = 0;   
							END  
						END  
					ELSE -- ALL DUE SO ARREARS EQUALS OUTSTANDING BALANCE...   
						BEGIN   
							--SET @ARREARS = ISNULL(@OUTSTBAL, 0) -@REPOS;   
							 SET @ARREARS = CASE WHEN ISNULL(@OUTSTBAL, 0) <= 0 THEN 0 ELSE ISNULL(@OUTSTBAL, 0) -@REPOS END;  --IP - 02/04/12 - #9857 
  							 IF  @CURRSTATUS != 'U' AND @CURRSTATUS != 'S'  
								BEGIN   
									SET @RETURN = 0 
								END  
						END 
					-- CALC NUMBER OF MONTHS SINCE DATEFIRST TO GET NUMBER OF INSTALMENTS DUE  
					--  YEAR (NOW)- YEAR (DATEFIRST) + MONTH (NOW) - MONTH (DATEFIRST) + 1 (IF DAY (DATEFIRST) BEFORE DAY (NOW))   
					SET @NSD = 12*(@YN-@YI)+@MN-@MI+@DB; 

					IF @DATEFIRST = ''  
						BEGIN  
							SET @NSD = 0;   
						END  

					--#19284 - CR15594
					IF EXISTS(SELECT * FROM READYASSISTDETAILS	WHERE ACCTNO = @ACCTNO	AND (STATUS IS NULL OR STATUS = 'ACTIVE'))
						BEGIN		
							SET @ISREADYASSIST = 1
						END
					ELSE
						BEGIN
							SET @ISREADYASSIST = 0
						END				   
					SELECT  @INSTALPREDEL = ISNULL(INSTALPREDEL,'N')    
					FROM    TERMSTYPE   
					WHERE   TERMSTYPE = @TTYPE;

					IF @INSTALPREDEL = 'Y'  
						AND @INSTALMENTWAIVED=0  --CR1090 INSTALMENT NOT WAIVED  
						AND @ISREADYASSIST = 0   --#19284 - CR15594 - AND NOT A READY ASSIST
						BEGIN  
							IF CAST(@DEPOSIT AS MONEY) = 0  
							BEGIN  
								SET @DEPOSIT = @INSTALAMOUNT;   
							END  
						END  
					IF @NSD < 1  
						BEGIN  
							SET @NSD = 0;   
						END  					  
					IF @OUTSTBAL - @REPOS <= 0 OR @OUTSTBAL IS NULL  
						BEGIN  
						SET @NSD = 0;   
						END  				  
						SET @MON = @NSD * ISNULL(@INSTALAMOUNT, 0)
	  
					DECLARE @BALANCEDUE MONEY  
					-- FOR VARIABLE INSTALMENTS STORE DIFFERENTLY  
					IF EXISTS (SELECT * FROM INSTALMENTVARIABLE WHERE ACCTNO =@ACCTNO)  
						BEGIN     
								--' INSTALMENTS FOR PREVIOUS VARIABLE '   
								SELECT @BALANCEDUE = ISNULL(SUM(INSTALMENTNUMBER * INSTALMENT),0) FROM INSTALMENTVARIABLE  
								WHERE DATETO <@DATEFROM AND DATEFROM >'1-JAN-1910' AND ACCTNO =@ACCTNO  
  
								 --'  INSTALMENTS FOR CURRENT VARIABLE '   
								SELECT @BALANCEDUE =@BALANCEDUE + ISNULL(DATEDIFF(MONTH,DATEFROM,@DATEFROM) * INSTALMENT,0) FROM INSTALMENTVARIABLE  
								WHERE DATETO >@DATEFROM  AND DATEFROM >'1-JAN-1910' AND DATEFROM <@DATEFROM AND ACCTNO =@ACCTNO  
  
								SET @MON = @BALANCEDUE  
						END  				  
					SET @MON = @MON + ISNULL(@DEPOSIT, 0);    

					IF  @ATYPE = 'M'   
						BEGIN  
							SET @DTE = (@DATEAGRMT);   
						END  
					ELSE  
						BEGIN  
							SET @DTE = '';   
						END  				  
					SELECT  @PAID = ISNULL(SUM(TRANSVALUE), 0)   
							FROM    FINTRANS   
							WHERE   ACCTNO = @ACCTNO   
							AND     TRANSTYPECODE NOT IN  
									('DEL',   
									'GRT',   
									'REP',   
									'ADD',  
									'RFN',    -- CR976  
									'CLD',  --JEC CR1232 #3291 LOAN DISBURSEMENT    
									'RPO',   
									'RDL')   
							AND DATETRANS > @DTE --;
							AND DATETRANS <=@DATEFROM;  
					-- INCLUDE REFINANCE DEPOSIT IN PAID AMOUNT      CR976 JEC 17/04/09  
					SELECT  @PAID = @PAID + ISNULL(SUM(TRANSVALUE), 0)   
						FROM    FINTRANS   
							WHERE   ACCTNO = @ACCTNO   
							AND     TRANSTYPECODE IN  
									('RFD')   
							AND TRANSVALUE<0      
							AND DATETRANS > @DTE --;
							AND DATETRANS <=@DATEFROM; 
					IF @PAID IS NULL  
						BEGIN  
							SET @PAID = 0;   
						END  
					IF  ABS(@PAID) >= @DEPOSIT AND @CURRSTATUS = 'U'  -- WAS @PAID > @DEPOSIT AND @CURRSTATUS = 'U'  
						BEGIN  
							SET @CURRSTATUS = '1';   
						END  				  
					IF @PAID IS NULL  
						BEGIN  
							SET @PAID = 0;   
						END  				  
					SET @ARREARS = @MON+@PAID; 
					IF @ARREARS <0
					   SET @ARREARS=0

		INSERT INTO @TABLEVAR (EXTRACT_DATE,CUSTOMER_ID , ACCOUNT_NUMBER , DATE_ACCOUNT_OPENED , OUTSTANDING_BALANCE ,
		DATE_OBS , BALANCE_ARREARS , DAYS_ARREARS , AGREEMENT_TOTAL)
		SELECT  @ENDDATEOFLASTMONTH ,@CUSTOMERID  ,@ACCTNO, @DATE_ACCOUNT_OPENED  ,@OUTSTBAL ,
		@FIRSTDATEOFCURRENTMONTH,
		@ARREARS,		
		CASE 
			WHEN @ARREARS=0
				THEN 0
			ELSE
				DATEDIFF(DAY,@DATEFIRST, @DATEFROM)
		END AS DAYS_ARREARS,	
		  @AGREEMENT_TOTAL	
		 WHERE  (@OUTSTBAL !=0)
		--WHERE NOT EXISTS (SELECT * FROM  ARREARSDAILY D WHERE D.ACCTNO =@ACCTNO AND D.ARREARS =@ARREARS AND D.DATETO IS NULL) AND @ARREARS > 0 

		SET	@ENDDATEOFLASTMONTH =   DATEADD(MONTH, -1, @ENDDATEOFLASTMONTH)	
END			
 ----------------------------------------------------------------- 
	FETCH NEXT FROM RESULTS_CURSOR     
	INTO 
			@CUSTOMERID, 
			@ACCTNO,
			@DATE_ACCOUNT_OPENED,
			@AGREEMENT_TOTAL

				SET @FIRSTDATEOFCURRENTMONTH  =  DATEADD(MONTH, DATEDIFF(MONTH, 1, GETDATE()), 0) --1 JUNE 2019
				SET @ENDDATEOFLASTMONTH    =  EOMONTH(@FIRSTDATEOFCURRENTMONTH,-1)  --30 -APRIL 2019
				SET @DATEEND  =   DATEADD(MONTH, -@PERIOD, @ENDDATEOFLASTMONTH)  --17 MONTHS BACK DATE	
				--SET @PERIOD=17
	END  -- END OF WHILE   
 CLOSE RESULTS_CURSOR;
 DEALLOCATE RESULTS_CURSOR;
 DROP TABLE #TEMPEXPORT
--SELECT * FROM @TABLEVAR  ORDER BY EXTRACT_DATE DESC
 --------------------------------------------------------------------------------------
 DECLARE 
		@Age									SMALLINT=0 ,    
		@Avg_agreement_total_1m_sq				float=0,    
		@Avg_balance_arrears_12m_ln				float=0,
		@Avg_balance_arrears_1m_ln				float=0,
		@Balancearrears_pound_6m				float=0,
		@Balancearrears_pound_6m_ln				float=0,
		@Count_daysarrear_30more_17m_ln			float=0,
		@Count_daysarrear_60more_17m_ln			float=0,
		@Daysarrears_pound_6m					float=0,
		@Employmentstatus_woe					CHAR(1),		---D
		@Flag_customerstatus_his_woe			CHAR(1),        ---
		@Gender_woe								CHAR(1),
		@Maritalstatus_woe						CHAR(1),
		@Max_perc_outs_3m_sq					float=0,
		@Max_perc_outsarrears_6m_ln				float=0,
		@Mobilenumber_woe						float=0,
		@Newest_credit_sq						float=0,
		@Number_account_17m						float=0,
		@Number_account_opened_3m				float=0,
		@Number_account_opened_3m_cr			float=0,
		@Numberdependents						float=0,
		@Numberdependents_cr					float=0,
		@Numberdependents_sq					float=0,
		@Occupation_woe							CHAR(2),
		@Oldest_credit_ln						float=0,
		@Postcode_woe							float=0,
		@Ratio_ndependent_to_age				float=0,
		@Ratio_tcurrentemploy_to_age			float=0,
		@Residentialstatus_woe					float=0,
		@Timecurrentaddress						INT,
		@Timecurrentaddress_ln					float=0,
		@Timecurrentemployment					INT,
		@Timecurrentemployment_ln				float=0,
		@Timecurrentemployment_sr				float=0,
		---------------------------------------------------------
		
		@AVG_AGREEMENT_TOTAL_1M					float=0,--FOR TEST
		@AVG_BALANCE_ARREARS_12M 				float=0,--FOR TEST		
		@AVG_BALANCE_ARREARS_1M 				float=0,--FOR TEST
		@COUNT_DAYSARREAR_30MORE_17M			float=0, --FOR TEST
		@COUNT_DAYSARREAR_60MORE_17M			float=0,
		@MAX_PERC_OUTS_3M						float=0,
		@MAX_PERC_OUTSARREARS_6M				float=0,
		@NEWEST_CREDIT							INT,
		@OLDEST_CREDIT							INT
---------------------------------------------------------------------------------
	  IF((SELECT COUNT(*) FROM @TABLEVAR) > 0)
				SET @Flag_customerstatus_his_woe = 'E'				
		ELSE 
				SET @Flag_customerstatus_his_woe = 'N'

IF (@Flag_customerstatus_his_woe = 'E') 
BEGIN
------------------------------------1--------------------------------------------
--@AVG_AGREEMENT_TOTAL_1M  AND  @AVG_AGREEMENT_TOTAL_1M_SQ
 SELECT @AVG_AGREEMENT_TOTAL_1M = SUM(ISNULL(AGREEMENT_TOTAL,0)) FROM @TABLEVAR   
	 WHERE OUTSTANDING_BALANCE > 0 AND EXTRACT_DATE>=@ONEMONTHBACKDATE;
		 IF (@AVG_AGREEMENT_TOTAL_1M = '0')
			SET @AVG_AGREEMENT_TOTAL_1M_SQ=0			
		 ELSE 
			 SET @AVG_AGREEMENT_TOTAL_1M_SQ=SQUARE(@AVG_AGREEMENT_TOTAL_1M)

	--SELECT @AVG_AGREEMENT_TOTAL_1M as 'Avg_agreement_total_1m'	
	-- SELECT @AVG_AGREEMENT_TOTAL_1M_SQ as '@@AVG_AGREEMENT_TOTAL_1M_SQ'
---------------------------------2------------------------------------------------------
--@AVG_BALANCE_ARREARS_1M AND @AVG_BALANCE_ARREARS_1M_LN 
 SELECT @AVG_BALANCE_ARREARS_1M=SUM(ISNULL(BALANCE_ARREARS,0)) FROM @TABLEVAR   
	 WHERE EXTRACT_DATE>=@ONEMONTHBACKDATE 
		 IF(@AVG_BALANCE_ARREARS_1M='0')
			SET @AVG_BALANCE_ARREARS_1M_LN=0
		 ELSE 
			SET @AVG_BALANCE_ARREARS_1M_LN=LOG(@AVG_BALANCE_ARREARS_1M)
	--SELECT @AVG_BALANCE_ARREARS_1M as 'avg_balance_arrears_1m'
	--SELECT @AVG_BALANCE_ARREARS_1M_LN as '@@@AVG_BALANCE_ARREARS_1M_LN'
--------------------------------3--------------------------------------------------------
---@AVG_BALANCE_ARREARS_12M  AND @AVG_BALANCE_ARREARS_12M_LN
 SELECT @AVG_BALANCE_ARREARS_12M = SUM(ISNULL(BALANCE_ARREARS,0))/12 FROM @TABLEVAR   
	WHERE EXTRACT_DATE>=@TWELVEMONTHSBACKDATE			
		IF(@AVG_BALANCE_ARREARS_12M=0)
			SET @AVG_BALANCE_ARREARS_12M_LN=0
		ELSE 
			SET @AVG_BALANCE_ARREARS_12M_LN=LOG(@AVG_BALANCE_ARREARS_12M);
	--SELECT @AVG_BALANCE_ARREARS_12M  as 'avg_balance_arrears_12m';
	--SELECT @AVG_BALANCE_ARREARS_12M_LN as '@@@@AVG_BALANCE_ARREARS_12M_LN'
-----------------------------------------------------------------------------------------
----BALANCEARREARS_POUND_6M
--SELECT * FROM @TABLEVAR;

WITH BALANCEARREARS_POUND_6M_Table  as
(SELECT --SUM(BALANCE_ARREARS) AS 'balance_arrears',EXTRACT_DATE AS MONTHS,
CASE 
WHEN EXTRACT_DATE>@ONEMONTHBACKDATE THEN SUM(BALANCE_ARREARS)*0.275
WHEN EXTRACT_DATE>=@TWOMONTHSBACKDATE AND EXTRACT_DATE<@ONEMONTHBACKDATE THEN SUM(BALANCE_ARREARS) *0.225
WHEN EXTRACT_DATE>=@THREEMONTSBACKDATE AND EXTRACT_DATE<@TWOMONTHSBACKDATE THEN SUM(BALANCE_ARREARS) *0.2 
WHEN EXTRACT_DATE>=@FOURMONTHSBACKDATE AND EXTRACT_DATE<@THREEMONTSBACKDATE THEN SUM(BALANCE_ARREARS) *0.15 
WHEN EXTRACT_DATE>=@FIVEMONTHSBACKDATE AND EXTRACT_DATE<@FOURMONTHSBACKDATE THEN SUM(BALANCE_ARREARS) *0.1
ELSE SUM(BALANCE_ARREARS) *0.05
END AS BALANCEARREARS_POUND_6M
 FROM @TABLEVAR 
GROUP BY  EXTRACT_DATE HAVING  EXTRACT_DATE >=@SIXMONTHSBACKDATE -- ORDER BY EXTRACT_DATE DESC
)
select @BALANCEARREARS_POUND_6M= SUM(BALANCEARREARS_POUND_6M) from BALANCEARREARS_POUND_6M_Table;
--select @BALANCEARREARS_POUND_6M as 'balancearrears_pound_6m'; 
------------------------------------------------------------------------------------------
----DAYSARREARS_POUND_6M
--SELECT * FROM @TABLEVAR;

WITH DAYSARREARS_POUND_6M_Table  as
(SELECT --SUM(DAYS_ARREARS) AS 'days_arrears',EXTRACT_DATE AS MONTHS,
CASE 
WHEN EXTRACT_DATE>@ONEMONTHBACKDATE THEN SUM(BALANCE_ARREARS)*0.275
WHEN EXTRACT_DATE>=@TWOMONTHSBACKDATE AND EXTRACT_DATE<@ONEMONTHBACKDATE THEN SUM(DAYS_ARREARS) *0.225
WHEN EXTRACT_DATE>=@THREEMONTSBACKDATE AND EXTRACT_DATE<@TWOMONTHSBACKDATE THEN SUM(DAYS_ARREARS) *0.2 
WHEN EXTRACT_DATE>=@FOURMONTHSBACKDATE AND EXTRACT_DATE<@THREEMONTSBACKDATE THEN SUM(DAYS_ARREARS) *0.15 
WHEN EXTRACT_DATE>=@FIVEMONTHSBACKDATE AND EXTRACT_DATE<@FOURMONTHSBACKDATE THEN SUM(DAYS_ARREARS) *0.1
ELSE SUM(DAYS_ARREARS) *0.05
END AS DAYSARREARS_POUND_6M
 FROM @TABLEVAR 
GROUP BY  EXTRACT_DATE HAVING  EXTRACT_DATE >=@SIXMONTHSBACKDATE -- ORDER BY EXTRACT_DATE DESC
)
select @DAYSARREARS_POUND_6M= SUM(DAYSARREARS_POUND_6M) from DAYSARREARS_POUND_6M_Table
--select @BALANCEARREARS_POUND_6M as 'balancearrears_pound_6m'; 
------------------------------------------------------------------------------------------
--COUNT_DAYSARREAR_30MORE_17M AND COUNT_DAYSARREAR_30MORE_17M_LN
SELECT @COUNT_DAYSARREAR_30MORE_17M=COUNT(DAYS_ARREARS)  FROM @TABLEVAR 
	WHERE  EXTRACT_DATE >=@DATESTART AND DAYS_ARREARS>=30
		IF(@COUNT_DAYSARREAR_30MORE_17M='0')
				SET @COUNT_DAYSARREAR_30MORE_17M_LN=0
			ELSE 
				SET @COUNT_DAYSARREAR_30MORE_17M_LN=LOG(@COUNT_DAYSARREAR_30MORE_17M)

--SELECT @COUNT_DAYSARREAR_30MORE_17M as 'count_daysarrear_30more_17m'
------------------------------------------------------------------------------------------
--@COUNT_DAYSARREAR_60MORE_17M
SELECT @COUNT_DAYSARREAR_60MORE_17M=COUNT(DAYS_ARREARS) FROM @TABLEVAR 
	WHERE  EXTRACT_DATE >=@DATESTART AND DAYS_ARREARS>=60
		IF(@COUNT_DAYSARREAR_60MORE_17M='0')
				SET @COUNT_DAYSARREAR_60MORE_17M_LN=0
			ELSE 
				SET @COUNT_DAYSARREAR_60MORE_17M_LN=LOG(@COUNT_DAYSARREAR_60MORE_17M)
--SELECT @COUNT_DAYSARREAR_60MORE_17M as 'count_daysarrear_60more_17m'
------------------------------------------------------------------------------------------
--MAX_PERC_OUTS_3M  AND MAX_PERC_OUTS_3M_SQ
	IF OBJECT_ID('tempdb..#TBLMAX_PERC') IS NOT NULL
		 BEGIN 
			DROP TABLE #TBLMAX_PERC
		 END
	SELECT SUM(ISNULL(OUTSTANDING_BALANCE,0)) AS OUTSTANDING_BALANCE, SUM(ISNULL(AGREEMENT_TOTAL,0)) AS AGREEMENT_TOTAL 
	INTO #TBLMAX_PERC FROM @TABLEVAR
	 GROUP BY EXTRACT_DATE
	HAVING EXTRACT_DATE >=@THREEMONTSBACKDATE 
	
	SET @MAX_PERC_OUTS_3M=
	(	SELECT TOP 1
		CASE  
		WHEN (AGREEMENT_TOTAL=0) THEN 0
		ELSE (OUTSTANDING_BALANCE/AGREEMENT_TOTAL)*100
		END AS MAX_PERC_OUTS_3M
		FROM #TBLMAX_PERC
		ORDER BY  MAX_PERC_OUTS_3M DESC
	)
	IF (@MAX_PERC_OUTS_3M=0)
		SET @MAX_PERC_OUTS_3M_SQ=0
	ELSE
		SET @MAX_PERC_OUTS_3M_SQ= SQUARE(@MAX_PERC_OUTS_3M) 	
------------------------------------------------------------------------------------------
--  MAX_PERC_OUTSARREARS_6M --MAX_PERC_OUTSARREARS_6M_LN
	IF OBJECT_ID('tempdb..#TBLMAX_PERC_OUT') IS NOT NULL
		 BEGIN 
			DROP TABLE #TBLMAX_PERC_OUT
		 END
	SELECT SUM(ISNULL(BALANCE_ARREARS,0)) AS BALANCE_ARREARS, SUM(ISNULL(OUTSTANDING_BALANCE,0)) AS OUTSTANDING_BALANCE 
	INTO #TBLMAX_PERC_OUT FROM @TABLEVAR
	 GROUP BY EXTRACT_DATE
	HAVING EXTRACT_DATE >=@SIXMONTHSBACKDATE 
	
	SET @MAX_PERC_OUTSARREARS_6M=
	(	SELECT TOP 1
		CASE  
		WHEN (OUTSTANDING_BALANCE=0) THEN 0
		ELSE (BALANCE_ARREARS/OUTSTANDING_BALANCE)*100
		END AS MAX_PERC_OUTSARREARS_6M
		FROM #TBLMAX_PERC_OUT
		ORDER BY  MAX_PERC_OUTSARREARS_6M DESC
	)
	IF (@MAX_PERC_OUTSARREARS_6M=0)
		SET @MAX_PERC_OUTSARREARS_6M_LN =0
	ELSE
		SET @MAX_PERC_OUTSARREARS_6M_LN= LOG(@MAX_PERC_OUTSARREARS_6M)
		
	--SELECT @MAX_PERC_OUTSARREARS_6M as 'max_perc_outsarrears_6m'
------------------------------------------------------------------------------------------
---NEWEST_CREDIT AND NEWEST_CREDIT_SQ   --DATE_ACCOUNT_OPENED
	SELECT TOP 1 @NEWEST_CREDIT= DATEDIFF(MONTH,DATE_ACCOUNT_OPENED,DATE_OBS) FROM @TABLEVAR ORDER BY DATE_ACCOUNT_OPENED DESC
	
	IF (@NEWEST_CREDIT=0)
		SET @NEWEST_CREDIT_SQ=0
	ELSE
		SET @NEWEST_CREDIT_SQ = SQUARE(@NEWEST_CREDIT)

	--SELECT @NEWEST_CREDIT as 'newest_credit'
------------------------------------------------------------------------------------------
----@NUMBER_ACCOUNT_17M	
	SELECT @NUMBER_ACCOUNT_17M=COUNT(DISTINCT ACCOUNT_NUMBER) FROM @TABLEVAR

--SELECT @NUMBER_ACCOUNT_17M as 'number_account_17m'
----------------------------------------------------------------------------------------------
----@NUMBER_ACCOUNT_OPENED_3M
	SELECT @NUMBER_ACCOUNT_OPENED_3M= COUNT (DISTINCT ACCOUNT_NUMBER) FROM @TABLEVAR WHERE DATE_ACCOUNT_OPENED >=@THREEMONTSBACKDATE

--SELECT @NUMBER_ACCOUNT_OPENED_3M as 'number_account_opened_3m'
------------------------------------------------------------------------------------------
	IF(@NUMBER_ACCOUNT_OPENED_3M=0)
		SET @NUMBER_ACCOUNT_OPENED_3M_CR=0
	ELSE
		SET @NUMBER_ACCOUNT_OPENED_3M_CR=POWER(@NUMBER_ACCOUNT_OPENED_3M,3)
-----------------------------------------------------------------------------------------------
---OLDEST_CREDIT_LN
	SELECT TOP 1 @OLDEST_CREDIT = DATEDIFF(MONTH,DATE_ACCOUNT_OPENED,DATE_OBS) FROM @TABLEVAR WHERE OUTSTANDING_BALANCE >0 
	ORDER BY DATE_ACCOUNT_OPENED

	IF(@OLDEST_CREDIT=0)
		SET @OLDEST_CREDIT_LN=0
	ELSE
		SET @OLDEST_CREDIT_LN=LOG(@OLDEST_CREDIT)

--SELECT @OLDEST_CREDIT as 'oldest_credit'
-----------------------------------------------------------------------------------------------
END 
SELECT 
ISNULL(@AVG_AGREEMENT_TOTAL_1M,0) 		as 'avg_agreement_total_1m',
ISNULL(@AVG_BALANCE_ARREARS_1M,0) 		as 'avg_balance_arrears_1m',
ISNULL(@AVG_BALANCE_ARREARS_12M,0)  	as 'avg_balance_arrears_12m',
ISNULL(@BALANCEARREARS_POUND_6M,0) 		as 'balancearrears_pound_6m',
ISNULL(@COUNT_DAYSARREAR_30MORE_17M,0) 	as 'count_daysarrear_30more_17m',
ISNULL(@COUNT_DAYSARREAR_60MORE_17M,0) 	as 'count_daysarrear_60more_17m',
ISNULL(@DAYSARREARS_POUND_6M,0) 		as 'daysarrears_pound_6m',
ISNULL(@Flag_customerstatus_his_woe,0)  as 'flag_customerstatus_his_woe',
ISNULL(@MAX_PERC_OUTS_3M,0) 			as 'max_perc_outs_3m',
ISNULL(@MAX_PERC_OUTSARREARS_6M,0) 		as 'max_perc_outsarrears_6m',
ISNULL(@NEWEST_CREDIT,0) 				as 'newest_credit',
ISNULL(@NUMBER_ACCOUNT_17M,0) 			as 'number_account_17m',
ISNULL(@NUMBER_ACCOUNT_OPENED_3M,0) 	as 'number_account_opened_3m',
ISNULL(@OLDEST_CREDIT,0) 				as 'oldest_credit'
-----------------------------------------------------------------------------------------------
--END -- PROCEDURE END
----------------------------------------------******************************************
END  TRY
BEGIN  CATCH
	DECLARE @err_msg VARCHAR(MAX)                                
	SELECT @err_msg =                               
			   'Procedure ' + CONVERT(VARCHAR(50),ERROR_PROCEDURE()) +                                
			   ', Error ' + CONVERT(VARCHAR(50), ERROR_NUMBER()) +                                
			   ', Severity ' + CONVERT(VARCHAR(5), ERROR_SEVERITY()) +                                
			   ', State ' + CONVERT(VARCHAR(5), ERROR_STATE()) +                                 
			   ', Line ' + CONVERT(VARCHAR(5), ERROR_LINE()) +                                 
			   ', ErrorMessage ' +  CONVERT(VARCHAR(8000), ERROR_MESSAGE()) 
	RAISERROR (@err_msg, 16, 1);      
END  CATCH; 
GO
