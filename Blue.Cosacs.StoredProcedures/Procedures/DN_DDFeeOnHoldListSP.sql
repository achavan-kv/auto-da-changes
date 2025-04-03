

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDFeeOnHoldListSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDFeeOnHoldListSP
END
GO


CREATE PROCEDURE DN_DDFeeOnHoldListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDFeeOnHoldListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : List the fees on hold waiting to create their financial transaction
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
    @return                 INTEGER       OUTPUT

AS  -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    /* Retrieve the fees on hold waiting to create their financial transaction.
    ** Some of the fees on hold could be from a previous run that failed to
    ** create their financial transaction during that run. This could be because
    ** the account was closed, and the users have now reopened the account to
    ** rectify the problem.
    */
    SELECT
        man.AcctNo,
        man.BankCode,
        man.BankAcctNo,
        pay.Amount,
        pay.PaymentId
    FROM   DDPayment pay, DDMandate man
    WHERE  man.MandateId = pay.MandateId
    AND    pay.PaymentType = 'F'        -- $DDPT_Fee
    AND    pay.Status = 'H'             -- $DDST_Hold


    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDFeeOnHoldListSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
