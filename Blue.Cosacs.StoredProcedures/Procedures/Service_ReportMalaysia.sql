--------------------------------------------------------------------------------
--
-- Project      : Securitisation Reports
-- File Name    : Service_ReportMalaysia.sql
-- File Type    : MSSQL Server SQL Script for reporting services
-- Author       : A Ayscough - 
-- Date         : 9 April 2008
-- Comments     : This writes data to the Rolling_service_rpt table. This will be used by Malaysia for the service report. 
-- Change Control
-- --------------
-- Date      By     Description
-- ----      --     -----------
-- 09/04/08  AA Created for Malaysia 

IF EXISTS (SELECT * FROM sysobjects WHERE NAME = 'Service_ReportMalaysia')
DROP PROCEDURE Service_ReportMalaysia
GO
CREATE PROCEDURE Service_ReportMalaysia
as

DECLARE @CalcDate DATETIME,@openingbalance MONEY,@newreceivables MONEY,
@additionalcharges MONEY,@collections MONEY,@normalcollections MONEY, @insurance MONEY,
@prepaymentcols MONEY, @subtotalcols MONEY 


IF EXISTS(SELECT * FROM information_schema.tables WHERE table_name = 'Rolling_Service_Rpt')
DROP TABLE Rolling_Service_Rpt

SELECT
    	s.datesecuritised,
    	s.balance as Sec_Balance,
    	a.outstbal as Balance_asat_1st,		-- Balance as at 1st of the current month but excluding BDW???
    	a.outstbal as Outstbalcorr, 		-- Excluding Charges
    	a.outstbal as AgeingBalance,			-- Net of Write Off
    	a.acctno,
    	a.dateacctopen,
    	a.agrmttotal,
    	a.termstype,
    	a.currstatus,
    	a.datelastpaid,
    	a.arrears,
	CONVERT(MONEY,0.00) AS Arrears_BDW,	-- RD/AA 13/10/04 Added to correct Mths_Arrs for BDW accts
    	ag.datedel,
    	ag.deposit,
    	ag.servicechg,
	ag.Datefullydelivered,
    	i.instalno, 
    	i.instalamount,
    	i.datefirst,
    	i.instalno as MthsLeft,
    	0 as Mths_arrs,
    	0 as New_Mths_arrs,
	0 AS BDW_Mths_Arrs,	-- RD/AA 13/10/04 Added to correct Mths_Arrs for BDW accts
    	CONVERT(MONEY,0.00) as Dilution, 
    	CONVERT(MONEY,0.00) as BalancePaid, 
    	CONVERT(MONEY, 0.00) as Charges,
    	CONVERT(MONEY, 0.00) as Charges_Collected,
    	CONVERT(MONEY, 0.00) as Charges_Added,
   	CONVERT(MONEY, 0.00) as RecoveryPay,	-- RD 13/08/04 Replaced with Service_Rpt_Charges
    	CONVERT(MONEY,0.00) as AddTo_Total,
    	CONVERT(MONEY,0.00) as WriteOff_Total,	-- RD 19/07/04 to get value for BDW 
  	0 as Sec_RunNo
INTO  	Rolling_Service_Rpt
FROM 	Acct a, Agreement ag, Instalplan i, Sec_Account s, Service_Data sd
WHERE	a.acctno = ag.acctno
AND	a.acctno = i.acctno
AND	a.acctno = s.acctno
AND	a.securitised = 'Y'
AND	s.datesecuritised <= sd.RunDate



IF NOT EXISTS (SELECT Name FROM SysIndexes
               WHERE  Name = 'ix_Service_Report_CalcDate')
BEGIN
    CREATE NONCLUSTERED INDEX ix_Service_Report_CalcDate ON Service_Report (CalcDate)
END
/*INSERT INTO Service_Data(  CalcDate, CollPeriod_2, RunDate, StartRunNo, EndRunNo, FirstMonthRunNo,
 LastMonthRunno, ProgramLimit, ProgramAmount, DiscountRate1, DiscountRate2)*/
--SELECT * FROM Service_Data
--SET    @CalcDate = '01-May-2004'

/*INSERT INTO interfacecontrol  
(interface, runno, datestart, datefinish, result) 
SELECT interface, runno, datestart, datefinish, result FROM bar311207.dbo.interfacecontrol 
WHERE interface = 'updsmry' AND datefrom BETWEEN '1-aug-2007' AND '30-sep-2007'
*/



IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
	   WHERE  table_name = 'Fin_Sec_Runno')
DROP TABLE Fin_Sec_Runno


-- Get max runno based on the date of securitisation for each account and 
SELECT 	a.acctno, R.datesecuritised, r.Sec_Runno, max(i.runno) as Fin_Runno
INTO 	Fin_Sec_Runno
FROM 	interfacecontrol i, Sec_Account a, Rolling_Service_Rpt r
WHERE 	a.acctno = r.acctno 
AND	CONVERT(VARCHAR(10),i.datestart,103) = CONVERT(VARCHAR(10), a.datesecuritised,103)
AND	interface = 'UPDSMRY'
GROUP BY
	r.datesecuritised, r.sec_runno, a.acctno


/* RD 31/03/05 Replaced with code above as some of the accounts had datesecuritised later
-- then the datefullydelivered but the query below was getting the date of the last
-- delivery transaction which was not the same as datesecuritised

-- Get max runno based on the date of securitisation for each account and 
SELECT 	f.acctno, R.datesecuritised, r.Sec_Runno, max(f.runno) as Fin_Runno
INTO 	Fin_Sec_Runno
FROM 	Fintrans f, Sec_Account a, Rolling_Service_Rpt r
WHERE 	f.acctno = a.acctno 
AND	a.acctno = r.acctno
AND 	f.datetrans <= a.datesecuritised
AND	f.transtypecode In ('DEL', 'ADJ', 'DDH', 'RPO', 'REP', 'GRT', 'RDL', 'MKT')
GROUP BY
	r.datesecuritised, r.sec_runno, f.acctno
*/


-- Update Sec_RunNo in Rolling_Service_Rpt
UPDATE	Rolling_Service_Rpt
SET	Sec_Runno = Fin_Runno
FROM	Fin_Sec_Runno f
WHERE	f.acctno = Rolling_Service_Rpt.acctno


UPDATE	Rolling_Service_Rpt
SET	Sec_Balance = ISNULL((SELECT SUM(transvalue)
		              FROM    fintrans f, Service_Data sd
		              WHERE   f.acctno = Rolling_Service_Rpt.acctno 
			      AND	(f.Runno <= Rolling_Service_Rpt.Sec_RunNo
			      AND	f.RunNo != 0 )
		              AND     f.transtypecode NOT IN ('ADM', 'INT')),0)


SELECT @CalcDate = CalcDate FROM Service_Data
DELETE FROM Service_Report WHERE CalcDate = @CalcDate

--SELECT * FROM Service_Data
SELECT	@openingbalance = ISNULL(SUM(f.transvalue),0)
FROM 	fintrans f, Rolling_Service_Rpt s, Service_Data sd
WHERE	f.acctno 	   = s.acctno
AND	s.datesecuritised <= DATEADD(Month,-1,sd.RunDate) 
AND	(f.RunNo 	  <= sd.StartRunNo
AND	f.runno		   != 0 )		-- RD 08/09/04 Added to exclude transactions with zero runno
AND     f.transtypecode NOT IN ('ADM', 'INT') --, 'DDF', 'FEE') --, 'BDW')  	

EXEC Service_ReportWriteLine 'OpeningBalance',@openingbalance
-- Value for New Receivables excluding value for Add-To 

SELECT @newreceivables = isnull(sum(Sec_balance),0) 
FROM   Rolling_Service_Rpt r,service_data sd
WHERE r.datesecuritised BETWEEN sd.CollPeriod_1 AND sd.CollPeriod_2

EXEC Service_ReportWriteLine 'Newreceivables',@newreceivables

-- additional charges on collections
SELECT @additionalcharges = ISNULL((SELECT SUM(transvalue)
		FROM	Fintrans f, Service_Data sd,Rolling_Service_Rpt a
		WHERE	a.acctno = f.acctno 
		AND 	f.transtypecode = 'FEE'
      AND	f.RunNo 	> a.Sec_RunNo
		AND	f.RunNo	> sd.StartRunNo
		AND	f.RunNo <= sd.EndRunNo),0)

EXEC Service_ReportWriteLine 'additionalcharges',@additionalcharges


