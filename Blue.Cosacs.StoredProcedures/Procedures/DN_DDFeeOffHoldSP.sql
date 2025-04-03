

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDFeeOffHoldSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDFeeOffHoldSP
END
GO


CREATE PROCEDURE DN_DDFeeOffHoldSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDFeeOffHoldSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Take the fee payment off hold
-- Author       : D Richardson
-- Date         : 4 May 2006
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
    @return                 INTEGER       OUTPUT

AS  -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON
    SET @return = 0


    UPDATE DDPayment
    SET    Status = 'I'     -- $DDST_Init
    WHERE  PaymentId = @piPaymentId
        

    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDFeeOffHoldSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
