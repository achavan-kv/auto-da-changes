
--/****************************************************/
--Start of Store Procedure for report 16 - amortisation
--/****************************************************/
IF EXISTS (SELECT * FROM dbo.sysobjects 
           WHERE ID = object_id('[dbo].[Report16Amortisation]') 
           AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
   DROP PROCEDURE Report16Amortisation
GO

CREATE PROCEDURE Report16Amortisation

--------------------------------------------------------------------------------
--
-- Project      : CoSACS dotNET
-- File Name    : Report16Amortisation.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Amortisation Triggers Report - Singapore CR619
-- Author       : D Richardson / Rupal Desai
-- Date         : 25 June 2004
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
-- 21/06/04  DSR BalancePaid now excludes DDF debit transactions > 0
--               Note: DDF can be negative (credit) and positive (debit).
-- 21/06/04  DSR Report_Dates.RunDate is used (instead of MonthYear) to
--               be consistent with the Servicer Report when grouping
--               financial transactions.
-- 21/06/04  DSR Ratio and Rolling Average queries written to update all
--               new rows added to Summary12 table.
-- 22/06/04  DSR Updates now use ISNULL(SELECT) instead of SELECT ISNULL()
-- 22/06/04  DSR Trigger thresholds now held on Summary12_Threshold table
-- 23/06/04  DSR Only look back 11 months for 12 month ratio
-- 24/06/04  DSR Added arrears re-calculation per month per account
-- 24/06/04  DSR Added Collection and Rebate queries
-- 25/06/04  DSR Converted to SProc and added Quantitative Triggers
-- 25/06/04  DSR Added summary counts
-- 28/06/04  RD  Removed create tables from procedure and added them in upgrade insteated
-- 29/06/04  RD  Modified to only insert data for last row as per max date securitised
-- 01/07/04  RD  Modified Calculation for Rebate
-- 19/08/04  RRD Removed Balancepaid as it's not being used
-- 19/08/04  RRD Modified Report_Dates and Tirgger_Data to add EndRunno
-- 19/08/04  RRD Modified Trigger_Data table to add Instalno and datefullydelivered
-- 19/08/04  RRD Modified calculation used for Months in arrears to age correctly as in the service report
-- 19/08/04  RRD Modified all the query to get data from fintrans using the runno rather then the datetrans
-- 19/08/04  RRD Added SecRunNo in order to get date after the date of securitisation based on the run no
-- 20/08/04  RRD Modified to added new column to store balance for ageing
-- 20/08/04  RRD Modified balances to added cumulative charges collected during the month
-- 31/08/04  DSR Rebate queries amended to agree with Service Report (individually commented)
-- 02/09/04  DSR Collection queries amended to agree with Service Report (individually commented)
--------------------------------------------------------------------------------

    -- Parameters
    -- none

AS -- DECLARE
    -- Local variables

BEGIN

PRINT ''
PRINT ' Amortisation Triggers Report '
PRINT ''


PRINT ' .. Delete old data'
-- Remove any results over 15 months old
DELETE FROM Summary12 WHERE DATEDIFF(Month,MonthYear,GETDATE()) > 15

-- Remove any results for a partial month not run on the 1st.
-- (Should only ever be one row like this if report was
--  last run part way through a month)
DELETE FROM Summary12 WHERE DAY(RptDate) != 1

--
-------------------- Start Date Init
--
DELETE FROM Report_Dates

PRINT ' .. Calculate dates'
DECLARE @LastRptDate   DATETIME,
        @RptDate       DATETIME,
        @EndRunNo      SMALLINT,
        @StartRunNo    SMALLINT
    
-- Need to populate each month since the report was last run.
-- If this is the first run of this report then assume last run was on 1st Apr 2004.
SELECT @LastRptDate = ISNULL(Max(RptDate),CONVERT(DATETIME,'01 Apr 2004',106)) FROM Summary12

IF DAY(@LastRptDate) = 1
    -- The last run was for a whole month so the first report date is the 1st of the next month
    SET @RptDate = DATEADD(Month,1,@LastRptDate)
ELSE
    -- The last run was for a part month so the first report date is the 1st of the same month
    SET @RptDate = DATEADD(Day,-DAY(@LastRptDate)+1,@LastRptDate)    


-- Populate each month report date up to the current date
WHILE @RptDate <= GETDATE()
BEGIN
    INSERT INTO Report_Dates (MonthYear, RptDate, RunDate)--, StartRunNo, EndRunNo)
    VALUES (@RptDate, @RptDate, DATEADD(DAY,1,DATEADD(HOUR,8,@RptDate)))
    SET @RptDate = DATEADD(Month,1,@RptDate)
END

-- If the current date is not the 1st of a month then add a row to include the part month
IF DAY(GETDATE()) != 1
    INSERT INTO Report_Dates (MonthYear, RptDate, RunDate)
    VALUES (@RptDate, GETDATE(),GETDATE())        -- '16-aug-2004','16-aug-2004')


-- RD 19/08/04
-- Populdate each month start runno up to the current month
UPDATE  Report_Dates
SET     StartRunNo = (SELECT max(i.runno)
                      FROM   interfacecontrol i
                      WHERE  i.interface = 'UPDSMRY'
                      AND    i.datefinish <= DATEADD(MONTH,-1,Report_Dates.Rundate))


-- Populdate each month last runno up to the current month
UPDATE Report_Dates
SET    EndRunNo = (SELECT max(i.runno)
                   FROM   interfacecontrol i
                   WHERE  i.interface = 'UPDSMRY'
                   AND    i.datefinish <= Report_Dates.Rundate)


-- Removing row where the MonthYear is greater then the max date securitised
-- Doning this way as the above code to only insert if the row as per max date securitised
DECLARE @MaxSecDate Datetime

SELECT @MaxSecDate = Max(datesecuritised) FROM sec_account

DELETE FROM Report_Dates WHERE monthyear > @MaxSecDate

-- Add the new report dates
INSERT INTO Summary12 (MonthYear, RptDate)
SELECT MonthYear, RptDate FROM Report_Dates

select * from Summary12
--
-------------------- End Date Init
--


IF EXISTS (SELECT table_name FROM INFORMATION_SCHEMA.TABLES
           WHERE table_name = 'Trigger_Data')
BEGIN
    DELETE FROM Trigger_Data
END

