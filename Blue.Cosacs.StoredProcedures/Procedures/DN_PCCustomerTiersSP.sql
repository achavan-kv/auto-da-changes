
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_PCCustomerTiersSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_PCCustomerTiersSP
END
GO


CREATE PROCEDURE DN_PCCustomerTiersSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_PCCustomerTiersSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Define the customers that qualify for Privilege Club Tier1/2
-- Author       : D Richardson
-- Date         : 8 Feb 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @return                             INTEGER OUTPUT

AS  DECLARE
    -- Local variables
    @DateToday                          DATETIME,
    @MinClubLen                         INTEGER,
    @ClubAverageStatus                  FLOAT,
    @CreditExpiryMonths                 INTEGER,
    @Tier1CashLen                       INTEGER,
    @Tier1CashSpend                     INTEGER,
    @Tier1CashMaintainLen               INTEGER,
    @Tier1CashMaintainSpend             INTEGER,
    @Tier1CreditLen                     INTEGER,
    @Tier2CashLen                       INTEGER,
    @Tier2CashSpend                     INTEGER,
    @Tier2CashMaintainLen               INTEGER,
    @Tier2CashMaintainSpend             INTEGER,
    @Tier2CreditLen                     INTEGER,
    @Tier1CashPeriodStart               DATETIME,
    @Tier1CashMaintainPeriodStart       DATETIME,
    @Tier1CreditPeriodStart             DATETIME,
    @Tier2CashPeriodStart               DATETIME,
    @Tier2CashMaintainPeriodStart       DATETIME,
    @Tier2CreditPeriodStart             DATETIME,
    @WarningAverageStatus               FLOAT,
    @WarningLeadDays                    INTEGER

