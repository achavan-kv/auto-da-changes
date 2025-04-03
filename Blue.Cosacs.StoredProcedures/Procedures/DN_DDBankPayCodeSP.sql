

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDBankPayCodeSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDBankPayCodeSP
END
GO


CREATE PROCEDURE DN_DDBankPayCodeSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDBankPayCodeSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get the bank Pay Code for the payment type and due day
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
    @poPayCode          CHAR(5) OUTPUT,
    @return             INTEGER  OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* Get the appropriate bank pay code */
    SELECT @poPayCode = PayCode
    FROM   DDBankPayCode
    WHERE  PaymentType = @piPaymentType
    AND    DueDayId = @piDueDayId
        
    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDBankPayCodeSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