--
-- Report_Dates.RunDate is used to be consistent with the Servicer Report
-- when grouping financial transactions. This includes all transactions up
-- to 8am of the second of the month to allow for the late running of EOD.
-- 
PRINT ' .. Insert Trigger_Data'
INSERT INTO Trigger_Data
    (MonthYear,
     RunDate,
     StartRunNo,
     EndRunNo,
     Acctno,
     InstalNo,
     InstalAmount,
     DateSecuritised,
     DateFullyDelivered,
     Datedel)
SELECT r.MonthYear,
       r.RunDate,
       r.StartRunNo,
       r.EndRunNo,
       a.acctno,  
       i.instalno,
       i.instalamount,
       s.DateSecuritised,
       ag.datefullydelivered,
       ag.datedel
FROM   Report_Dates r, Sec_Account s, acct a, agreement ag, instalplan i
WHERE  s.DateSecuritised <= r.RunDate
AND    a.acctno           = s.acctno
AND    a.securitised      = 'Y'
AND    ag.acctno          = s.acctno
AND    i.acctno           = s.acctno

-- AA 23/07/04 This appears to work.
-- first update it from sec_account        
UPDATE Trigger_Data 
SET    SecRunno= a.runno
FROM   sec_account a WHERE a.acctno = Trigger_Data.acctno

-- then update from interfacecontrol
UPDATE Trigger_Data
SET    SecRunno= (select max(f.runno)
                  from   interfacecontrol f -- financial interface
                  where  f.interface = 'UPDSMRY' 
                  and    f.datestart =  (select max(g.datestart) 
                                         from   interfacecontrol g /*financial interface */, interfacecontrol S /*securitised interface */
                                         where  G.interface = F.interface
                                         and    G.runno = f.runno
                                         and    G.datestart < S.datestart 
                                         and    S.interface ='SECURITISE'
                                         and    S.runno = Trigger_Data.SecRunno))

Create clustered index ix_Trigger_Data_acctno on
Trigger_Data(acctno,secrunno) -- already have run this on June report


-- Updating add-to flag
UPDATE Trigger_Data
SET    AddTo = 'Y'
FROM   Fintrans f, Report_Dates rd
WHERE  f.acctno        = Trigger_Data.acctno
AND    f.transtypecode = 'ADD'
AND    f.RunNo        <= rd.EndRunNo


PRINT 'Updating Balance'
PRINT Getdate()
-- Update Balance as at 1st of the month
UPDATE Trigger_Data
SET    Balance = ISNULL((SELECT SUM(f.transvalue)
                         FROM   fintrans f
                         WHERE  f.acctno = Trigger_Data.acctno 
                         AND    f.RunNo <= Trigger_Data.EndRunNo),0)

PRINT 'Updating Balance Excluding BDW'
PRINT GETDATE()
UPDATE Trigger_Data
SET    BalanceExBDW = ISNULL((SELECT sum(transvalue)
                              FROM   fintrans f
                              WHERE  f.acctno = Trigger_Data.acctno 
                              AND    f.RunNo <= Trigger_Data.EndRunNo
                              AND    f.transtypecode != 'BDW'),0) 



PRINT 'Updating OutstBalCorr'
PRINT Getdate()
-- Update Outstbalcorr to get balance with out any charges and no BDW
-- (Balance Gross of Charges and BDW)
UPDATE Trigger_Data
SET    OutstbalCorr =
         ISNULL((SELECT SUM(f.transvalue)
                 FROM   fintrans f
                 WHERE  f.acctno = Trigger_Data.acctno 
                 AND    f.RunNo <= Trigger_Data.EndRunNo
                 AND    f.transtypecode NOT IN ('ADM', 'INT','FEE', 'DDF','BDW')),0) 

PRINT 'Updating Ageing Balance'
-- Update AgeingBalance to be same as Ageing balance in the service report
UPDATE Trigger_Data
SET    AgeingBalance = ISNULL((SELECT SUM(f.transvalue)
                               FROM   fintrans f
                               WHERE  f.acctno = Trigger_Data.acctno 
                               AND    f.RunNo <= Trigger_Data.EndRunNo
                               AND    f.transtypecode NOT IN ('ADM', 'INT','FEE', 'DDF')),0) 

PRINT 'Updating BalanceExBDW'
PRINT Getdate()
-- RD 20/08/04 Added to get the same balance as Service Report
UPDATE Trigger_Data
SET    RecoveryPay = (SELECT ISNULL(SUM(f.transvalue),0)
                      FROM   fintrans f, custacct ca
                      WHERE  f.acctno        = ca.acctno
                      AND    f.chequeno      = Trigger_Data.acctno
                      AND    ca.custid       = 'BDWSECRECOVER' 
                      AND    f.RunNo        <= Trigger_Data.EndRunNo
                      AND    f.transtypecode = 'DPY')

PRINT 'Updating Charges'
PRINT Getdate()

-- RD 19/08/04 Modified to have the same calcualtion as service report
UPDATE Trigger_Data
SET    Charges = (SELECT ISNULL(SUM(transvalue),0)
                  FROM   Fintrans f
                  WHERE  f.acctno = Trigger_Data.acctno
                  AND    f.transtypecode = 'FEE'
                  AND    f.RunNo > Trigger_Data.StartRunNo
                  AND    f.RunNo <= Trigger_Data.EndRunNo)


-- Getting value for Interest and Admin charges for settled securitised accounts.
UPDATE Trigger_Data
SET    Charges = ( ISNULL(Charges,0) +
        ISNULL((SELECT sum(f.transvalue)
                FROM   fintrans f
                WHERE  f.acctno = Trigger_Data.acctno 
                AND    f.transtypecode IN ('INT', 'ADM')
                AND    f.RunNo > Trigger_Data.StartRunNo
                AND    f.RunNo <= Trigger_Data.EndRunNo
                AND    f.datetrans <= (SELECT max(datestatchge)
                                       FROM   status s
                                       WHERE  s.statuscode = 'S'
                                       AND    s.acctno = Trigger_Data.acctno
                                       AND    s.datestatchge  > DATEADD(Month,-1,Trigger_Data.RunDate)
                                       AND    s.datestatchge <= Trigger_Data.RunDate)),0) )


PRINT '** DDF **'
-- Charges Collected for Seasoned account securitised pervious month
UPDATE Trigger_Data
SET    Charges = (ISNULL(Charges,0) -
        ISNULL((SELECT sum(f.transvalue)
                FROM   fintrans f
                WHERE  f.acctno        = Trigger_Data.acctno
                and    f.RunNo         > Trigger_Data.StartRunNo
                AND    f.RunNo        <= Trigger_Data.EndRunNo
                AND    f.transtypecode = 'DDF'
                AND    f.transvalue    < 0),0) )

