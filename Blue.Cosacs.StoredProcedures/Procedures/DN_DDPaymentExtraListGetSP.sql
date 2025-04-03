SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDPaymentExtraListGetSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDPaymentExtraListGetSP
END
GO


CREATE PROCEDURE DN_DDPaymentExtraListGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDPaymentExtraListGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Singapore Direct Debit Extra Payments list
-- Author       : D Richardson
-- Date         : 13 July 2005
--
-- To return a list of accounts in arrears and any extra payments
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piEffectiveDate    SMALLDATETIME,
    @Return             INTEGER OUT

AS --DECLARE
    -- Local variables
BEGIN
   
    /* Create a temporary table for extra payments */
    CREATE TABLE #DDExtraPayment ( 
       MandateId       INT             PRIMARY KEY NOT NULL, 
       AcctNo          CHAR(12)        NOT NULL, 
       CustomerName    CHAR(115)      NOT NULL, 
       OutStBal        MONEY           NOT NULL DEFAULT 0, 
       Arrears         MONEY           NOT NULL DEFAULT 0, 
       DDPending       MONEY           NOT NULL DEFAULT 0, 
       Amount          MONEY           NOT NULL DEFAULT 0, 
       Consent         TINYINT         NOT NULL DEFAULT 0, 
       OrigMonth       TINYINT         NOT NULL DEFAULT 0, 
       PaymentId       INT             NOT NULL DEFAULT 0)
        

    /* Load the existing extra payments */
    INSERT INTO #DDExtraPayment 
       (MandateId, 
        AcctNo, 
        CustomerName, 
        OutstBal, 
        Arrears, 
        Amount, 
        PaymentId, 
        Consent, 
        OrigMonth) 
    SELECT 
        man.MandateId, 
        man.AcctNo, 
        customer.Title + ' '
          + customer.Name + ', ' 
          + customer.Firstname, 
        ISNULL(acct.OutStBal,0), 
        ISNULL(acct.Arrears,0), 
        ISNULL(extra.Amount,0), 
        extra.PaymentId, 
        1, 
        extra.OrigMonth 
    FROM  DDMandate man, CustAcct, Customer, Acct, DDPayment extra 
    WHERE man.Status = 'C' 
    AND   DATEDIFF(Day, man.StartDate, @piEffectiveDate) >= 0 
    AND   (   man.EndDate IS NULL 
           OR DATEDIFF(Day, man.EndDate, @piEffectiveDate) < 0) 
    AND   CustAcct.AcctNo = man.AcctNo 
    AND   CustAcct.HldOrJnt = 'H' 
    AND   Customer.CustId = CustAcct.CustId 
    AND   Acct.AcctNo = man.AcctNo 
    AND   extra.MandateId = man.MandateId 
    AND   extra.PaymentType = 'E' 
    AND   extra.Status = 'I';
        

    /* Load new extra payments for current mandates with arrears */
    INSERT INTO #DDExtraPayment 
       (MandateId, 
        AcctNo, 
        CustomerName, 
        OutstBal, 
        Arrears) 
    SELECT 
        man.MandateId, 
        man.AcctNo, 
        customer.Title + ' ' 
          + customer.Name + ', ' 
          + customer.Firstname, 
        ISNULL(acct.OutStBal,0), 
        ISNULL(acct.Arrears,0) 
    FROM  DDMandate man, CustAcct, Customer, Acct 
    WHERE man.Status = 'C' 
    AND   DATEDIFF(Day, man.StartDate, @piEffectiveDate) >= 0 
    AND   (   man.EndDate IS NULL 
           OR DATEDIFF(Day, man.EndDate, @piEffectiveDate) < 0) 
    AND   CustAcct.AcctNo = man.AcctNo 
    AND   CustAcct.HldOrJnt = 'H' 
    AND   Customer.CustId = CustAcct.CustId 
    AND   Acct.AcctNo = man.AcctNo 
    AND   Acct.Arrears > 0.0 
    AND   man.MandateId NOT IN (SELECT MandateId FROM #DDExtraPayment)
        

    /* Calculate the DDPending amount for each mandate */
    UPDATE #DDExtraPayment 
    SET    DDPending = 
        ISNULL((SELECT ROUND(SUM(Amount),2)
                FROM   DDMandate man, DDPayment pay 
                WHERE  man.AcctNo = #DDExtraPayment.AcctNo 
                AND    pay.MandateId = man.MandateId 
                AND    (   pay.Status = 'I' 
                        OR pay.Status = 'S' 
                        OR (    pay.Status = 'R' 
                        AND pay.RejectAction = 'R'))),0)


    /* Return the list.
    ** New rows (consent=0) will be excluded where the amount
    ** already pending is greater than the arrears.
    */
    SELECT 
       MandateID, 
       AcctNo, 
       RTRIM(CustomerName) AS CustomerName, 
       OutStBal, 
       Arrears, 
       DDPending, 
       DDPending AS CurDDPending, 
       Amount, 
       Amount AS CurAmount, 
       Consent, 
       Consent AS CurConsent, 
       OrigMonth, 
       PaymentId 
    FROM #DDExtraPayment extra 
    WHERE ROUND(Arrears,2) > ROUND(DDPending,2) 
    OR    Consent = 1 
    ORDER BY Acctno 

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_DDPaymentExtraListGetSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

