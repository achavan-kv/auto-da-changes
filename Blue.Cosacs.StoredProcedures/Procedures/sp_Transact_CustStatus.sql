SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[sp_Transact_CustStatus]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[sp_Transact_CustStatus]
GO

CREATE PROCEDURE sp_Transact_CustStatus

--------------------------------------------------------------------------------
--
-- Project      : CoSACS Transact r 2002 Strategic Thought Ltd.
-- File Name    : Transact_Status.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Transact Customer Account Status history
-- Author       : D Richardson
-- Date         : 7 May 2002
--
-- The following values are calculated from the Customer Account history.
-- 'NOW'  means the CurrStatus value (or last status before being settled) is used;
-- 'EVER' means the HighstStatus value is used.
--
--  1) Number of Current Accounts
--  2) Number of Settled Accounts
--  3) Most recent (to change status) Current Account Status NOW (MAX CurrStatus)
--  4) Most recent (to change status) penultimate Settled Account Status NOW (MAX Penultimate CurrStatus)
--  5) Largest Settled Account Status EVER (MAX HighstStatus)
--  6) Size Code of the largest Settled Account
--  7) Total of all ongoing instalments
--  8) Total of all Outstanding Balances
--  9) Number of credit applications in the last 90 days
-- 10) Number of rejected credit applications in the last 90 days

-- CR262 requires the following calculated values:
-- 11) Highest Status of Current Account EVER (MAX HighstStatus)
-- 12) Highest Status of Settled Account EVER (MAX HighstStatus)
-- 13) Highest Status of Current Account NOW  (MAX CurrStatus)
-- 14) Highest Status of Settled Account NOW  (MAX Penultimate CurrStatus)
-- 15) Weighted Average Status of Current Account
-- 16) Weighted Average Status of Settled Account
--
-- Weighted Average is calculated on the last 18 months of each account's
-- performance. Each status value is multiplied by the number of days in
-- that status code. These values are summed and divided by 546 days (18 months).
--
-- The account being sanctioned and cancelled accounts are excluded.
-- Accounts are only included where considered complete by the Agreement
-- Delivery Flag being set to 'Y'.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 07/05/02  DSR CR262 / FR76209 Creation
-- 10/02/03  AA  Agreement size codes changed for new customer and no settled accounts
-- 01/04/03  AA  Fixing error for above
-- 14/04/03  DSR CR396 Exclude CASH accounts (Also excluded from Account_History table)
-- 16/05/04  AA updating as incorrect No of settled accounts appearing
-- 13/05/10 jec UAT153 Scoring not calculated correctly
-- 16/06/14  IP #17478 - incorrect value returned for parameter @poTotalOutStBal
--------------------------------------------------------------------------------

    -- Parameters
    @piCustid           VARCHAR(15),
    @piAcctNo           VARCHAR(12),
    
    @poCurNumAcc        INTEGER      OUTPUT,
    @poSetNumAcc        INTEGER      OUTPUT,
    @poCurRecent        CHAR(1)     OUTPUT,
    @poSetRecent        CHAR(1)     OUTPUT,
    @poCurHiEver        CHAR(1)     OUTPUT,
    @poSetHiEver        CHAR(1)     OUTPUT,
    @poCurHiNow         CHAR(1)     OUTPUT,
    @poSetHiNow         CHAR(1)     OUTPUT,
    @poCurWeightAvg     FLOAT        OUTPUT,
    @poSetWeightAvg     FLOAT        OUTPUT,
    @poSetLargest       CHAR(1)     OUTPUT,
    @poSetLargestSize   CHAR(1)     OUTPUT,
    @poTotalInstal      MONEY        OUTPUT,
    @poTotalOutStBal    MONEY        OUTPUT,
    @poNumAppsLst90     SMALLINT     OUTPUT,
    @poRejLst90         CHAR(1)     OUTPUT