SELECT @insurance= ISNULL(SUM(transvalue),0)
FROM	Fintrans f, Rolling_Service_Rpt a, Service_Data sd
WHERE	f.acctno = a.acctno
AND	f.RunNo 	> a.Sec_RunNo
AND	f.transtypecode = 'INS'
AND	f.RunNo	> sd.StartRunNo
AND	f.RunNo	<= sd.EndRunNo

EXEC Service_ReportWriteLine 'insurance',@insurance
--DROP TABLE #Prepayment_Rebates
SELECT Sec_RunNo FROM Rolling_Service_Rpt WHERE acctno = '500000830184'
SELECT 	a.Acctno, MAX(f.DateTrans) as DateTrans
INTO   	#Prepayment_Rebates
FROM   	Rolling_Service_Rpt a, Fintrans f, Service_Data sd
WHERE  	f.acctno = a.acctno 
AND    	f.transtypecode = 'PAY'
AND    	f.transvalue < 0
AND	f.RunNo 	> a.Sec_RunNo
AND	f.RunNo	> sd.StartRunNo
AND	f.RunNo <= sd.EndRunNo
AND    	f.datetrans <= (SELECT max(datestatchge)
                       FROM   status s, Service_Data sd
                       WHERE  s.statuscode = 'S'
                       AND    s.acctno = a.acctno
                       AND    s.datestatchge  > DATEADD(Month,-1,sd.RunDate)
                       AND    s.datestatchge <= sd.RunDate)
AND    f.datetrans <= (SELECT max(f1.datetrans)
                       FROM   fintrans f1, Service_Data sd
                       WHERE  f1.acctno = f.acctno
                       AND    f1.transtypecode = 'REB'
		       AND    f1.RunNo > sd.StartRunNo
		       AND    f1.RunNo <= sd.EndRunNo)
GROUP BY a.acctno



--SELECT  sd.StartRunNo,sd.EndRunNo,sd.RunDate FROM  Service_Data sd
SELECT	@prepaymentcols = ISNULL(sum(f.transvalue),0)
FROM   	#Prepayment_Rebates p, Fintrans f  
WHERE  	f.acctno        = p.acctno 
AND    	f.datetrans     = p.datetrans
AND    	f.transtypecode = 'PAY'
AND    	f.transvalue    < 0

EXEC Service_ReportWriteLine 'prepaymentcols',@prepaymentcols



SELECT @normalcollections =  isnull(sum(f.transvalue),0)
FROM   	Rolling_Service_Rpt a, fintrans f, Service_Data sd
WHERE  	
  	f.acctno        = a.acctno 
AND	f.RunNo 	> a.Sec_RunNo
AND	f.RunNo 	> sd.StartRunNo
AND	f.RunNo 	<= sd.EndRunNo
AND    	f.transtypecode IN ('PAY', 'DDN', 'DDE', 'DDR',  'COR', 'REF', 'RET', 'XFR', 'SCX', 'PEX', 'BNK','ADX')

SET @normalcollections = @normalcollections-@prepaymentcols

EXEC Service_ReportWriteLine 'normalcollections',@normalcollections


SELECT @additionalcharges= 	ISNULL((SELECT SUM(transvalue)
		FROM	Fintrans f, Service_Data sd,Rolling_Service_Rpt a
		WHERE	a.acctno = f.acctno 
      AND	f.RunNo 	> a.Sec_RunNo
		AND 	f.transtypecode = 'FEE'
		AND	f.RunNo	> sd.StartRunNo
		AND	f.RunNo <= sd.EndRunNo),0)

/* Leaving out interest charges for the time being
SELECT @additionalcharges= @additionalcharges + 
	ISNULL((SELECT SUM(f.transvalue)
	FROM   	fintrans f, Service_Data sd,Rolling_Service_Rpt
	WHERE  	Rolling_Service_Rpt.acctno = f.acctno 
	AND    	f.transtypecode IN ('INT', 'ADM')
	AND	f.RunNo		> sd.StartRunNo
	AND	f.RunNo 	<= sd.EndRunNo
	AND	Rolling_Service_Rpt.currstatus = 'S'
	AND	EXISTS (SELECT * FROM Status s
			WHERE  statuscode = 'S'
			AND    s.acctno = Rolling_Service_Rpt.acctno
			AND    s.datestatchge  > DATEADD(Month,-1,sd.RunDate)
	       	        AND    s.datestatchge <= sd.RunDate)),0) */
