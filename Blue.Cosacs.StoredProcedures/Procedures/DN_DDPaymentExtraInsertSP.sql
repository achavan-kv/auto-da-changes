SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDPaymentExtraInsertSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDPaymentExtraInsertSP
END
GO


CREATE PROCEDURE DN_DDPaymentExtraInsertSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDPaymentExtraInsertSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Singapore Direct Debit Extra Payments list
-- Author       : D Richardson
-- Date         : 13 July 2005
--
-- To insert a new entry into the extra payments list
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piMandateId            INTEGER,
    @piAmount               DECIMAL,
    @poSessionConflict      SMALLINT OUT,
    @Return                 INTEGER  OUT

AS DECLARE
    -- Local variables
    @RowCount               INTEGER

BEGIN

    SET @Return = 0;
    SET @poSessionConflict = 0;

    -- First see if an update returns a RowCount and LOCK THE TABLE!
    -- Do not match on the OrigMonth key column, because a mandate should
    -- only ever have one Extra payment with Status=Init.

    UPDATE DDPayment WITH(TABLOCKX)
    SET    MandateId   = MandateId
    WHERE  MandateId   = @piMandateId
    AND    PaymentType = 'E'
    AND    Status      = 'I'

    SET @RowCount = @@ROWCOUNT

    IF @RowCount != 0
    BEGIN
         -- Another session must have inserted this row
         SET @poSessionConflict = 1;
    END
    ELSE
    BEGIN
        INSERT INTO DDPayment
           (MandateId,
            PaymentType,
            OrigPaymentType,
            OrigMonth,
            DateEffective,
            Amount,
            Status,
            RejectCount)
        VALUES
           (@piMandateId,
            'E',
            'E',
            MONTH(GETDATE()),
            NULL,
            @piAmount,
            'I',
            0)
    END

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_DDPaymentExtraInsertSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