-- Updating value for Cumulative Charges collected from Month June
UPDATE Trigger_Data
SET    CumCharges = ISNULL(Charges,0) + 
        ISNULL((SELECT SUM(td.charges)
                FROM   Trigger_Data td
                WHERE  td.acctno = Trigger_Data.acctno
                AND    td.MonthYear > CONVERT(DATETIME,'02 May 2004',106)
                AND    td.MonthYear <= DATEADD(MONTH,-1,Trigger_Data.MonthYear)),0) 
WHERE  Trigger_Data.MonthYear > CONVERT(DATETIME,'02 Jun 2004',106)

-- Update balance to include Cumulative Charges to have the same balance as Service Report
UPDATE Trigger_Data
SET    Outstbalcorr = ISNULL(Outstbalcorr,0) + ISNULL(CumCharges,0) + ISNULL(RecoveryPay,0)

-- Update ageing balance to be same as service report ageing balance
-- Balance excluding chrages but including charges added and write off and recovery payment
UPDATE Trigger_Data
SET    AgeingBalance = ISNULL(AgeingBalance,0) + ISNULL(CumCharges,0) + ISNULL(RecoveryPay,0)

--
-- REQUIRES Trigger_Data table from here on
--

-- retrieve accounts which may have a fully delivered date later than
-- the date of delivery and recalculate

PRINT ' .. Re-calculate arrears'
declare @acctno   char(12),
        @counter  integer

DECLARE datedel_cursor CURSOR 
FOR     SELECT DISTINCT td.acctno 
        FROM   Trigger_Data td
        WHERE  EXISTS (SELECT * from agreement ag, fintrans f
                       WHERE  ag.AcctNo = td.AcctNo
                       AND    f.acctno = ag.acctno
                       AND    f.Transtypecode IN ('DEL', 'GRT', 'ADD') 
                       AND    f.RunNo > td.SecRunNo )

OPEN datedel_cursor

FETCH NEXT FROM datedel_cursor INTO @acctno

WHILE (@@fetch_status <> -1)
BEGIN
   IF (@@fetch_status <> -2)
   begin
       set @counter = @counter + 1
       if @counter % 100 = 0 print convert (varchar, @counter) + ' fully delivered dates updated'
       execute dbUpdateDateFullyDelivered @acctno = @acctno
   END
   FETCH NEXT FROM datedel_cursor INTO @acctno
END

CLOSE datedel_cursor
DEALLOCATE datedel_cursor


--IF EXISTS (SELECT table_name FROM tempdb.INFORMATION_SCHEMA.TABLES
--           WHERE table_name like '#Trig_Sec_Arrears_Rpt__%')
--BEGIN
--    DROP TABLE #Trig_Sec_Arrears_Rpt
--END


PRINT ' .. Create table #Trig_Sec_Arrears_Rpt'
DECLARE @enddate DATETIME,@datefirstrun  DATETIME     -- First ever securitised date

DECLARE @daytoday INT,@monthtoday INT, @yeartoday INT

select  @datefirstrun = MIN(datefinish) from interfacecontrol where interface = 'securitise'



SELECT  td.MonthYear,
        td.AcctNo,
        s.datesecuritised,
        a.termstype,
        a.accttype,
        ip.instalamount,
        ip.datefirst, 
        ip.instalno,
        ag.datefullydelivered,
        CONVERT(MONEY,0) as deposit,
        CONVERT(MONEY,0) as amountdue,
        CONVERT(MONEY,0) as amountpaid,
        CONVERT(MONEY,0) AS arrears,
        convert(money,0) AS balanceexcharges,
        dayofm = datepart(day,datefirst), 
        monthofy =datepart(month,datefirst),  
        yearofy =datepart(year,datefirst), 
        numberofinstdue = convert(integer, 0)
INTO    #Trig_Sec_Arrears_Rpt
FROM    Trigger_Data td, sec_account s, acct a, instalplan ip, agreement ag
WHERE   s.acctno = td.AcctNo
AND     a.acctno = td.acctno 
AND     ip.acctno = td.acctno 
AND     ag.acctno = td.acctno

CREATE CLUSTERED INDEX ix_#Trig_Sec_Arrears_Rpt ON #Trig_Sec_Arrears_Rpt (acctno, MonthYear)

-- Getting the value for Deposit for account set up with deposit

PRINT ' .. Update table #Trig_Sec_Arrears_Rpt'
UPDATE  #Trig_Sec_Arrears_Rpt
SET     deposit = ISNULL(ag.deposit,0)
FROM    agreement ag
WHERE   ag.acctno = #Trig_Sec_Arrears_Rpt.acctno
AND     ag.deposit > 0

-- Getting value for instalamount for instpredeposit account