EXEC Service_ReportWriteLine 'additionalcharges',@additionalcharges


SELECT @subtotalcols = @normalcollections + @insurance + @prepaymentcols + @additionalcharges
EXEC Service_ReportWriteLine 'subtotalcols',@subtotalcols

--SELECT @retur
--SELECT @
DECLARE @returns MONEY,@allowances MONEY,@addtosettlements MONEY, @otherdilutions MONEY,
@writeoffs MONEY,@rebates MONEY,@dilutionssubtotal MONEY,@weightedaverage FLOAT
--SELECT Sec_RunNo FROM Rolling_Service_Rpt WHERE Sec_RunNo>0
SELECT @returns = ISNULL(SUM(f.transvalue),0)
FROM	Fintrans f,  Rolling_Service_Rpt a, Service_Data sd
WHERE	f.acctno 	  = a.acctno
AND	f.RunNo		> a.Sec_RunNo
AND	f.transtypecode in ('DEL', 'GRT','REP','RDL')
AND	f.RunNo		  > sd.StartRunNo
AND	f.RunNo 	  <= sd.EndRunNo
--AND	f.transvalue 	  < 0

EXEC Service_ReportWriteLine 'returns',@returns

SELECT	@allowances = ISNULL(SUM(f.transvalue),0)
FROM	Fintrans f,  Rolling_Service_Rpt a, Service_Data sd
WHERE	f.acctno 	  = a.acctno
AND	f.RunNo		> a.Sec_RunNo
AND	f.transtypecode in ('ADJ', 'DDH', 'MKT')
--AND	f.transtypecode in ('DEL', 'ADJ', 'DDH', 'RPO', 'REP', 'GRT', 'RDL', 'MKT')
AND	f.RunNo		  > sd.StartRunNo
AND	f.RunNo 	  <= sd.EndRunNo
AND	f.transvalue 	  <> 0

EXEC Service_ReportWriteLine 'allowances',@allowances

-- todo SELECT @otherdilutions = 

SELECT @addtosettlements = ISNULL(SUM(f.transvalue),0)
FROM	Fintrans f,  Rolling_Service_Rpt a, Service_Data sd
WHERE	f.acctno 	  = a.acctno
AND	f.RunNo		> a.Sec_RunNo
AND	f.transtypecode in ('ADD')
--AND	f.transtypecode in ('DEL', 'ADJ', 'DDH', 'RPO', 'REP', 'GRT', 'RDL', 'MKT')
AND	f.RunNo		  > sd.StartRunNo
AND	f.RunNo 	  <= sd.EndRunNo

EXEC Service_ReportWriteLine 'addtosettlements',@addtosettlements

SELECT @dilutionssubtotal = @returns + @allowances + @addtosettlements + ISNULL(@otherdilutions,0)

EXEC Service_ReportWriteLine 'dilutionssubtotal',@dilutionssubtotal

DECLARE 
@closing_balance MONEY,@totalwriteoffs MONEY,@losses MONEY,
@1montharrs MONEY, @2montharrs MONEY,@3montharrs MONEY,@4montharrs MONEY,
@5montharrs MONEY,@6montharrs MONEY,@7montharrs MONEY,@8montharrs MONEY,
@9montharrs MONEY,@10montharrs MONEY,@11montharrs MONEY,@12montharrs MONEY,
@gr12monthsarrs MONEY

SELECT @totalwriteoffs =  ISNULL(SUM(f.transvalue),0)
FROM	Fintrans f,  Rolling_Service_Rpt a, Service_Data sd
WHERE	f.acctno 	  = a.acctno
AND	f.RunNo		> a.Sec_RunNo
AND	f.transtypecode in ('BDW')
--AND	f.transtypecode in ('DEL', 'ADJ', 'DDH', 'RPO', 'REP', 'GRT', 'RDL', 'MKT')
AND	f.RunNo		  > sd.StartRunNo
AND	f.RunNo 	  <= sd.EndRunNo

EXEC Service_ReportWriteLine 'totalwriteoffs',@totalwriteoffs

