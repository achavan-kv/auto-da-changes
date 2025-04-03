SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDPaymentExtraDeleteSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDPaymentExtraDeleteSP
END
GO


CREATE PROCEDURE DN_DDPaymentExtraDeleteSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDPaymentExtraDeleteSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Singapore Direct Debit Extra Payments list
-- Author       : D Richardson
-- Date         : 13 July 2005
--
-- To delete an entry from the extra payments list
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piMandateId        INTEGER,
    @piPaymentId        INTEGER,
    @piOrigMonth        SMALLINT,
    @piCurAmount        DECIMAL,
    @poSessionConflict  SMALLINT OUT,
    @Return             INTEGER  OUT

AS DECLARE
    -- Local variables
    @RowCount           INTEGER

BEGIN

    SET @poSessionConflict = 0;

    -- Delete the row, matching every column in case
    -- it has been changed by another session.

    DELETE FROM DDPayment 
    WHERE  PaymentId   = @piPaymentId
    AND    MandateId   = @piMandateId
    AND    PaymentType = 'E' 
    AND    OrigMonth   = @piOrigMonth
    AND    DateEffective IS NULL 
    AND    Amount      = @piCurAmount
    AND    Status      = 'I'

    SET @RowCount = @@ROWCOUNT;

    IF @RowCount = 0
    BEGIN
        /* Another session must have changed this row */
        SET @poSessionConflict = 1;
    END

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_DDPaymentExtraDeleteSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
