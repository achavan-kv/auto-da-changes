

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDSubmittedToBankSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDSubmittedToBankSP
END
GO


CREATE PROCEDURE DN_DDSubmittedToBankSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDSubmittedToBankSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Mark the pending payments as submitted to the bank
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
    @piEffectiveDate    SMALLDATETIME,
    @return             INTEGER OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* Update the pending payments as having been submitted to the bank.
    ** Ensure no other session is using this table by using an
    ** exclusive table lock.
    */
    IF @piPaymentType = 'R'                                            -- $DDPT_Represent
    BEGIN
        /* Representations need the payment type to be changed */
        UPDATE DDPayment WITH (TABLOCKX)
        SET    PaymentType = 'R',                                      -- $DDPT_Represent
               DateEffective = @piEffectiveDate,
               Status = 'S',                                           -- $DDST_Submitted
               RejectAction = 'I'                                      -- $DDRA_Init
        FROM   DDMandate man
        WHERE  DDPayment.Status = 'R'                                  -- $DDST_Rejected
        AND    DDPayment.RejectAction = 'R'                            -- $DDRA_Represent
        AND    man.MandateId = DDPayment.MandateId
        AND    man.DueDayId = @piDueDayId
    END
    ELSE
    BEGIN
        UPDATE DDPayment WITH (TABLOCKX)
        SET    Status = 'S',                                           -- $DDST_Submitted
               DateEffective = @piEffectiveDate
        FROM   DDMandate man
        WHERE  DDPayment.PaymentType = @piPaymentType
        AND    DDPayment.Status = 'I'                                  -- $DDST_Init
        AND    man.MandateId = DDPayment.MandateId
        AND    man.DueDayId = @piDueDayId
    END

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDSubmittedToBankSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