SELECT @rebates =  ISNULL(SUM(f.transvalue),0)
FROM	Fintrans f,  Rolling_Service_Rpt a, Service_Data sd
WHERE	f.acctno 	  = a.acctno
AND	f.RunNo		> a.Sec_RunNo
AND	f.transtypecode in ('REB')
--AND	f.transtypecode in ('DEL', 'ADJ', 'DDH', 'RPO', 'REP', 'GRT', 'RDL', 'MKT')
AND	f.RunNo		  > sd.StartRunNo
AND	f.RunNo 	  <= sd.EndRunNo

EXEC Service_ReportWriteLine 'rebates',@rebates

SELECT	@Closing_Balance = isnull(sum(transvalue),0) 
FROM   	Rolling_Service_Rpt a, fintrans f, Service_Data sd
WHERE  	f.acctno 	= a.acctno 
AND    	(f.RunNo 	<= sd.EndRunNo
AND	f.RunNo	 	!= 0 )				-- RD 09/09/04 Added to exclude value for zero no
AND    	f.transtypecode NOT IN ('INT', 'ADM')

EXEC Service_ReportWriteLine 'Closing_Balance',@Closing_Balance

CREATE TABLE #secaccts (acctno CHAR(12) NOT NULL PRIMARY KEY,outstbalcorr MONEY,arrearsexcharges MONEY,MonthsinArrears int)

INSERT INTO #secaccts 
SELECT s.acctno ,MAX(A.outstbal),MAX(A.arrears),0
FROM sec_account s
JOIN ACCT A 
ON s.acctno = A.acctno
GROUP BY s.acctno

UPDATE #SECACCTS SET OUTSTBALCORR = 
ISNULL((SELECT SUM(transvalue) 
FROM fintrans F ,Service_Data sd
WHERE F.ACCTNO= #SECACCTS.ACCTNO
--AND	f.RunNo	> sd.StartRunNo
AND	f.RunNo <= sd.EndRunNo
AND f.transtypecode NOT IN ('int','adm')),0)



UPDATE #secaccts SET arrearsexcharges = arrearsexcharges -
ISNULL((SELECT SUM(transvalue) 
FROM fintrans F ,Service_Data sd
WHERE F.ACCTNO= #SECACCTS.ACCTNO
--AND	f.RunNo	> sd.StartRunNo
AND	f.RunNo <= sd.EndRunNo
AND f.transtypecode NOT IN ('int','adm')),0)

UPDATE #secaccts 
SET monthsinarrears =FLOOR(arrearsexcharges/i.instalamount )
FROM instalplan i 
WHERE i.acctno= #secaccts.acctno AND i.instalamount>0

UPDATE #secaccts SET monthsinarrears = 0 WHERE monthsinarrears <0
-- Inserted to get the value of Weighted Average as per calculation
-- received from Georg  RD 06/05/2004
IF EXISTS(SELECT * FROM sysobjects WHERE NAME = 'totalbookos')
   DROP TABLE totalbookos
SELECT	ISNULL(SUM(Outstbalcorr),0) AS TotalBookOS, CONVERT(MONEY,0.00) as Maturity_Profile
INTO	TotalBookOS
FROM	Rolling_Service_Rpt

--SELECT Outstbalcorr FROM Rolling_Service_Rpt WHERE Outstbalcorr >0

DECLARE @AvgMthsLeft MONEY

SELECT	@AvgMthsLeft = AVG(MthsLeft) FROM Rolling_Service_Rpt

PRINT '** Getting Average Life **'
PRINT GETDATE()
PRINT ''
UPDATE	TotalBookOS
SET	Maturity_Profile = @AvgMthsLeft


PRINT '** Total Values for Service Report **'
PRINT''

SELECT 	SUM(Balance_asat_1st) as Total_Receivable FROM Rolling_Service_Rpt

--SELECT 	SUM(Sec_Balance) as New_Receivable FROM Rolling_Service_Rpt
--WHERE	Seasoned = 'N'

PRINT '** Point 18 : Weighted Average Portfolio Life in Months **'
PRINT ''

-- Need to multiply results of @WAvgLife_Cal_1 times each month for Maturity Profile.
-- For example if Maturity_Profile is 15 then we need:
-- TotalBookOS * 1 , TotalBookOS * 2, TotalBooks * 3 up to TotalBookOS * 15
-- Sum all the results and divide by TotalBookOS to get the weighted average.
--
-- EXAMPLE :
--
-- (TotalBookOS / Maturity_Profile)
-- (141499722.5300 / 15) = 9433314.8353333
-- 9433314.8353333 * 1 = 9433314.8353333
-- 9433314.8353333 * 2 = 18866629.6706666
-- (9433314.8353333 + 18866629.6706666) = 28299944.5059999
-- (28299944.5059999 / 141499722.5300) = .199999999999999293284
-- Weighted Average = .19

