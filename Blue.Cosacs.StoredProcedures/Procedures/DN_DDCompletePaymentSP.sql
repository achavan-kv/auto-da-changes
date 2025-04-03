

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDCompletePaymentSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDCompletePaymentSP
END
GO


CREATE PROCEDURE DN_DDCompletePaymentSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDCompletePaymentSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Mark a DD payment as complete 
-- Author       : D Richardson
-- Date         : 11 May 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piPaymentId            INTEGER,
    @piPaymentType          CHAR(1),
    @piAcctNo               CHAR(12),
    @return                 INTEGER       OUTPUT

AS  -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    UPDATE DDPayment
    SET    Status = 'C'     -- $DDST_Complete
    WHERE  PaymentId = @piPaymentId
        

    IF @piPaymentType = 'N'     -- $DDPT_Normal
    OR @piPaymentType = 'R'     -- $DDPT_Represent
    BEGIN 
        /* Reset the rejection counter on the mandate for a successful
        ** Normal payment or representation.
        ** (Reset the 'Current' record with this customer account number).
        */
        UPDATE DDMandate
        SET    RejectCount = 0
        WHERE  Status = 'C'     -- $DDMS_Current
        AND    AcctNo = @piAcctNo
    END

    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDCompletePaymentSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
