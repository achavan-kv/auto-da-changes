

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDRejectPaymentsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDRejectPaymentsSP
END
GO


CREATE PROCEDURE DN_DDRejectPaymentsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDRejectPaymentsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Reject payments that match rejection records
-- Author       : D Richardson
-- Date         : 10 May 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piToday                SMALLDATETIME,
    @piLeadTime             SMALLINT,
    @piCountryFee           MONEY,
    @return                 INTEGER       OUTPUT

AS  -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON
    SET @return = 0


    /* Update the submitted payments that have been rejected.
    ** The Mandate Id is also updated on the payment to the latest Mandate Id,
    ** in case a mandate history record was created while waiting for the
    ** rejection from the bank.
    */
    UPDATE DDPayment WITH (TABLOCKX)
    SET    Status        = 'R',                     -- $DDST_Rejected
           RejectReason  = trej.RejectReason,
           RejectReason2 = trej.RejectReason2,
           RejectAction  = (CASE PaymentType
                            WHEN 'F' THEN 'N'     -- $DDPT_Fee, $DDRA_NotRepresent
                            ELSE 'R' END),         -- $DDRA_Represent
           RejectCount   = DDPayment.RejectCount + 1,
           MandateId     = man1.MandateId
    FROM   DDRejections trej, DDMandate man1
    WHERE  DDPayment.PaymentId = trej.PaymentId
    AND    man1.AcctNo = trej.AcctNo
    AND    man1.MandateId =
               (SELECT MAX(man2.MandateId)
                FROM   DDMandate man2
                WHERE  man2.AcctNo = trej.AcctNo)



    /* Update the rejection counter on the mandates
    ** for Normal and Representation payments.
    */
    UPDATE DDMandate WITH (TABLOCKX)
    SET    RejectCount = DDMandate.RejectCount + 1
    FROM   DDRejections trej, DDPayment pay
    WHERE  DDMandate.MandateId = pay.MandateId
    AND    pay.PaymentId = trej.PaymentId
    AND    pay.PaymentType IN ('N','R')     -- $DDPT_Normal, $DDPT_Represent



    /* Add the Fee payments to the DDPayment table but with the Status = Hold.
    ** After the the Debit Fee Account Transaction is created successfully, the
    ** Status will be changed to Init so that the payment can be sent to the bank.
    **
    ** The Normal Payments file also includes 'deleterecords to delete mandates
    ** at the bank. These are stored as -1 amounts and therefore this query will
    ** not add a fee for an amount less than 0.01.
    */

    /* The country has the option to not charge a fee */
    IF @piCountryFee >= 0.01
    BEGIN
        INSERT INTO DDPayment WITH (TABLOCKX)
            (MandateId,
             PaymentType,
             OrigPaymentType,
             OrigMonth,
             DateEffective,
             Amount,
             Status,
             RejectAction)
        SELECT
             pay.MandateId,
             'F',       -- $DDPT_Fee
             'F',       -- $DDPT_Fee
             pay.OrigMonth,
             @piToday,
             @piCountryFee,
             'H',       -- $DDST_Hold
             'I'        -- $DDRA_Init
        FROM   DDRejections trej, DDPayment pay, DDMandate man
        WHERE  pay.PaymentId = trej.PaymentId
        AND    man.MandateId = pay.MandateId
        AND    man.noFees = 0 
        AND    pay.PaymentType IN ('N', 'R')        -- $DDPT_Normal, $DDPT_Represent
        AND    pay.Amount >= 0.01 
    END
    

    /* Mark the 'DELETE' payments as complete */
    UPDATE DDPayment
    SET    Status = 'C'         -- $DDST_Complete''
    FROM   DDPayment pay
    WHERE  pay.Status = 'S'     -- $DDST_Submitted''
    AND    DATEDIFF(Day, pay.DateEffective, @piToday) >= @piLeadTime
    AND    pay.Amount = -1.0


    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDRejectPaymentsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