AS DECLARE
    -- Local variables
    @SQLError           INTEGER,
    @CurObjectName      VARCHAR(30),
    @tmpAcctNo          CHAR(12),
    @tmpAgrmtTotal      MONEY,
    @AvgRowcount        INTEGER,
    @HistRowcount       INTEGER


BEGIN

    SET NOCOUNT ON

    -- Current Object Name for error logging
    SET @CurObjectName = '(SP) Transact_CustStatus'

    -- Default values for 'No Info'
    SET @poCurNumAcc      = 0
    SET @poSetNumAcc      = 0
    SET @poCurRecent      = '0'
    SET @poSetRecent      = '0'
    SET @poCurHiEver      = '0'
    SET @poSetHiEver      = '0'
    SET @poCurHiNow       = '0'
    SET @poSetHiNow       = '0'
    SET @poCurWeightAvg   = 0.0
    SET @poSetWeightAvg   = 0.0
    SET @poSetLargest     = '0'
    SET @poSetLargestSize = '0'
    SET @poTotalInstal    = 0
    SET @poTotalOutStBal  = 0
    SET @poNumAppsLst90   = 0
    SET @poRejLst90       = ' '


    -- Number of applications in the last 90 days
    
    SELECT @poNumAppsLst90 = COUNT(*)
    FROM   Proposal
    WHERE  CustId  = @piCustId
    AND    AcctNo <> @piAcctNo
    AND    DATEADD(DAY, 90, DateProp) > GETDATE()

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select number of apps in last 90 days',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END
    
    -- Any rejects in the last 90 days
    
    SELECT @poRejLst90 = CASE WHEN COUNT(*) > 0 THEN 'Y' ELSE 'N' END
    FROM   Proposal
    WHERE  CustId  = @piCustId
    AND    AcctNo <> @piAcctNo
    AND    DATEADD(DAY, 90, DateProp) > GETDATE()
    AND    PropResult IN ('D','X')

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select any rejects in last 90 days',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END

    ----------------------------------------------------------------------------


    -- Create a temp table of all Account Status Codes for this Customer Id
    -- Exclude the account being sanctioned and cancelled accounts
    -- Only include accounts considered complete with Delivery Flag = 'Y'
    -- For multiple Status Codes on the same date use the highest Status Code
    -- Note that 'Days' needs to be FLOAT
  DECLARE @NumMonthsStatustocheck INT 
  
  SELECT @NumMonthsStatustocheck = CONVERT(INT,value) FROM CountryMaintenance 
  WHERE codename ='WorstStatusPeriod'  
  
  IF @NumMonthsStatustocheck = 0 
	SET @NumMonthsStatustocheck = 500	
  