DECLARE 
        @TotalBookOS        FLOAT,
        @Maturity_Profile   FLOAT,
        @CalcBook           FLOAT,
        @Result             FLOAT,
	@WAverage	    FLOAT

SELECT 	@CalcDate = CalcDate FROM Service_Data

SELECT 	@TotalBookOS      = TotalBookOS,
       	@Maturity_Profile = Maturity_Profile
FROM   	TotalBookOS
--SELECT * FROM totalbookos
IF (@Maturity_Profile = 0) SET @Maturity_Profile = 1

SET 	@CalcBook = @TotalBookOS / @Maturity_Profile
SET 	@Result = 0

WHILE 	@Maturity_Profile > 0
BEGIN
  SET 	@Result = @Result + (@CalcBook * @Maturity_Profile)
  SET 	@Maturity_Profile = @Maturity_Profile - 1
END

SET 	@WAverage = (@Result / @TotalBookOS)

PRINT CONVERT(VARCHAR,@WAverage) 

UPDATE 	Service_Report SET ResultValue = ISNULL(@WAverage,0)
WHERE  	CalcDate = @CalcDate AND ResultId = 'R16'


EXEC Service_ReportWriteLine 'WeightedAverage',@WAverage

DECLARE @month1 MONEY,@month2 MONEY,
@month3 MONEY,@month4 MONEY,
@month5 MONEY,@month6 MONEY,
@month7 MONEY,@month8 MONEY,
@month9 MONEY,@month10 MONEY,
@month11 MONEY,@month12 MONEY,
@monthgr12 MONEY,@month0 MONEY
SELECT TOP 100 * FROM #secaccts
SELECT @month0 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 0
EXEC Service_ReportWriteLine 'Monthsarrears0',@month0

SELECT @month1 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 1
EXEC Service_ReportWriteLine 'Monthsarrears1',@month1

SELECT @month2 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 2
EXEC Service_ReportWriteLine 'Monthsarrears2',@month2

SELECT @month3 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 3
EXEC Service_ReportWriteLine 'Monthsarrears3',@month3

SELECT @month4 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 4
EXEC Service_ReportWriteLine 'Monthsarrears4',@month4

SELECT @month5 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 5
EXEC Service_ReportWriteLine 'Monthsarrears5',@month5

SELECT @month6 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 6
EXEC Service_ReportWriteLine 'Monthsarrears6',@month6

SELECT @month7 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 7
EXEC Service_ReportWriteLine 'Monthsarrears7',@month7

SELECT @month8 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 8
EXEC Service_ReportWriteLine 'Monthsarrears8',@month8

SELECT @month9 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 9
EXEC Service_ReportWriteLine 'Monthsarrears9',@month9

SELECT @month10 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 10
EXEC Service_ReportWriteLine 'Monthsarrears10',@month10

SELECT @month11 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 11
EXEC Service_ReportWriteLine 'Monthsarrears11',@month11

SELECT @month12 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears = 12
EXEC Service_ReportWriteLine 'Monthsarrears12',@month12

SELECT @monthgr12 = SUM(outstbalcorr) FROM #secaccts  WHERE monthsinarrears > 12
EXEC Service_ReportWriteLine 'MonthsarrearsGR12',@monthgr12

DECLARE @writeoffnetofrecoveries MONEY
SELECT @writeoffnetofrecoveries= SUM(f.transvalue) FROM 
 fintrans f, Rolling_Service_Rpt s, Service_Data sd
WHERE f.acctno 	   = s.acctno
AND	s.datesecuritised <= DATEADD(Month,-1,sd.RunDate) 
AND	(f.RunNo 	  <= sd.StartRunNo
AND	f.runno		   != 0 )		-- RD 08/09/04 Added to exclude transactions with zero runno
AND     f.transtypecode  IN ('BDW', 'DPY') --, 'DDF', 'FEE') --, 'BDW')  	
-- to do work out what happens to accounts written off whilst still doing Coaster to Cosacs

GO
