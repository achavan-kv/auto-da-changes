
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_RFCheckExpiredSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_RFCheckExpiredSP
END
GO


/****** Object:  StoredProcedure [dbo].[DN_RFCheckExpiredSP]    Script Date: 11/05/2007 12:20:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[DN_RFCheckExpiredSP]
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_RFCheckExpiredSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Converted OpenRoad code
-- Author       : Rupal Desai
-- Date         : 02 June 2006
--
-- Change Control
-- --------------
-- Date          By  Description
-- ----          --  -----------
-- 9 Oct 2006    DSR FR68546 A few corrections
--04 Jan 2007	 PC  68736 Added a check for credit limit in reminder letters
--------------------------------------------------------------------------------

    -- Parameters
    @empeeno int,
    @Return  INTEGER OUTPUT

AS

BEGIN

    SET     @return = 0            --initialise return code

    -- Local variables

    --************************************************************
    -- Send a reminder letter where zero balance over 10 months
    -- or reset credit limit where zero balance over 12 months
    --************************************************************

    DECLARE
        @AYearAgo         DATETIME,
        @TenMonthsAgo     DATETIME

    SET @AYearAgo        = DATEADD(Day, +15,(DATEADD(Year, -1,GETDATE())))  --  (-'1 Year' + '15 Days') -Actually 11.5 Months ago
    SET @TenMonthsAgo    = DATEADD(Month,-10,GETDATE())       -- '10 Months'

    -- Find customers with zero RF balance, no manual score in the last ten months
    -- and no deliveries on any of their RF accounts.

    -- Get the RF customer accounts not already expired

    SELECT  c.CustId,
            c.RFCreditLimit,
            c.RFDateReminded,
            a.AcctNo,
            a.OutStBal,
            CONVERT(DATETIME, '') AS DateManualScore,
            CONVERT(INTEGER, 0) AS HasDelivery
    INTO    #RFAcctList
    FROM    Customer c, CustAcct ca, Acct a
    WHERE   ca.CustId = c.CustId
    AND     ca.HldOrJnt = 'H'
    AND     a.AcctNo = ca.AcctNo
    AND     a.AcctType = 'R'
    AND     a.CurrStatus != 'S'
    AND		a.dateacctopen < DATEADD(MONTH,-10,GETDATE())
    AND     NOT EXISTS (SELECT * FROM CustCatCode cc
                        WHERE  cc.CustId = c.CustId
                        AND    cc.Code = 'REX'
                        AND    ISNULL(cc.DateDeleted,'') = '')

    -- Get the latest Stage One manual score date for each account
    UPDATE #RFAcctList
    SET    DateManualScore =
            (SELECT MAX(pf.DateCleared)
             FROM   Proposal p, ProposalFlag pf
             WHERE  p.AcctNo = #RFAcctList.AcctNo
             AND    pf.acctno = p.acctno)


    -- Consider Stage One flags not completed to have activity today
    UPDATE #RFAcctList
    SET    DateManualScore = GETDATE()
    WHERE  ISNULL(DateManualScore,'') = ''


    -- Flag each account that has ever had a delivery
    UPDATE #RFAcctList
    SET    HasDelivery = 1
    WHERE  EXISTS (SELECT * FROM FinTrans f
                   WHERE  f.AcctNo = #RFAcctList.AcctNo AND f.TransTypeCode = 'DEL')


    -- Get customers with zero RF balance, no manual score in the last ten months
    -- and no deliveries on any of their RF accounts.

    SELECT  CustId,
            ISNULL(RFCreditLimit,0) as RFCreditLimit,
            ISNULL(RFDateReminded,'') as RFDateReminded,
            MAX(AcctNo) as AcctNo,
            MAX(DateManualScore) as DateManualScore,
            CONVERT(VARCHAR(20), '') as Lettercode
    INTO    #RFLetter
    FROM    #RFAcctList
    GROUP BY CustId, RFCreditLimit, RFDateReminded
    HAVING ISNULL(SUM(OutStBal),0) = 0
    AND    MAX(DateManualScore) < DATEADD(MONTH,-10,GETDATE())
    AND    SUM(HasDelivery) = 0

    -- Update letter code to generate reminder letter for this Customer using one of the RF accounts
    
	UPDATE  #RFLetter
    SET     Lettercode = 'RFRE'
    WHERE   RFDateReminded < @TenMonthsAgo
	AND RFCreditLimit > 0 --68736 We don't want a to send a reminder to people who don't have any credit (haven't been scored etc)

    -- Update letter code to generate expiry letter for this Customer using one of the RF accounts
    -- This update must be after the reminder update so that expiry overrides a reminder
    UPDATE  #RFLetter
    SET     Lettercode = 'RFEX'
    WHERE   DateManualScore <  @AYearAgo

    DECLARE @runno smallint
	SELECT @runno =MAX(runno) FROM interfacecontrol WHERE interface = 'collections'
	-- just putting run number in to give an indication of when this was generated -- letters produced seperately anyway.... 

    -- Insert record in Letter table were record not exists to generate expiry  and reminder letters
    INSERT INTO LETTER
       (runno,
        AcctNo,
        DateAcctLttr,
        DateDue,
        LetterCode)
    SELECT  0,
            t.AcctNo,
            GETDATE(),
            GETDATE(),
            t.Lettercode
    FROM    #RFLetter t
    WHERE t.LetterCode IN ('RFRE','RFEX')
    AND NOT EXISTS (SELECT * FROM Letter l
                    WHERE  l.acctno = t.acctno
                    AND    l.LetterCode = t.LetterCode)


    -- Update the customer date reminded
    UPDATE  Customer
    SET     RFDateReminded = GETDATE()
    WHERE EXISTS (SELECT * FROM #RFLetter t
                  WHERE  t.CustId = Customer.CustId
                  AND    t.LetterCode = 'RFRE')


    --**************************************************************
    --* Cancel any open RF accounts for a customer with expired RF *
    --**************************************************************

    --  Add the 'REX' Customer Code for expired RF customer
    INSERT INTO CustCatCode
        (OrigBr, CustId, DateCoded, Code)
    SELECT  0, CustId, GETDATE(), 'REX'
    FROM    #RFLetter
    WHERE   #RFLetter.Lettercode = 'RFEX'

    -- * Settle Expired RF Account *
    SELECT  a.AcctNo, a.AgrmtTotal, a.CurrStatus
    INTO    #RFSettle
    FROM    #RFLetter t, CustAcct ca, Acct a
    WHERE   t.Lettercode  = 'RFEX'
    AND     ca.CustId     = t.CustId
    AND     ca.HldOrJnt   = 'H'
    AND     a.AcctNo      = ca.AcctNo
    AND		t.Acctno	  = a.AcctNo	
    AND		t.Acctno	  = ca.AcctNo	
    AND     a.AcctType    = 'R'
    AND     a.OutStBal    = 0
    AND     a.CurrStatus != 'S'

    -- Update highest status in acct table if highest status is lower then the
    -- current status before settling the account
    UPDATE  Acct
    SET     HighstStatus = CurrStatus
    WHERE   EXISTS (SELECT * FROM #RFSettle
                    WHERE  #RFSettle.acctno = Acct.acctno
                    AND    #RFSettle.CurrStatus > Acct.HighstStatus
                    AND    #RFSettle.CurrStatus NOT IN ('9', 'S', 'U'))

    -- Update current status to settle the account
    UPDATE  Acct
    SET     Currstatus = 'S'
    WHERE   EXISTS (SELECT * FROM #RFSettle WHERE  #RFSettle.AcctNo = Acct.AcctNo)


    -- Insert settle record in status table
    INSERT INTO STATUS
        (OrigBr, AcctNo, DateStatChge, Empeenostat, StatusCode)
    SELECT  0, AcctNo, GETDATE(), @empeeno, 'S'
    FROM    #RFSettle


    -- * Cancel Expried RF account *
    INSERT INTO CANCELLATION
        (OrigBr, AcctNo, AgrmtNo, DateCancel, empeenocanc, Code, AgrmtTotal)
    SELECT  0, AcctNo, '1', GETDATE(), @empeeno, 'R', AgrmtTotal
    FROM    #RFSettle

    -- * End of CancelRF *


    SET @Return = @@ERROR
    RETURN @Return
END

