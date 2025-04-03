

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDPendingPaymentListSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDPendingPaymentListSP
END
GO


CREATE PROCEDURE DN_DDPendingPaymentListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDPendingPaymentListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get the list of pending payments for this payment type and due day
-- Author       : D Richardson
-- Date         : 3 May 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piPaymentType      CHAR(1),
    @piDueDayId         INTEGER,
    @return             INTEGER OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* Retrieve all the pending payments.
    ** Ensure no other session is using these tables by using
    ** exclusive table locks.
    */

    IF (@piPaymentType = 'R')                                          -- $DDPT_Represent
    BEGIN
        /* Representations can be any payment type */
        SELECT
            CONVERT(CHAR(12), man.AcctNo) +                           -- $DDFL_CourtsAcctNo
            CONVERT(CHAR(20), man.BankAcctName) +                     -- $DDFL_BankAcctName
            CONVERT(CHAR(4),  man.BankCode) +                         -- $DDFL_BankCode
            CONVERT(CHAR(3),  man.BankBranchNo) +                     -- $DDFL_BankBranchNo
            CONVERT(CHAR(16), man.BankAcctNo) AS RecordLine,          -- $DDFL_BankAcctNo
            CONVERT(INTEGER,  ROUND(pay.Amount*100.00,0,1)) AS Amount
        FROM  DDMandate man WITH (TABLOCKX), DDPayment pay WITH (TABLOCKX)
        WHERE pay.Status = 'R'                                        -- $DDST_Rejected
        AND   pay.RejectAction = 'R'                                  -- $DDRA_Represent
        AND   man.MandateId = pay.MandateId
        AND   man.DueDayId = @piDueDayId
    END
    ELSE
    BEGIN
        SELECT
            CONVERT(CHAR(12), man.AcctNo) +                           -- $DDFL_CourtsAcctNo
            CONVERT(CHAR(20), man.BankAcctName) +                     -- $DDFL_BankAcctName
            CONVERT(CHAR(4),  man.BankCode) +                         -- $DDFL_BankCode
            CONVERT(CHAR(3),  man.BankBranchNo) +                     -- $DDFL_BankBranchNo
            CONVERT(CHAR(16), man.BankAcctNo) AS RecordLine,          -- $DDFL_BankAcctNo
            CONVERT(INTEGER,  ROUND(pay.Amount*100.00,0,1)) AS Amount
        FROM  DDMandate man WITH (TABLOCKX), DDPayment pay WITH (TABLOCKX)
        WHERE pay.PaymentType = @piPaymentType
        AND   pay.Status = 'I'                                        -- $DDST_Init
        AND   man.MandateId = pay.MandateId
        AND   man.DueDayId = @piDueDayId
    END

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDPendingPaymentListSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