UPDATE  #Trig_Sec_Arrears_Rpt 
SET     deposit = ISNULL(#Trig_Sec_Arrears_Rpt.instalamount, 0)
FROM    termstype tt
WHERE   #Trig_Sec_Arrears_Rpt.deposit = 0 
AND     tt.termstype = #Trig_Sec_Arrears_Rpt.termstype
AND     tt.instalpredel = 'Y'

--deferred accounts have extra period before first instalment due

UPDATE  #Trig_Sec_Arrears_Rpt
SET     datefirst = dateadd (month, ac.mthsdeferred, #Trig_Sec_Arrears_Rpt.datefirst)
FROM    accttype ac
WHERE   ac.accttype = #Trig_Sec_Arrears_Rpt.accttype
AND     ac.mthsdeferred > 0

-- date of first instalment may be increased if account was not fully delivered when delivery date set
-- Setting the datefirst to be one month after the datesecuritised if it's earlier then datesecuritised

UPDATE  #Trig_Sec_Arrears_Rpt 
SET     datefirst = DATEADD(MONTH, 1,datefullydelivered),
        dayofm    = datepart(day,DATEADD(MONTH, 1,datefullydelivered)), 
        monthofy  = datepart(month,DATEADD(MONTH, 1,datefullydelivered)),  
        yearofy   = datepart(year,DATEADD(MONTH, 1,datefullydelivered)) 
WHERE   datefirst < DATEADD(MONTH, 1,datefullydelivered)

-- also for newly securitised accounts should not have any accounts with arrears for unseasoned
-- achieve this by setting date of first instalment to be one month > date securitised but not
-- for date of first-ever securitisation run

UPDATE  #Trig_Sec_Arrears_Rpt
SET     datefirst = DATEADD(MONTH, 1,datesecuritised),
        dayofm    = datepart(day,DATEADD(MONTH, 1,datesecuritised)), 
        monthofy  = datepart(month,DATEADD(MONTH, 1,datesecuritised)),  
        yearofy   = datepart(year,DATEADD(MONTH, 1,datesecuritised))
WHERE   datesecuritised > @datefirstrun
AND     datesecuritised > datefullydelivered

-- RD 19/08/04 Added
update #Trig_Sec_Arrears_Rpt  
set    numberofinstdue = 12*(@yeartoday-yearofy)+@monthtoday-monthofy 
--updating where due today -should this be less <= or <
update #Trig_Sec_Arrears_Rpt  
set    numberofinstdue = numberofinstdue +1 where dayofm < @daytoday

-- Amount due number of months between the date of first instalment 
-- AND today's date + the first instalment + the deposit 

-- 
UPDATE #Trig_Sec_Arrears_Rpt 
SET    amountdue = (numberofinstdue* ISNULL(instalamount,0)) + ISNULL(deposit,0)
where  datefirst >'1-jan-1990'  -- but should be recent

-- RD 19/08/04 Replaced with code above
--UPDATE  #Trig_Sec_Arrears_Rpt 
--SET     amountdue = DATEDIFF(MONTH, datefirst, MonthYear) * ISNULL(instalamount,0) + ISNULL(deposit,0)
--WHERE   datefirst > CONVERT(DATETIME,'1 Jan 1990',106)  -- but should be recent

-- Getting the value for amount paid exclude charges
UPDATE  #Trig_Sec_Arrears_Rpt 
SET     amountpaid = (SELECT ISNULL(SUM(transvalue),0)
                      FROM   fintrans f
                      WHERE  f.acctno = #Trig_Sec_Arrears_Rpt.acctno
                      AND    f.Transtypecode NOT IN ('DEL', 'GRT','ADD','FEE','DDF','ADM', 'INT')
                      AND    f.datetrans <= #Trig_Sec_Arrears_Rpt.MonthYear)

UPDATE #Trig_Sec_Arrears_Rpt
SET    arrears = (ISNULL(amountdue,0) + ISNULL(amountpaid,0) ) 

-- but arrears cannot exceed outstanding balance
UPDATE #Trig_Sec_Arrears_Rpt
SET    balanceexcharges = (SELECT ISNULL(SUM(transvalue),0)
                           FROM   fintrans f
                           WHERE  f.acctno = #Trig_Sec_Arrears_Rpt.acctno
                           AND    f.Transtypecode NOT IN ('ADM', 'INT','DDF','FEE')
                           AND    f.datetrans <= #Trig_Sec_Arrears_Rpt.MonthYear)
                    

UPDATE #Trig_Sec_Arrears_Rpt
SET    arrears = balanceexcharges
WHERE  arrears > balanceexcharges

-------------------------------------------------------------------------------
------------------------------- END OF ARREARS --------------------------------
-------------------------------------------------------------------------------

PRINT 'Updating Months Arrears '
PRINT getdate()

-- Update number of months in arrears
UPDATE Trigger_Data
SET    Arrears = tr.Arrears
FROM   #Trig_Sec_Arrears_Rpt tr
WHERE  tr.MonthYear = Trigger_Data.MonthYear
AND    tr.Acctno    = Trigger_Data.AcctNo

-- RD 19/08/04 Replaced with code below
-- Mths_arrs = ISNULL(cast(0.5 + ((arrears - Charges) / instalamount) AS integer),0) 
UPDATE Trigger_Data
SET    Mths_arrs = isnull(FLOOR(arrears / instalamount),0) 
WHERE  instalamount > 0


-- RD 09/08/04 Added to age account correctly where pass their datelast
UPDATE Trigger_Data
SET    Mths_arrs = Mths_arrs + 
             DATEDIFF(MONTH,DATEADD(MONTH,instalno,DATEADD(MONTH,+1,Datefullydelivered)),r.RunDate)
FROM   Report_Dates r
WHERE  Mths_arrs <> 0
AND    dateadd(month,instalno,DATEADD(Month,+1,Datefullydelivered)) < r.RunDate



-- Group the balances by the number of months in arrears
--IF EXISTS (SELECT table_name FROM tempdb.INFORMATION_SCHEMA.TABLES
--           WHERE table_name like '#Trigger_Data_Ageing__%')
--BEGIN
--    DROP TABLE #Trigger_Data_Ageing
--END

PRINT ' .. Create table Trigger_Data_Ageing'
SELECT MonthYear, SUM(Outstbalcorr) AS Outstbal, 
       ISNULL(SUM(CASE WHEN Mths_arrs < 0 THEN AgeingBalance ELSE  null end),0.0) AS 'In_Advance' ,
       ISNULL(SUM(CASE WHEN Mths_arrs = 0 THEN AgeingBalance ELSE  null end),0.0) AS 'Current' ,
       ISNULL(SUM(CASE WHEN Mths_arrs = 1 THEN AgeingBalance ELSE  null end),0.0) AS 'In_One_Month_Arrs' ,
       ISNULL(SUM(CASE WHEN Mths_arrs > 1 AND Mths_arrs !> 2 THEN AgeingBalance ELSE  null end),0.0) AS 'One_to_Two',
       ISNULL(SUM(CASE WHEN Mths_arrs > 2 AND Mths_arrs !> 3 THEN AgeingBalance ELSE  null end),0.0) AS 'Two_to_Three',
       ISNULL(SUM(CASE WHEN Mths_arrs > 3 AND Mths_arrs !> 4 THEN AgeingBalance ELSE  null end),0.0) AS 'Three_to_Four',
       ISNULL(SUM(CASE WHEN Mths_arrs > 4 AND Mths_arrs !> 5 THEN AgeingBalance ELSE  null end),0.0) AS 'Four_to_Five',
       ISNULL(SUM(CASE WHEN Mths_arrs > 5 AND Mths_arrs !> 6 THEN AgeingBalance ELSE  null end),0.0) AS 'Five_to_Six',
       ISNULL(SUM(CASE WHEN Mths_arrs > 6 AND Mths_arrs !> 7 THEN AgeingBalance ELSE  null end),0.0) AS 'Six_to_Seven',
       ISNULL(SUM(CASE WHEN Mths_arrs > 7 AND Mths_arrs !> 8 THEN AgeingBalance ELSE  null end),0.0) AS 'Seven_to_Eight',
       ISNULL(SUM(CASE WHEN Mths_arrs > 8 AND Mths_arrs !> 9 THEN AgeingBalance ELSE  null end),0.0) AS 'Eight_to_Nine',
       ISNULL(SUM(CASE WHEN Mths_arrs > 9 AND Mths_arrs !> 10 THEN AgeingBalance ELSE  null end),0.0) AS 'Nine_to_Ten',
       ISNULL(SUM(CASE WHEN Mths_arrs > 10 AND Mths_arrs !> 11 THEN AgeingBalance ELSE  null end),0.0) AS 'Ten_to_Eleven',
       ISNULL(SUM(CASE WHEN Mths_arrs > 11 AND Mths_arrs !> 12 THEN AgeingBalance ELSE  null end),0.0) AS 'Eleven_to_Twelve',
       ISNULL(SUM(CASE WHEN Mths_arrs > 12 THEN OutstbalCorr ELSE  null END),0.0) AS 'Greater_than_Twelve'
INTO   #Trigger_Data_Ageing
FROM   Trigger_Data
GROUP BY MonthYear


-- Populate the Total Amount
PRINT ' .. Update Summary12 Total_OS'
UPDATE Summary12
SET    Total_OS = t.Outstbal
FROM   #Trigger_Data_Ageing t
WHERE  Summary12.MonthYear = t.MonthYear


---------------------------- Moodys 3 months ----------------------------

PRINT '** Update data for Moodys **'
PRINT '** ---------------------- **'
PRINT ''


PRINT ''
PRINT '** 3 Months Delinquents Ratio **'
PRINT ''

-- Set total amount for 3 months in arrears
UPDATE Summary12
SET    M3_Amount = t.Three_to_Four
FROM   #Trigger_Data_Ageing t
WHERE  Summary12.MonthYear = t.MonthYear


-- Calc ratio (but only for the new rows)
UPDATE Summary12
SET    M3_Ratio =
         ISNULL((SELECT ((CONVERT(FLOAT,Summary12.M3_Amount) / CONVERT(FLOAT,s.Total_OS)) * 100)
                 FROM   Summary12 s
                 WHERE  s.MonthYear = DATEADD(MONTH,-3,Summary12.MonthYear)
                 AND    s.Total_OS != 0),0) -- DSR Ensure against divide by zero
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear


-- Calc rolling Average (but only for the new rows)
UPDATE Summary12
SET    M3_Average =
         ISNULL((SELECT AVG(s.M3_Ratio)
                 FROM   Summary12 s
                 WHERE  s.MonthYear BETWEEN DATEADD(MONTH,-2,Summary12.MonthYear)
                 AND    Summary12.MonthYear),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

-- Check whether the average breaches the set ratio (but only for the new rows)
UPDATE Summary12
SET    M3_Breach = CASE WHEN (Summary12.M3_Average > t.M3_Threshold)
                   THEN 'Breach' ELSE '-' END
FROM   Report_Dates r, Summary12_Threshold t
WHERE  Summary12.MonthYear = r.MonthYear


---------------------------- Moodys 6 months ----------------------------

PRINT ''
PRINT '** 6 Months Delinquents Ratio **'
PRINT ''

-- Set total amount for 6 months in arrears
UPDATE Summary12
SET    M6_Amount = t.Six_to_Seven
FROM   #Trigger_Data_Ageing t
WHERE  Summary12.MonthYear = t.MonthYear


-- Calc ratio (but only for the new rows)
UPDATE Summary12
SET    M6_Ratio =
         ISNULL((SELECT ((CONVERT(FLOAT,Summary12.M6_Amount) / CONVERT(FLOAT,s.Total_OS)) * 100)
                 FROM   Summary12 s
                 WHERE  s.MonthYear = DATEADD(MONTH,-6,Summary12.MonthYear)
                 AND    s.Total_OS != 0),0)  -- DSR Ensure against divide by zero
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear


-- Calc rolling Average (but only for the new rows)
UPDATE Summary12
SET    M6_Average =
         ISNULL((SELECT AVG(s.M6_Ratio)
                 FROM   Summary12 s
                 WHERE  s.MonthYear BETWEEN DATEADD(MONTH,-2,Summary12.MonthYear)
                 AND    Summary12.MonthYear),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

-- Check whether the average breaches the set ratio (but only for the new rows)
UPDATE Summary12
SET    M6_Breach = CASE WHEN (Summary12.M6_Average > t.M6_Threshold)
                   THEN 'Breach' ELSE '-' END
FROM   Report_Dates r, Summary12_Threshold t
WHERE  Summary12.MonthYear = r.MonthYear


---------------------------- Moodys 12 months ----------------------------

PRINT ''
PRINT '** 12 Months Delinquents Ratio **'
PRINT ''

-- Set total amount for 12 months in arrears
UPDATE Summary12
SET    M12_Amount = t.Eleven_to_Twelve
FROM   #Trigger_Data_Ageing t
WHERE  Summary12.MonthYear = t.MonthYear


-- Calc ratio (but only for the new rows)
UPDATE Summary12
SET    M12_Ratio =
         ISNULL((SELECT ((CONVERT(FLOAT,Summary12.M12_Amount) / CONVERT(FLOAT,s.Total_OS)) * 100)
                 FROM   Summary12 s -- DSR look back 11 months according to spec
                 WHERE  s.MonthYear = DATEADD(MONTH,-11,Summary12.MonthYear)
                 AND    s.Total_OS != 0),0)  -- DSR Ensure against divide by zero
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear


-- Calc rolling Average (but only for the new rows)
UPDATE Summary12
SET    M12_Average =
         ISNULL((SELECT AVG(s.M12_Ratio)
                 FROM   Summary12 s
                 WHERE  s.MonthYear BETWEEN DATEADD(MONTH,-2,Summary12.MonthYear)
                 AND    Summary12.MonthYear),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

-- Check whether the average breaches the set ratio (but only for the new rows)
UPDATE Summary12
SET    M12_Breach = CASE WHEN (Summary12.M12_Average > t.M12_Threshold)
                    THEN 'Breach' ELSE '-' END
FROM   Report_Dates r, Summary12_Threshold t
WHERE  Summary12.MonthYear = r.MonthYear


---------------------------- Fitch 6 months ----------------------------

PRINT ''
PRINT '** Fitch Triggers **'
PRINT '** -------------- **'
PRINT ''


PRINT '** 6 months Plus Delinquencies **'
PRINT ''

UPDATE Summary12
SET    F_6_Mths_Pls = ((CONVERT(FLOAT,t.Five_To_Six) / CONVERT(FLOAT,Summary12.Total_OS)) * 100)
FROM   #Trigger_Data_Ageing t
WHERE  Summary12.MonthYear = t.MonthYear
AND    Summary12.Total_OS != 0   -- DSR Ensure against divide by zero


-- Check whether the average breaches the set ratio (but only for the new rows)
UPDATE Summary12
SET    F_Breach_6 = CASE WHEN (Summary12.F_6_Mths_Pls > t.F6_Threshold)
                    THEN 'Breach' ELSE '-' END
FROM   Report_Dates r, Summary12_Threshold t
WHERE  Summary12.MonthYear = r.MonthYear


---------------------------- Fitch 12 months ----------------------------

 PRINT '** 12 months Plus Delinquencies **'
PRINT ''

UPDATE Summary12
SET    F_12_Mths_Pls = ((CONVERT(FLOAT,t.Eleven_to_Twelve) / CONVERT(FLOAT,Summary12.Total_OS)) * 100)
FROM   #Trigger_Data_Ageing t
WHERE  Summary12.MonthYear = t.MonthYear
AND    Summary12.Total_OS != 0   -- DSR Ensure against divide by zero


-- Check whether the average breaches the set ratio (but only for the new rows)
UPDATE Summary12
SET    F_Breach_12 = CASE WHEN (Summary12.F_12_Mths_Pls > t.F12_Threshold)
                     THEN 'Breach' ELSE '-' END
FROM   Report_Dates r, Summary12_Threshold t
WHERE  Summary12.MonthYear = r.MonthYear


---------------------------- Collections ----------------------------
-- Based on Servicer Report but changed to calculate multiple months
-- ** Needs to be consistent with Servicer Report **

PRINT '** Collections **'
PRINT ''

-- Total Collections per month including any Insurance Claims (but only for the new rows)
-- and seasoned accounts

--
-- 7e from Servicer Report
--
UPDATE Summary12
SET    Coll_Amount =
       ISNULL((SELECT SUM(f.TransValue)
               FROM   Trigger_Data td, fintrans f
               WHERE  td.MonthYear = Summary12.MonthYear
               AND    ((td.RunDate <= CONVERT(DATETIME,'02 Jun 2004 08:00',106)  -- DSR June report is different
                        AND td.datedel         <= DATEADD(MONTH, -2,td.RunDate)
                        AND td.datesecuritised <= DATEADD(MONTH, -1,td.RunDate))
                       OR
                        (td.RunDate > CONVERT(DATETIME,'02 Jun 2004 08:00',106)
                         AND td.datesecuritised <= DATEADD(MONTH, -2,td.RunDate)))
               AND    f.acctno = td.acctno 
               AND    f.RunNo  > td.StartRunNo
               AND    f.RunNo <= td.EndRunNo
               AND    f.transtypecode
                      IN ('PAY', 'DDN', 'DDE', 'DDR', 'INS', 'COR', 'REF', 'RET', 'XFR', 'SCX','PEX')),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear


-- Total Collections per month including any Insurance Claims (but only for the new rows)
-- and new receivables this month
UPDATE Summary12
SET    Coll_Amount = Coll_Amount +
       ISNULL((SELECT SUM(f.TransValue)
               FROM   Trigger_Data td, fintrans f
               WHERE  td.MonthYear       = Summary12.MonthYear
               AND    td.datesecuritised BETWEEN DATEADD(Month,-1,td.RunDate) AND td.RunDate -- DSR Same as Servicer
               AND    td.AddTo           = 'Y'    -- DSR Only with AddTo to get Seasoned New Receivables
               AND    f.acctno           = td.acctno 
               AND    f.RunNo            > td.secrunno
               AND    f.RunNo           <= td.EndRunNo
               AND    f.transtypecode
                      IN ('PAY', 'DDN', 'DDE', 'DDR', 'INS', 'COR', 'REF', 'RET', 'XFR', 'SCX','PEX')),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear


-- Total Recoveries per month (but only for the new rows)
UPDATE Summary12
SET    Coll_Amount = Coll_Amount +
       ISNULL((SELECT SUM(f.TransValue)
               FROM   CustAcct ca, Fintrans f, Trigger_Data td
               WHERE  ca.custid        = 'BDWSECRECOVER' 
               AND    f.acctno         = ca.acctno 
               AND    td.acctno        = f.chequeno
               AND    f.RunNo          > td.StartRunNo  -- DSR do not use SecRunNo here
               AND    f.RunNo         <= td.EndRunNo
               AND    f.transtypecode  = 'DPY'),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

--
-- DSR 1 Sep 2004 - Add 7f from Servicer Report
--
        
UPDATE Summary12
SET    Coll_Amount = Coll_Amount +
       ISNULL((SELECT sum(f.transvalue)
               FROM   Trigger_Data td, fintrans f
               WHERE  td.MonthYear     = Summary12.MonthYear
               AND    ((td.RunDate <= CONVERT(DATETIME,'02 Jun 2004 08:00',106)  -- DSR June report is different
                        AND td.datedel  > DATEADD(MONTH,-2,td.RunDate)
                        AND td.datedel <= DATEADD(Month,-1,td.RunDate) 
                        AND td.datesecuritised  > DATEADD(Month,-2,td.RunDate)
                        AND td.datesecuritised <= DATEADD(MONTH,-1,td.RunDate))
                       OR
                        (td.RunDate > CONVERT(DATETIME,'02 Jun 2004 08:00',106)
                         AND td.datesecuritised  > DATEADD(Month,-2,td.RunDate)
                         AND td.datesecuritised <= DATEADD(MONTH,-1,td.RunDate)))
               AND    f.acctno         = td.acctno 
               AND    f.RunNo          > td.StartRunNo
               AND    f.RunNo         <= td.EndRunNo
               AND    f.transtypecode IN ('PAY', 'DDN', 'DDE', 'DDR', 'INS', 'COR', 'REF', 'RET', 'XFR', 'SCX', 'PEX')),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

UPDATE Summary12
SET    Coll_Amount = Coll_Amount +
       ISNULL((SELECT sum(f.transvalue)
              FROM   Trigger_Data td, fintrans f
              WHERE  td.MonthYear      = Summary12.MonthYear
              AND    td.datesecuritised BETWEEN DATEADD(MONTH, -1,td.RunDate) AND td.RunDate
              AND    td.AddTo          = 'N'
              AND    f.acctno          = td.acctno 
              AND    f.RunNo           > td.SecRunNo
              AND    f.RunNo          <= td.EndRunNo
              AND    f.transtypecode  IN ('PAY', 'DDN', 'DDE', 'DDR', 'INS', 'COR', 'REF', 'RET', 'XFR', 'SCX', 'PEX')),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

--
-- End of 7f
--

/*****************************************
*** DSR 2 Sep 2004 - NOT USED:
***-- Record the date within each month where an account was settled
***-- The result will be null where the account was not settled in that month
***UPDATE Trigger_Data
***SET    SettleDate =
***       (SELECT MAX(s.datestatchge)
***        FROM   Status s
***        WHERE  s.AcctNo        = Trigger_Data.AcctNo
***        AND    s.datestatchge  > DATEADD(Month,-1,Trigger_Data.RunDate)
***        AND    s.datestatchge <= Trigger_Data.RunDate
***        AND    s.StatusCode    = 'S'
***        AND    NOT EXISTS (SELECT * FROM status s1
***                           WHERE  s1.acctno = s.acctno
***                           AND    s1.datestatchge > s.datestatchge))
***
***
***-- RD 19/08/04 Added to only add charges considered collected furing the collection period
***UPDATE Summary12
***SET    Coll_Amount = Coll_Amount +
***       ISNULL((SELECT - SUM(charges)
***               FROM   Trigger_Data td
***               WHERE  td.MonthYear = Summary12.MonthYear),0)
***FROM   Report_Dates r
***WHERE  Summary12.MonthYear = r.MonthYear
*****************************************/

-- Deemed Collections - Dilutions  (but only for the new rows)
-- Subtract credits such as ods Returns and Repossesions summed
-- from seasoned accounts securitised at least two months earlier

-- RD 19/08/04 New code Dilutions and Adjustments
-- Point 8 AND 9 from the Servicer Report (BOTH seasoned and UNseasoned combined)
-- only take transaction during the collection period for seasoned accounts
UPDATE Summary12
SET    Coll_Amount = Coll_Amount +
       ISNULL((SELECT SUM(f.TransValue)
               FROM   Trigger_Data td, Fintrans f
               WHERE  td.MonthYear        = Summary12.MonthYear  -- DSR Join to Summary12
               AND    td.datesecuritised <= DATEADD(MONTH,-1,td.RunDate)
               AND    f.acctno            = td.acctno
               AND    f.RunNo             > td.StartRunNo
               AND    f.RunNo            <= td.EndRunNo
               AND    f.transtypecode    in ('DEL', 'ADJ', 'DDH', 'RPO', 'REP', 'GRT', 'RDL', 'MKT')
               AND    f.transvalue        < 0),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

-- Only for new receivables this month
UPDATE Summary12
SET    Coll_Amount = Coll_Amount +
       ISNULL((SELECT SUM(f.TransValue)
               FROM   Trigger_Data td, Fintrans f
               WHERE  td.MonthYear       = Summary12.MonthYear  -- DSR Join to Summary12
               AND    td.datesecuritised BETWEEN DATEADD(Month,-1,td.RunDate) AND td.RunDate -- DSR Same as Servicer
               AND    f.acctno           =  td.acctno
               AND    f.RunNo            >  td.SecRunNo
               AND    f.RunNo            <= td.EndRunNo
               AND    f.transtypecode    in ('DEL', 'ADJ', 'DDH', 'RPO', 'REP', 'GRT', 'RDL', 'MKT')
               AND    f.transvalue       < 0),0)
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear

-- Calc Collection Rate (but only for the new rows)
UPDATE Summary12
SET    Coll_Percentage = 
       ISNULL((SELECT ((CONVERT(FLOAT,Summary12.Coll_Amount) / CONVERT(FLOAT,s.Total_OS)) * 100)
               FROM   Summary12 s
               WHERE  s.MonthYear = DATEADD(Month,-1,Summary12.MonthYear)
               AND    s.Total_OS != 0),0)   -- DSR Ensure against divide by zero
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear


-- Check whether the rate breaches the set ratio (but only for the new rows)
UPDATE Summary12
SET    Coll_Breach = CASE WHEN (Summary12.Coll_Percentage > t.Col_Threshold)
                     THEN 'Breach' ELSE '-' END
FROM   Report_Dates r, Summary12_Threshold t
WHERE  Summary12.MonthYear = r.MonthYear


---------------------------- Rebates ----------------------------
-- Based on Servicer Report but changed to calculate multiple months
--  ** Needs to be consistent with Servicer Report **

PRINT '** Rebates **'
PRINT ''

-- Start of new code RD 01/07/2004
-- Amended for 1 Jun 2004 report - DSR 1 Sept 2004

-- Update rebate value with seasoned account delivered over 2 months
-- same as point 10 in service report
UPDATE Summary12
SET    Rebate_Amount = (SELECT ISNULL(SUM(f.transvalue),0)
                        FROM   Trigger_Data td, Fintrans f
                        WHERE  td.monthyear       = Summary12.Monthyear
                        AND    ((td.RunDate <= CONVERT(DATETIME,'02 Jun 2004 08:00',106)  -- DSR June report is different
                                 AND td.datedel         <= DATEADD(MONTH, -2,td.RunDate)
                                 AND td.datesecuritised <= DATEADD(MONTH, -1,td.RunDate))
                                OR
                                 (td.RunDate > CONVERT(DATETIME,'02 Jun 2004 08:00',106)
                                  AND td.datesecuritised <= DATEADD(MONTH, -2,td.RunDate)))
                        AND    f.acctno           = td.acctno
                        AND    f.RunNo            > td.StartRunNo -- DSR don't use SecRunNo here
                        AND    f.RunNo           <= td.EndRunNo
                        AND    f.transtypecode    = 'REB'
                        AND    NOT EXISTS (SELECT * FROM Fintrans f2
                                           WHERE  f2.acctno        = f.acctno
                                           AND    f2.transtypecode = 'ADD'
                                           AND    f2.RunNo         = f.RunNo))

-- Update rebate value for New receivable this month with add-to
UPDATE Summary12
SET    Rebate_Amount = ISNULL(Rebate_Amount,0) + 
            ISNULL((SELECT SUM(f.transvalue)
                    FROM   Trigger_Data td, Fintrans f
                    WHERE  td.monthyear      = Summary12.Monthyear
                    AND    td.datesecuritised BETWEEN DATEADD(Month,-1,td.RunDate) AND td.RunDate
                    AND    td.AddTo          = 'Y'  -- DSR WITH AddTo
                    AND    f.acctno          = td.acctno 
                    AND    f.RunNo           > td.SecRunNo
                    AND    f.RunNo          <= td.EndRunNo
                    AND    f.transtypecode   = 'REB'
                    AND    NOT EXISTS (SELECT * FROM Fintrans f2
                                       WHERE  f2.acctno        = f.acctno
                                       AND    f2.transtypecode = 'ADD'
                                       AND    f2.RunNo         = f.RunNo)),0) 


-- Calc Collection Rate (but only for the new rows)
-- This is divided by SEASONED receivables - so  back TWO months
UPDATE Summary12
SET    Rebate_Percentage = 
       ISNULL((SELECT ((CONVERT(FLOAT,Summary12.Rebate_Amount) / CONVERT(FLOAT,s.Total_OS)) * 100)
               FROM   Summary12 s
               WHERE  s.MonthYear = DATEADD(Month,-1,Summary12.MonthYear)
               AND    s.Total_OS != 0),0)   -- DSR Ensure against divide by zero
FROM   Report_Dates r
WHERE  Summary12.MonthYear = r.MonthYear


-- Check whether the rate breaches the set ratio (but only for the new rows)
UPDATE Summary12
SET    Rebate_Breach = CASE WHEN (Summary12.Rebate_Percentage > t.Reb_Threshold)
                       THEN 'Breach' ELSE '-' END
FROM   Report_Dates r, Summary12_Threshold t
WHERE  Summary12.MonthYear = r.MonthYear


------------------------ Quantitative Triggers ------------------------

PRINT '** Quantitative Triggers **'
PRINT ''

-- Quantitative Triggers are only for the current month
DECLARE @CurMonthYear DATETIME

SELECT  @CurMonthYear = MAX(MonthYear),
        @RptDate      = MAX(RptDate)
FROM Report_Dates

-- Only one summary row stored for the latest report
--DELETE FROM Summary12_QT

--
-- OutStanding Seasoned Receivables
--
PRINT '.. OutStanding Seasoned Receivables'
PRINT ''

DECLARE @SeasonedOS MONEY

SELECT  @SeasonedOS = isnull(SUM(td.OutStBalCorr),0)
FROM    Trigger_Data td
WHERE   td.MonthYear = @CurMonthYear
--AND     td.DateSecuritised <= DATEADD(Month,-1,td.RunDate)  -- Seasoned


-- Only remove row if the value is not null otherwise leave the data as per previous run
IF @SeasonedOS != CONVERT(MONEY,0)
DELETE FROM Summary12_QT

-- Only insert row if the value is not null otherwise leave the data as per previous run
IF @SeasonedOS != CONVERT(MONEY,0)
   INSERT INTO Summary12_QT (RptDate, Seasoned_OS, Seasoned_OS_Breach)
   SELECT  @RptDate,
           @SeasonedOS,
           CASE WHEN (@SeasonedOS < t.Sea_Threshold)
                THEN 'Breach' ELSE '-' END
   FROM    Summary12_Threshold t


--
-- Weighted Average Portfolio Life
--
PRINT '.. Weighted Average Portfolio Life'
PRINT ''

DECLARE @TotalBookOS        FLOAT,
        @Maturity_Profile   FLOAT,
        @CalcBook           FLOAT,
        @Result             FLOAT,
        @WAverage           FLOAT

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

-- Update months remaining left to run on an agreement
UPDATE  Trigger_Data
SET     MthsLeft = FLOOR(BalanceExBDW / InstalAmount)
WHERE   InstalAmount > 0

SELECT  @Maturity_Profile = AVG(MthsLeft) FROM Trigger_Data
SELECT  @TotalBookOS = SUM(BalanceExBDW) FROM Trigger_Data

IF (ISNULL(@Maturity_Profile,0) = 0) SET @Maturity_Profile = 1

SET @CalcBook = @TotalBookOS / @Maturity_Profile
SET @Result = 0

WHILE @Maturity_Profile > 0
BEGIN
  SET @Result = @Result + (@CalcBook * @Maturity_Profile)
  SET @Maturity_Profile = @Maturity_Profile - 1
END

SET @WAverage = (@Result / @TotalBookOS)

UPDATE Summary12_QT
SET    Weighted_Avg_Life = @WAverage,
       WAL_Breach = CASE WHEN (@WAverage > t.WAL_Threshold)
                         THEN 'Breach' ELSE '-' END
FROM   Summary12_Threshold t



---------------------------- Summary totals ----------------------------
PRINT '.. Summary totals'
PRINT ''

UPDATE Summary12_QT
SET    M3_Breach_Count = (SELECT COUNT(*) FROM Summary12 WHERE M3_Breach = 'Breach')

UPDATE Summary12_QT
SET    M6_Breach_Count = (SELECT COUNT(*) FROM Summary12 WHERE M6_Breach = 'Breach')

UPDATE Summary12_QT
SET    M12_Breach_Count = (SELECT COUNT(*) FROM Summary12 WHERE M12_Breach = 'Breach')

UPDATE Summary12_QT
SET    F_Breach_6_Count = (SELECT COUNT(*) FROM Summary12 WHERE F_Breach_6 = 'Breach')

UPDATE Summary12_QT
SET    F_Breach_12_Count = (SELECT COUNT(*) FROM Summary12 WHERE F_Breach_12 = 'Breach')

UPDATE Summary12_QT
SET    Coll_Breach_Count = (SELECT COUNT(*) FROM Summary12 WHERE Coll_Breach = 'Breach')

UPDATE Summary12_QT
SET    Rebate_Breach_Count = (SELECT COUNT(*) FROM Summary12 WHERE Rebate_Breach = 'Breach')


---------------------------- Finish ----------------------------
PRINT '.. Finish.'
PRINT ''

-- Dropping Temp tables created

DROP TABLE #Trig_Sec_Arrears_Rpt
DROP TABLE #Trigger_Data_Ageing

END
GO

--/****************************************************/
--End of Store Procedure for report 16 - amortisation
--/****************************************************/