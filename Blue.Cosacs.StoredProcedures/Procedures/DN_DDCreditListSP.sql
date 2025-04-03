

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDCreditListSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDCreditListSP
END
GO


CREATE PROCEDURE DN_DDCreditListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDCreditListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : List payments ready to be credited that have not been rejected
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
    @piToday                SMALLDATETIME,
    @piLeadTime             INTEGER,
    @return                 INTEGER       OUTPUT

AS  -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    SELECT
        man.AcctNo,
        man.BankCode,
        man.BankAcctNo,
        pay.PaymentId,
        pay.PaymentType,
        pay.Amount
    FROM   DDMandate man, DDPayment pay
    WHERE  man.MandateId = pay.MandateId
    AND    pay.Status = 'S'     -- $DDST_Submitted
    AND    DATEDIFF(Day, pay.DateEffective, @piToday) >= @piLeadTime
    AND    pay.Amount >= 0.01
        

    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDCreditListSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