BEGIN
    SET @return = 0
    SET @DateToday = GETDATE()
    
    -------------------------------------------------------------------------
    -- Load country parameters
    -------------------------------------------------------------------------
    -- PRINT 'Load country params (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    SELECT @MinClubLen = Value FROM CountryMaintenance WHERE CodeName = 'MinClubLen'
    SELECT @ClubAverageStatus = Value FROM CountryMaintenance WHERE CodeName = 'ClubAverageStatus'
    SELECT @CreditExpiryMonths = Value FROM CountryMaintenance WHERE CodeName = 'CreditExpiryMonths'
    SELECT @Tier1CashLen = Value FROM CountryMaintenance WHERE CodeName = 'Tier1CashLen'
    SELECT @Tier1CashSpend = Value FROM CountryMaintenance WHERE CodeName = 'Tier1CashSpend'
    SELECT @Tier1CashMaintainLen = Value FROM CountryMaintenance WHERE CodeName = 'Tier1CashMaintainLen'
    SELECT @Tier1CashMaintainSpend = Value FROM CountryMaintenance WHERE CodeName = 'Tier1CashMaintainSpend'
    SELECT @Tier1CreditLen = Value FROM CountryMaintenance WHERE CodeName = 'Tier1CreditLen'
    SELECT @Tier2CashLen = Value FROM CountryMaintenance WHERE CodeName = 'Tier2CashLen'
    SELECT @Tier2CashSpend = Value FROM CountryMaintenance WHERE CodeName = 'Tier2CashSpend'
    SELECT @Tier2CashMaintainLen = Value FROM CountryMaintenance WHERE CodeName = 'Tier2CashMaintainLen'
    SELECT @Tier2CashMaintainSpend = Value FROM CountryMaintenance WHERE CodeName = 'Tier2CashMaintainSpend'
    SELECT @Tier2CreditLen = Value FROM CountryMaintenance WHERE CodeName = 'Tier2CreditLen'
    SELECT @WarningAverageStatus = Value FROM CountryMaintenance WHERE CodeName = 'WarningAverageStatus'
    SELECT @WarningLeadDays = Value FROM CountryMaintenance WHERE CodeName = 'WarningLeadDays'
    SET @Tier1CashPeriodStart = DATEADD(Month, -@Tier1CashLen, @DateToday)
    SET @Tier1CashMaintainPeriodStart = DATEADD(Month, -@Tier1CashMaintainLen, @DateToday)
    SET @Tier1CreditPeriodStart = DATEADD(Month, -@Tier1CreditLen, @DateToday)
    SET @Tier2CashPeriodStart = DATEADD(Month, -@Tier2CashLen, @DateToday)
    SET @Tier2CashMaintainPeriodStart = DATEADD(Month, -@Tier2CashMaintainLen, @DateToday)
    SET @Tier2CreditPeriodStart = DATEADD(Month, -@Tier2CreditLen, @DateToday)


    -------------------------------------------------------------------------
    -- Calculate the days in each status code
    -------------------------------------------------------------------------
    -- This is a big table so to minimise the size of the update this should
    -- only be done for rows not previously calculated and for rows that were
    -- marked as the current status the last time this update was run.

    UPDATE  Status
    SET     CurrentStatus = 'N',
            DaysInStatus = DATEDIFF(Day, Status.DateStatChge, (SELECT MIN(s2.DateStatChge)
                                                               FROM   Status s2
                                                               WHERE  s2.AcctNo = Status.AcctNo
                                                               AND    s2.DateStatChge > Status.DateStatChge))
    WHERE   DaysInStatus IS NULL
    OR      CurrentStatus = 'Y'


    -- Rows that do not have a later row will not have been calculated,
    -- so mark these rows as the current rows and calculate from today's date
    UPDATE Status
    SET    CurrentStatus = 'Y',
           DaysInStatus = DATEDIFF(Day, DateStatChge, @DateToday)
    WHERE  DaysInStatus IS NULL



    -------------------------------------------------------------------------
    -- Load Accounts
    -------------------------------------------------------------------------
    --
    -- Find qualifying CASH or CREDIT accounts for each customer
    --
    -- PRINT 'Find qualifying CASH or CREDIT accounts for each customer (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    IF EXISTS (SELECT Table_Name FROM INFORMATION_SCHEMA.Tables
               WHERE  Table_Name = 'tmpPCAccount')
    BEGIN
        DROP TABLE tmpPCAccount
    END

    SELECT a.AcctNo, a.AcctType, a.CurrStatus, a.HighstStatus, ca.CustId,
           ISNULL(a.DateLastPaid,'') AS DateLastPaid,
           CONVERT(DATETIME,'') AS DateSettled,
           CONVERT(DATETIME,'') AS Tier1FirstStatusDate,
           CONVERT(DATETIME,'') AS Tier2FirstStatusDate,
           CONVERT(FLOAT,0) AS Tier1AvgStatus,
           CONVERT(FLOAT,0) AS Tier2AvgStatus,
           CONVERT(INTEGER,0) AS Tier1SettledInstalmentsPaid,
           CONVERT(INTEGER,0) AS Tier2SettledInstalmentsPaid,
           CONVERT(INTEGER,0) AS CurrentInstalmentsPaid
    INTO   tmpPCAccount
    FROM   Acct a, CustAcct ca
    WHERE  a.AcctType IN ('C','O','R')
    AND    ca.AcctNo = a.AcctNo
    AND    ca.HldOrJnt = 'H'

    CREATE INDEX ix_tmpPCAccount_CustId_AcctNo ON tmpPCAccount (CustId, AcctNo)


    -------------------------------------------------------------------------
    -- PER ACCOUNT
    -------------------------------------------------------------------------

    -- Ignore all accounts for a customer with any account above status code '2'
    DELETE FROM tmpPCAccount
    WHERE  EXISTS (SELECT 1 FROM tmpPCAccount t
                   WHERE  t.CustId = tmpPCAccount.Custid
                   AND    t.CurrStatus NOT IN ('S','U','0','1','2'))


    -- Date settled for settled accounts
    UPDATE tmpPCAccount
    SET    DateSettled = ISNULL((SELECT MAX(DateStatChge)
                                 FROM   Status s
                                 WHERE  s.AcctNo = tmpPCAccount.AcctNo
                                 AND    s.StatusCode = 'S'),'')
    WHERE  CurrStatus = 'S'

    -- Ignore CASH accounts settled before the CASH period.
    -- The status and spend for CASH accounts is only calculated in the
    -- CASH period immediately prior to today.
    -- PRINT 'Ignore CASH accounts settled before the CASH period (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    DELETE FROM tmpPCAccount
    WHERE  AcctType = 'C'
    AND    CurrStatus = 'S'
    AND    DateSettled < DATEADD(Month,-@Tier2CashLen,@DateToday)


    -- Ignore CREDIT accounts settled before the CreditExpiryMonths period.
    -- Old qualifying accounts expire after the CreditExpiryMonths period.
    -- The status and instalments for CURRENT CREDIT accounts is calculated
    -- in the CREDIT period immediately prior to today, but for SETTLED CREDIT
    -- accounts it is the same duration prior to each settle date.
    -- PRINT 'Ignore CREDIT accounts settled before the CreditExpiryMonths period (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    DELETE FROM tmpPCAccount
    WHERE  AcctType IN ('O','R')
    AND    CurrStatus = 'S'
    AND    DateSettled < DATEADD(Month,-@CreditExpiryMonths,@DateToday)


    -- The average status of each account is calculated within a date range
    -- for either the CASH or CREDIT period. In order to know the status at
    -- the start of a period the last status change before the period started
    -- is required.
    -- Status period for each CASH Account over last @Tier1CashLen months
    -- PRINT 'Status period for each CASH Account over last @Tier1CashLen months (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier1FirstStatusDate =
               ISNULL((SELECT MAX(s.DateStatChge)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge <= @Tier1CashPeriodStart),'')
    WHERE  tmpPCAccount.AcctType = 'C'

    -- Status period for each CASH Account over last @Tier2CashLen months
    -- PRINT 'Status period for each CASH Account over last @Tier2CashLen months (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier2FirstStatusDate =
               ISNULL((SELECT MAX(s.DateStatChge)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge <= @Tier2CashPeriodStart),'')
    WHERE  tmpPCAccount.AcctType = 'C'

    -- Status period for each CURRENT CREDIT Account over last @Tier1CreditLen months
    -- PRINT 'Status period for each CURRENT CREDIT Account over last @Tier1CreditLen months (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier1FirstStatusDate =
               ISNULL((SELECT MAX(s.DateStatChge)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge <= @Tier1CreditPeriodStart),'')
    WHERE  tmpPCAccount.AcctType IN ('O','R')
    AND    tmpPCAccount.CurrStatus != 'S'

    -- Status period for each CURRENT CREDIT Account over last @Tier2CreditLen months
    -- PRINT 'Status period for each CURRENT CREDIT Account over last @Tier2CreditLen months (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier2FirstStatusDate =
               ISNULL((SELECT MAX(s.DateStatChge)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge <= @Tier2CreditPeriodStart),'')
    WHERE  tmpPCAccount.AcctType IN ('O','R')
    AND    tmpPCAccount.CurrStatus != 'S'

    -- Status period for each SETTLED CREDIT Account over last @Tier1CreditLen months
    -- PRINT 'Status period for each SETTLED CREDIT Account over last @Tier1CreditLen months (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier1FirstStatusDate =
               ISNULL((SELECT MAX(s.DateStatChge)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge <= DATEADD(Month,-@Tier1CreditLen,tmpPCAccount.DateSettled)),'')
    WHERE  tmpPCAccount.AcctType IN ('O','R')
    AND    tmpPCAccount.CurrStatus = 'S'

    -- Status period for each SETTLED CREDIT Account over last @Tier2CreditLen months
    -- PRINT 'Status period for each SETTLED CREDIT Account over last @Tier2CreditLen months (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier2FirstStatusDate =
               ISNULL((SELECT MAX(s.DateStatChge)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge <= DATEADD(Month,-@Tier2CreditLen,tmpPCAccount.DateSettled)),'')
    WHERE  tmpPCAccount.AcctType IN ('O','R')
    AND    tmpPCAccount.CurrStatus = 'S'


    -- Average Status for each Account for Tier1
    -- This includes settled credit accounts, but we can still use today's date for the end of
    -- the period because there won't be any more status records after their settle date.
    -- PRINT 'Average Status for each Account for Tier1 (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier1AvgStatus =
               ISNULL((SELECT ROUND(SUM(CONVERT(FLOAT,s.StatusCode)*s.DaysInStatus) / SUM(s.DaysInStatus), 2)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge BETWEEN Tier1FirstStatusDate AND @DateToday
                       HAVING SUM(s.DaysInStatus) > 0),0)

    -- Average Status for each Account for Tier2
    -- This includes settled credit accounts, but we can still use today's date for the end of
    -- the period because there won't be any more status records after their settle date.
    -- PRINT 'Average Status for each Account for Tier2 (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier2AvgStatus =
               ISNULL((SELECT ROUND(SUM(CONVERT(FLOAT,s.StatusCode)*s.DaysInStatus) / SUM(s.DaysInStatus), 2)
                       FROM   Status s
                       WHERE  s.AcctNo = tmpPCAccount.AcctNo
                       AND    s.StatusCode IN ('1','2','3','4','5','6','7','8')
                       AND    s.DateStatChge BETWEEN Tier2FirstStatusDate AND @DateToday
                       HAVING SUM(s.DaysInStatus) > 0),0)

    -- Count the instalments per account for current CREDIT accounts
    -- PRINT 'Count the instalments per account for current CREDIT accounts (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    CurrentInstalmentsPaid =
               ISNULL((SELECT ROUND(SUM(-f.TransValue / ip.InstalAmount),0,1)
                       FROM   Instalplan ip, FinTrans f
                       WHERE  ip.AcctNo = tmpPCAccount.AcctNo
                       AND    ip.InstalAmount >= 0.01
                       AND    f.AcctNo = tmpPCAccount.AcctNo
                       AND    f.TransTypeCode IN ('PAY','DDN','DDR','DDE')),0)
    WHERE  AcctType IN ('O','R')
    AND    CurrStatus IN ('U','0','1','2')

    -- Count the instalments per account for settled CREDIT accounts.
    -- This will only include agreement terms with at least @Tier1CreditLen instalments
    -- but could have settled early with fewer actual instalments.
    -- PRINT 'Count the instalments per account for settled CREDIT accounts (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier1SettledInstalmentsPaid =
               ISNULL((SELECT ROUND(SUM(-f.TransValue / ip.InstalAmount),0,1)
                       FROM   Instalplan ip, FinTrans f
                       WHERE  ip.AcctNo = tmpPCAccount.AcctNo
                       AND    ip.InstalAmount >= 0.01
                       AND    ip.InstalNo >= @Tier1CreditLen
                       AND    DATEDIFF(Month, ip.DateFirst, tmpPCAccount.DateSettled) <= ip.InstalNo
                       AND    f.AcctNo = tmpPCAccount.AcctNo
                       AND    f.TransTypeCode IN ('PAY','DDN','DDR','DDE')),0)
    WHERE  AcctType IN ('O','R')
    AND    CurrStatus = 'S'

    -- Count the instalments per account for settled CREDIT accounts.
    -- This will only include agreement terms with at least @Tier2CreditLen instalments
    -- but could have settled early with fewer actual instalments.
    -- PRINT 'Count the instalments per account for settled CREDIT accounts (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE tmpPCAccount
    SET    Tier2SettledInstalmentsPaid =
               ISNULL((SELECT ROUND(SUM(-f.TransValue / ip.InstalAmount),0,1)
                       FROM   Instalplan ip, FinTrans f
                       WHERE  ip.AcctNo = tmpPCAccount.AcctNo
                       AND    ip.InstalAmount >= 0.01
                       AND    ip.InstalNo >= @Tier2CreditLen
                       AND    DATEDIFF(Month, ip.DateFirst, tmpPCAccount.DateSettled) <= ip.InstalNo
                       AND    f.AcctNo = tmpPCAccount.AcctNo
                       AND    f.TransTypeCode IN ('PAY','DDN','DDR','DDE')),0)
    WHERE  AcctType IN ('O','R')
    AND    CurrStatus = 'S'



    -------------------------------------------------------------------------
    -- PER CUSTOMER
    -------------------------------------------------------------------------
    --
    -- Only need the worst / highest values per customer
    --
    -- PRINT 'Only need the worst / highest values per customer (' + CONVERT(VARCHAR,GETDATE(),113) + ')'

    UPDATE tmpPCAccount SET HighstStatus = '0' WHERE ISNUMERIC(HighstStatus) = 0

    TRUNCATE TABLE PCCustomerTiers

    INSERT INTO PCCustomerTiers
        (CustId,
         AcctNo,
         AcctType,
         LetterCode,
         OldPCMember,
         CurrentPC,
         NewPC,
         HighstStatus,
         Tier1AvgStatus,
         Tier2AvgStatus,
         CurrentInstalmentsPaid,
         Tier1SettledInstalmentsPaid,
         Tier2SettledInstalmentsPaid,
         CurrentInstalmentsPaidAcctNo,
         Tier1SettledInstalmentsPaidAcctNo,
         Tier2SettledInstalmentsPaidAcctNo,
         DateLastPaid,
         DateLastDelivery,
         Tier1CashSpend,
         Tier2CashSpend,
         Tier1CashMaintainSpend,
         Tier2CashMaintainSpend,
         CashSpendAcctNo)
    SELECT t.CustId,
           '',
           '',
           '',
           'N',
           '',
           '',
           MAX(t.HighstStatus),
           AVG(t.Tier1AvgStatus),
           AVG(t.Tier2AvgStatus),
           MAX(t.CurrentInstalmentsPaid),
           MAX(t.Tier1SettledInstalmentsPaid),
           MAX(t.Tier2SettledInstalmentsPaid),
           '',
           '',
           '',
           MAX(t.DateLastPaid),
           '',
           0,
           0,
           0,
           0,
           ''
    FROM   tmpPCAccount t
    GROUP BY t.CustId


    -- Pick account numbers that might qualify
    UPDATE PCCustomerTiers
    SET    CurrentInstalmentsPaidAcctNo =
               ISNULL((SELECT MAX(AcctNo) FROM tmpPCAccount ta
                       WHERE  ta.Custid = PCCustomerTiers.CustId
                       AND    ta.AcctType IN ('O','R')
                       AND    ta.CurrentInstalmentsPaid = PCCustomerTiers.CurrentInstalmentsPaid),'')

    UPDATE PCCustomerTiers
    SET    Tier1SettledInstalmentsPaidAcctNo =
               ISNULL((SELECT MAX(AcctNo) FROM tmpPCAccount ta
                       WHERE  ta.Custid = PCCustomerTiers.CustId
                       AND    ta.AcctType IN ('O','R')
                       AND    ta.Tier1SettledInstalmentsPaid = PCCustomerTiers.Tier1SettledInstalmentsPaid),'')

    UPDATE PCCustomerTiers
    SET    Tier2SettledInstalmentsPaidAcctNo =
               ISNULL((SELECT MAX(AcctNo) FROM tmpPCAccount ta
                       WHERE  ta.Custid = PCCustomerTiers.CustId
                       AND    ta.AcctType IN ('O','R')
                       AND    ta.Tier2SettledInstalmentsPaid = PCCustomerTiers.Tier2SettledInstalmentsPaid),'')

    UPDATE PCCustomerTiers
    SET    CashSpendAcctNo =
               ISNULL((SELECT MAX(AcctNo) FROM tmpPCAccount ta
                       WHERE  ta.Custid = PCCustomerTiers.CustId
                       AND    ta.AcctType = 'C'),'')


    -- Latest delivery date per customer
    -- PRINT 'Latest delivery date per customer (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE PCCustomerTiers
    SET    DateLastDelivery =
               ISNULL((SELECT MAX(f.DateTRans)
                       FROM   tmpPCAccount t, FinTrans f
                       WHERE  t.CustId = PCCustomerTiers.CustId
                       AND    f.AcctNo = t.AcctNo
                       AND    f.TransTypeCode = 'DEL'),'')


    -- Sum the spend per customer for CASH accounts agreed during the Tier1 CASH period
    -- PRINT 'Sum the spend per customer for CASH accounts agreed during the Tier1 CASH period (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE PCCustomerTiers
    SET    Tier1CashSpend =
               ISNULL((SELECT ROUND(SUM(ag.AgrmtTotal),2,1)
                       FROM   tmpPCAccount t, Agreement ag
                       WHERE  t.CustId = PCCustomerTiers.CustId
                       AND    t.AcctType = 'C'
                       AND    ag.AcctNo = t.AcctNo
                       AND    ag.DateAgrmt BETWEEN @Tier1CashPeriodStart AND @DateToday),0)

    -- Sum the spend per customer for CASH accounts agreed during the Tier2 CASH period
    -- PRINT 'Sum the spend per customer for CASH accounts agreed during the Tier2 CASH period (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE PCCustomerTiers
    SET    Tier2CashSpend =
               ISNULL((SELECT ROUND(SUM(ag.AgrmtTotal),2,1)
                       FROM   tmpPCAccount t, Agreement ag
                       WHERE  t.CustId = PCCustomerTiers.CustId
                       AND    t.AcctType = 'C'
                       AND    ag.AcctNo = t.AcctNo
                       AND    ag.DateAgrmt BETWEEN @Tier2CashPeriodStart AND @DateToday),0)

    UPDATE PCCustomerTiers
    SET    Tier1CashMaintainSpend =
               ISNULL((SELECT ROUND(SUM(ag.AgrmtTotal),2,1)
                       FROM   tmpPCAccount t, Agreement ag
                       WHERE  t.CustId = PCCustomerTiers.CustId
                       AND    t.AcctType = 'C'
                       AND    ag.AcctNo = t.AcctNo
                       AND    ag.DateAgrmt BETWEEN @Tier1CashMaintainPeriodStart AND @DateToday),0)

    -- Sum the spend per customer for CASH accounts agreed during the Tier2 CASH period
    -- PRINT 'Sum the spend per customer for CASH accounts agreed during the Tier2 CASH period (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    UPDATE PCCustomerTiers
    SET    Tier2CashMaintainSpend =
               ISNULL((SELECT ROUND(SUM(ag.AgrmtTotal),2,1)
                       FROM   tmpPCAccount t, Agreement ag
                       WHERE  t.CustId = PCCustomerTiers.CustId
                       AND    t.AcctType = 'C'
                       AND    ag.AcctNo = t.AcctNo
                       AND    ag.DateAgrmt BETWEEN @Tier2CashMaintainPeriodStart AND @DateToday),0)



    -------------------------------------------------------------------------
    -- Add rows for customers who did not qualify but may need to be demoted
    -------------------------------------------------------------------------
    -- PRINT 'Add rows for customers who did not qualify but may need to be demoted ... (' + CONVERT(VARCHAR,GETDATE(),113) + ')'

    INSERT INTO PCCustomerTiers
        (CustId,
         AcctNo,
         AcctType,
         LetterCode,
         OldPCMember,
         CurrentPC,
         NewPC,
         HighstStatus,
         Tier1AvgStatus,
         Tier2AvgStatus,
         CurrentInstalmentsPaid,
         Tier1SettledInstalmentsPaid,
         Tier2SettledInstalmentsPaid,
         CurrentInstalmentsPaidAcctNo,
         Tier1SettledInstalmentsPaidAcctNo,
         Tier2SettledInstalmentsPaidAcctNo,
         DateLastPaid,
         DateLastDelivery,
         Tier1CashSpend,
         Tier2CashSpend,
         Tier1CashMaintainSpend,
         Tier2CashMaintainSpend,
         CashSpendAcctNo)
    SELECT c.CustId,
           '',
           '',
           '',
           'N',
           '',
           '',
           0,
           0,
           0,
           0,
           0,
           0,
           '',
           '',
           '',
           '',
           '',
           0,
           0,
           0,
           0,
           ''
    FROM   Customer c
    WHERE  c.CustId NOT IN (SELECT CustId FROM PCCustomerTiers)


    -------------------------------------------------------------------------
    -- Final report updates
    -------------------------------------------------------------------------
    -- PRINT 'Final report updates ... (' + CONVERT(VARCHAR,GETDATE(),113) + ')'

    -- Find out who is in the old Privilege Club
    UPDATE PCCustomerTiers
    SET    OldPCMember = 'Y'
    WHERE  EXISTS (SELECT 1 FROM CustCatCode cc
                   WHERE  cc.CustId = PCCustomerTiers.CustId
                   AND    cc.Code = 'CLAC'
                   AND    ISNULL(cc.DateDeleted,'') = ''
                   AND NOT EXISTS (SELECT 1 FROM CustCatCode cc2
                                   WHERE  cc2.CustId = PCCustomerTiers.CustId
                                   AND    cc2.Code IN ('CLAS', 'CLAW')
                                   AND    ISNULL(cc2.DateDeleted,'') = ''))
    OR     EXISTS (SELECT 1 FROM OldPCMember opcm
                   WHERE  opcm.CustId = PCCustomerTiers.CustId)

    -- Find out who is already in Tier1
    UPDATE PCCustomerTiers
    SET    CurrentPC = 'TIR1'
    WHERE  EXISTS (SELECT 1 FROM CustCatCode
                   WHERE CustId = PCCustomerTiers.CustId
                   AND   Code = 'TIR1'
                   AND   ISNULL(DateDeleted,'') = '')

    -- Find out who is already in Tier2
    UPDATE PCCustomerTiers
    SET    CurrentPC = 'TIR2'
    WHERE  EXISTS (SELECT 1 FROM CustCatCode
                   WHERE CustId = PCCustomerTiers.CustId
                   AND   Code = 'TIR2'
                   AND   ISNULL(DateDeleted,'') = '')

    --
    -- Mark where qualified for Tier1.
    -- Note the order of these queries can overwrite a qualifying
    -- account number with a subsequent qualifying account number.
    -- So a customer will only be qualified by a cash account if there
    -- is no qualifying credit account. A settled credit account will only
    -- be used to qualify if there is no qualifying current credit account.
    --
    -- Qualified Tier1 on CASH accounts
    UPDATE PCCustomerTiers
    SET    NewPC = 'TIR1',
           AcctNo = CashSpendAcctNo
    WHERE  Tier1AvgStatus <= @ClubAverageStatus
    AND    (  (Tier1CashSpend > @Tier1CashSpend AND Tier1CashMaintainSpend > @Tier1CashMaintainSpend)
           OR (CurrentPC = 'TIR1' AND Tier1CashMaintainSpend > @Tier1CashMaintainSpend))

    -- Qualified Tier1 on CREDIT accounts
    UPDATE PCCustomerTiers
    SET    NewPC = 'TIR1',
           AcctNo = Tier1SettledInstalmentsPaidAcctNo
    WHERE  Tier1AvgStatus <= @ClubAverageStatus
    AND    Tier1SettledInstalmentsPaid >= @MinClubLen

    UPDATE PCCustomerTiers
    SET    NewPC = 'TIR1',
           AcctNo = CurrentInstalmentsPaidAcctNo
    WHERE  Tier1AvgStatus <= @ClubAverageStatus
    AND    CurrentInstalmentsPaid >= @Tier1CreditLen


    --
    -- Mark where qualified for Tier2.
    -- This also checks that TIER1 would have qualified.
    -- Note the order of these queries can overwrite a qualifying
    -- account number with a subsequent qualifying account number.
    -- So a customer will only be qualified by a cash account if there
    -- is no qualifying credit account. A settled credit account will only
    -- be used to qualify if there is no qualifying current credit account.
    --
    -- Qualified Tier2 on CASH accounts
    UPDATE PCCustomerTiers
    SET    NewPC = 'TIR2',
           AcctNo = CashSpendAcctNo
    WHERE  NewPC = 'TIR1'
    AND    Tier2AvgStatus <= @ClubAverageStatus
    AND    (  (Tier2CashSpend > @Tier2CashSpend AND Tier2CashMaintainSpend > @Tier2CashMaintainSpend)
           OR (CurrentPC = 'TIR2' AND Tier2CashMaintainSpend > @Tier2CashMaintainSpend))

    -- Qualified Tier2 on CREDIT accounts
    UPDATE PCCustomerTiers
    SET    NewPC = 'TIR2',
           AcctNo = CurrentInstalmentsPaidAcctNo
    WHERE  NewPC = 'TIR1'
    AND    Tier2AvgStatus <= @ClubAverageStatus
    AND    CurrentInstalmentsPaid >= @Tier2CreditLen

    UPDATE PCCustomerTiers
    SET    NewPC = 'TIR2',
           AcctNo = Tier2SettledInstalmentsPaidAcctNo
    WHERE  NewPC = 'TIR1'
    AND    Tier2AvgStatus <= @ClubAverageStatus
    AND    Tier2SettledInstalmentsPaid >= @MinClubLen

    -- Copy the account type here as well
    UPDATE PCCustomerTiers
    SET    AcctType = ta.AcctType
    FROM   tmpPCAccount ta
    WHERE  ta.Custid = PCCustomerTiers.Custid
    AND    ta.AcctNo = PCCustomerTiers.AcctNo

    -- Set the letter codes for TIER1
    -- Note the order of these queries can overwrite a letter
    -- code with a subsequent letter code. So an HP settled letter
    -- will only be sent if there is no HP live / welcome back letter,
    -- and an HP live letter will only be sent if there is no HP
    -- welcome back letter.
    --
    -- Cash letter TIER1
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER1C'
    WHERE  AcctType = 'C' AND NewPC = 'TIR1'

    -- HP settled letter TIER1
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER1HPS'
    WHERE  AcctType = 'O' AND NewPC = 'TIR1'
    AND    Tier1SettledInstalmentsPaid >= @MinClubLen

    -- HP live letter TIER1
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER1HPL'
    WHERE  AcctType = 'O' AND NewPC = 'TIR1'
    AND    CurrentInstalmentsPaid >= @Tier1CreditLen

    -- HP welcome back letter TIER1
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER1HPO'
    WHERE  AcctType = 'O' AND NewPC = 'TIR1'
    AND EXISTS (SELECT 1 FROM CustCatCode cc
                WHERE  cc.CustId = PCCustomerTiers.CustId
                AND    cc.Code IN ('TIR1','TIR2'))

    -- RF letter TIER1
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER1RF'
    WHERE  AcctType = 'R' AND NewPC = 'TIR1'


    -- Set the letter codes for TIER2
    -- Note the order of these queries can overwrite a letter
    -- code with a subsequent letter code. So an HP settled letter
    -- will only be sent if there is no HP live / welcome back letter,
    -- and an HP live letter will only be sent if there is no HP
    -- welcome back letter.
    --
    -- Cash letter TIER2
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER2C'
    WHERE  AcctType = 'C' AND NewPC = 'TIR2'

    -- HP settled letter TIER2
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER2HPS'
    WHERE  AcctType = 'O' AND NewPC = 'TIR2'
    AND    Tier2SettledInstalmentsPaid >= @MinClubLen

    -- HP live letter TIER2
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER2HPL'
    WHERE  AcctType = 'O' AND NewPC = 'TIR2'
    AND    CurrentInstalmentsPaid >= @Tier2CreditLen

    -- HP welcome back letter TIER2
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER2HPO'
    WHERE  AcctType = 'O' AND NewPC = 'TIR2'
    AND EXISTS (SELECT 1 FROM CustCatCode cc
                WHERE  cc.CustId = PCCustomerTiers.CustId
                AND    cc.Code = 'TIR2')

    -- RF letter TIER2
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIER2RF'
    WHERE  AcctType = 'R' AND NewPC = 'TIR2'


    --
    -- Set the letter codes for Warning Letters
    --
    -- Mark up each TIER1 customer with a current warning 
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIERWARN'
    FROM   CustCatCode cc
    WHERE  PCCustomerTiers.LetterCode = ''
    AND    PCCustomerTiers.NewPC = 'TIER1'
    AND    PCCustomerTiers.Tier1AvgStatus > @WarningAverageStatus
    AND    cc.CustId = PCCustomerTiers.CustId
    AND    cc.Code = 'TIR1'
    AND    ISNULL(cc.DateDeleted,'') = ''
    AND    DATEDIFF(Day, cc.DateCoded, @DateToday) > @WarningLeadDays

    -- Mark up each TIER2 customer with a current warning 
    UPDATE PCCustomerTiers
    SET    LetterCode = 'TIERWARN'
    FROM   CustCatCode cc
    WHERE  PCCustomerTiers.LetterCode = ''
    AND    PCCustomerTiers.NewPC = 'TIER2'
    AND    PCCustomerTiers.Tier2AvgStatus > @WarningAverageStatus
    AND    cc.CustId = PCCustomerTiers.CustId
    AND    cc.Code = 'TIR2'
    AND    ISNULL(cc.DateDeleted,'') = ''
    AND    DATEDIFF(Day, cc.DateCoded, @DateToday) > @WarningLeadDays


    DROP TABLE tmpPCAccount

    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_PCCustomerTiersSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