SELECT a.AcctNo, a.CurrStatus, a.AgrmtTotal, a.OutStBal,
            MAX(isnull(s.StatusCode,1)) AS StatusCode,
           s.DateStatChge,
           CAST(0.0 AS FLOAT) AS Days
    INTO   #temp_AvgStatus
    FROM   CustAcct ca LEFT outer join Agreement ag on ca.acctno = ag.acctno,	-- UAT153 
			Acct a  join status s on s.AcctNo = A.AcctNo and s.statuscode not in ('S','0','U')
			AND  s.datestatchge > DATEADD(MONTH,-@NumMonthsStatustocheck,GETDATE())        
	WHERE  ca.CustId       = @piCustId
    AND    ca.HldOrJnt     = 'H'
    AND    ca.AcctNo      <> @piAcctno
    --AND    ag.AcctNo       = ca.AcctNo		-- UAT153
    AND    ISNULL(ag.DeliveryFlag,'Y') = 'Y'	-- UAT153
    AND    a.AcctNo        = ca.AcctNo
    AND    a.AcctType     != 'C'        -- CR396 Exclude CASH accounts
    
    -- AA 27/09/04 do not exclude cancelled accounts CR 447
    --AND NOT EXISTS (SELECT AcctNo FROM Cancellation WHERE AcctNo = a.AcctNo)
    GROUP BY a.AcctNo, a.CurrStatus,  a.AgrmtTotal, a.OutStBal ,
             s.DateStatChge
	SELECT @SQLError = @@ERROR, @AvgRowcount = @@ROWCOUNT
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Create temp table',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END
    
    -- remove accounts which were ealier than number of months to check and have no balance so should not be included as not current
    delete FROM #temp_AvgStatus WHERE datestatchge IS NULL 
    AND outstbal < 1
    
	-- need to insert previous status from just before date as within period
	INSERT INTO #temp_AvgStatus ( acctno ,
		StatusCode,
		DateStatChge,
		Days,
		currstatus 
	) SELECT s.acctno,s.statuscode,s.datestatchge,0  ,a.currstatus
	FROM status s, acct a 
	WHERE s.datestatchge = (SELECT MAX(ss.datestatchge) FROM status ss 
	WHERE ss.acctno = s.acctno AND s.datestatchge < DATEADD(MONTH,-@NumMonthsStatustocheck,GETDATE())
	and ss.statuscode not in ('S','0','U'))
	AND EXISTS (SELECT * FROM #temp_AvgStatus t WHERE t.acctno= s.acctno)
	AND a.acctno = s.acctno
    
    --updating this for those accounts which are missing a record from the status table
    update #temp_AvgStatus 
    set DateStatChge = datelastpaid
    from acct where acct.acctno =#temp_AvgStatus.acctno and DateStatChge is null

    IF @AvgRowcount > 0
    BEGIN

        -- Calculate the number of days in each Status Code
        -- The last Status Code is up to today unless it is Settled 'S'
        -- Each Settled Status will be left as zero days so it is ignored
    
        UPDATE #temp_AvgStatus
        SET Days = (SELECT DATEDIFF(Day,#temp_AvgStatus.DateStatChge,ISNULL(MIN(b.DateStatChge),GETDATE()))
                    FROM   #temp_AvgStatus b
                    WHERE  b.AcctNo       = #temp_AvgStatus.AcctNo
                    AND    b.DateStatChge > #temp_AvgStatus.DateStatChge)
        WHERE StatusCode <> 'S'

        SET @SQLError = @@ERROR
        IF (@SQLError <> 0)
        BEGIN
            EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                                   @piMsg        = 'Update temp table with days',
                                   @piObjectName = @CurObjectName
            RETURN @SQLError
        END


        -- Only the last 18 months (546 days) excluding settled periods 
        -- will be used for the average
    
        UPDATE #temp_AvgStatus
        SET Days = ISNULL((SELECT 546 - SUM(b.Days)
                           FROM   #temp_AvgStatus b
                           WHERE  b.AcctNo       = #temp_AvgStatus.AcctNo
                           AND    b.DateStatChge > #temp_AvgStatus.DateStatChge
                           HAVING (546 - SUM(b.Days)) < #temp_AvgStatus.Days),Days)

        SET @SQLError = @@ERROR
        IF (@SQLError <> 0)
        BEGIN
            EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                                   @piMsg        = 'Update temp table for last 18 months',
                                   @piObjectName = @CurObjectName
            RETURN @SQLError
        END


        -- Calculate the highest Weighted Average for Settled Accounts
    
        SELECT TOP 1 @tmpAcctNo      = AcctNo,
                     @poSetWeightAvg = ISNULL(ROUND(SUM(StatusCode * Days) / SUM(Days),1),0)
        FROM   #temp_AvgStatus
        WHERE  ISNUMERIC(StatusCode) = 1
        AND    Days > 0
        AND    CurrStatus = 'S'
        GROUP BY AcctNo
        ORDER BY SUM(StatusCode * Days) / SUM(Days) DESC

        SET @SQLError = @@ERROR
        IF (@SQLError <> 0)
        BEGIN
            EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                                   @piMsg        = 'Select Settled Weighted Average',
                                   @piObjectName = @CurObjectName
            RETURN @SQLError
        END


        -- Calculate the highest Weighted Average for Current Accounts
    
        SELECT TOP 1 @tmpAcctNo      = AcctNo,
                     @poCurWeightAvg = ISNULL(ROUND(SUM(StatusCode * Days) / SUM(Days),1),0)
        FROM   #temp_AvgStatus
        WHERE  ISNUMERIC(StatusCode) = 1
        AND    Days > 0
        AND    CurrStatus <> 'S'
        GROUP BY AcctNo
        ORDER BY SUM(StatusCode * Days) / SUM(Days) DESC

        SET @SQLError = @@ERROR
        IF (@SQLError <> 0)
        BEGIN
            EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                                   @piMsg        = 'Select Current Weighted Average',
                                   @piObjectName = @CurObjectName
            RETURN @SQLError
        END


 
    END
    
    -------------------------------------------------------------------------------


    -- Add any history accounts to the temp table
    
    --INSERT INTO #temp_AvgStatus
    --    (AcctNo, CurrStatus, HighstStatus, AgrmtTotal, StatusCode, DateStatChge, Days)
    --SELECT ah.AcctNo, 'S', ah.SettledStatus, ags.MaxValue, ah.SettledStatus, ah.SettledDate, 1.0
    --FROM   Account_History ah, AgreementSize ags
    --WHERE  ah.Custid                    = @piCustId
    --AND    ah.AcctNo                   <> @piAcctNo
    --AND    RIGHT(LEFT(ah.AcctNo,4),1)  != '4'        -- CR396 Exclude CASH accounts
    --AND    ags.SizeCode                 = ah.CodeSize

    SELECT @SQLError = @@ERROR, @HistRowcount = @@ROWCOUNT
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Insert history into temp table',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END
   -- Number of Settled Accounts, highest status now and highest status ever
    -- Highest Status 'now' for a Settled Account is the penultimate Status
    
    SELECT @poSetNumAcc = ISNULL(COUNT(*),0),
           @poSetHiEver = ISNULL(MAX(StatusCode),0) -- Highest status ever (during period). 
    FROM   #temp_AvgStatus
    WHERE  CurrStatus = 'S'

    SELECT @poSetHiNow  = ISNULL(MAX(StatusCode),0) -- Highest status when settled
    FROM   #temp_AvgStatus
    WHERE  CurrStatus = 'S'
	AND datestatchge = (SELECT MAX(datestatchge) FROM #temp_AvgStatus t WHERE t.acctno= #temp_AvgStatus.acctno
	AND t.currstatus = 'S')

    select @poSetNumAcc = ISNULL(COUNT(distinct (acctno)),0)  FROM   #temp_AvgStatus
    WHERE  CurrStatus = 'S'
    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select number of Settled Accounts',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


	--#17478
	select 
		distinct AcctNo, OutStBal 
	into 
		#temp_AvgStatusBals
	from 
		#temp_AvgStatus
	where 
		OutStBal > 0
	and currstatus != 'S'


    SELECT @poCurNumAcc     = ISNULL(COUNT(*),0),
           @poCurHiNow      = ISNULL(MAX(CurrStatus),0),
           @poCurHiEver     = ISNULL(MAX(StatusCode),0)--,
           --@poTotalOutStBal = ISNULL(SUM(OutStBal),0)
    FROM   #temp_AvgStatus
    WHERE  CurrStatus <> 'S'  

	--#17478
	SELECT @poTotalOutStBal = ISNULL(SUM(OutStBal),0)
	FROM #temp_AvgStatusBals

    -- Highest Status and Size Code of largest Settled Agreement Total
    IF  @poSetNumAcc > 0
    BEGIN
    
        SELECT @tmpAgrmtTotal    = t.AgrmtTotal,
               @poSetLargestSize = ags.SizeCode,
               @poSetLargest     = ISNULL(MAX(t.StatusCode),0)
        FROM   #temp_AvgStatus t, AgreementSize ags
        WHERE  t.CurrStatus = 'S'
        AND    t.AgrmtTotal = (SELECT MAX(AgrmtTotal)
                               FROM   #temp_AvgStatus
                               WHERE  CurrStatus = 'S')
        AND    ags.MaxValue = (SELECT MIN(MaxValue)
                               FROM   AgreementSize
                               WHERE  MaxValue >= t.AgrmtTotal)
        GROUP BY t.AgrmtTotal, ags.SizeCode

        SET @SQLError = @@ERROR
        IF (@SQLError <> 0)
        BEGIN
            EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                                   @piMsg        = 'Select largest Size Code',
                                   @piObjectName = @CurObjectName
            RETURN @SQLError
        END
    END    
    ELSE    -- No settled accounts
        IF @poCurNumAcc > 0  --existing customer no settled
            SET @poSetLargestSize= '0'
        ELSE -- New customer 
            SET @poSetLargestSize= '1'

    
    -- Check for a new customer 
    IF @AvgRowcount = 0 AND @HistRowcount = 0
    BEGIN
        -- Default values for a new customer
        SET @poCurNumAcc      = 0
        SET @poSetNumAcc      = 0
        SET @poCurRecent      = 'N'
        SET @poSetRecent      = 'N'
        SET @poCurHiEver      = 'N'
        SET @poSetHiEver      = 'N'
        SET @poCurHiNow       = 'N'
        SET @poSetHiNow       = 'N'
        SET @poCurWeightAvg   = 0.0
        SET @poSetWeightAvg   = 0.0
        SET @poSetLargest     = 'N'
        SET @poTotalInstal    = 0
        SET @poTotalOutStBal  = 0
        RETURN 0
    END
    -- Number of Current Accounts, highest status now, highest status ever
    -- and total of Outstanding Balances
    
    SELECT
           @poCurHiNow      = ISNULL(MAX(CurrStatus),0),
           @poCurHiEver     = ISNULL(MAX(statuscode),0)--,
           --@poTotalOutStBal = ISNULL(SUM(OutStBal),0)						--#17478
    FROM   #temp_AvgStatus
    WHERE  CurrStatus <> 'S'

	--#17478
	SELECT @poTotalOutStBal = ISNULL(SUM(OutStBal),0)
	FROM #temp_AvgStatusBals

    SELECT @poCurNumAcc     = ISNULL(COUNT(DISTINCT(acctno)),0)
           
    FROM   #temp_AvgStatus
    WHERE  CurrStatus <> 'S'



    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select number of Current Accounts',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END

    
    -- Most recent Current Account Status

    SELECT @poCurRecent = CurrStatus
    FROM   #temp_AvgStatus
    WHERE  CurrStatus <> 'S'
    AND    DateStatChge = (SELECT MAX(DateStatChge)
                           FROM   #temp_AvgStatus b
                           WHERE  b.CurrStatus <> 'S')

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select recent Current Account',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


 
    
    -- Most recent penultimate Settled Account Status

    SELECT @poSetRecent = StatusCode
    FROM   #temp_AvgStatus
    WHERE  CurrStatus = 'S' -- surely !=
    AND    DateStatChge = (SELECT MAX(DateStatChge)
                           FROM   #temp_AvgStatus b
                           WHERE  b.CurrStatus = 'S')
                           
    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select recent Settled Account',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END





    -- Total of Current instalments

    SELECT @poTotalInstal = ISNULL(SUM(ip.InstalAmount),0)
    FROM   #temp_AvgStatus t, InstalPlan ip
    WHERE  t.CurrStatus <> 'S'
    AND    ip.AcctNo = t.AcctNo

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select total current instalments',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END
    
    ----------------------------------------------------------------------------


    RETURN 0
    
END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


