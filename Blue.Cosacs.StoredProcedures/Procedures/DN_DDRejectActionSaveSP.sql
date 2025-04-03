SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDRejectActionSaveSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDRejectActionSaveSP
END
GO


CREATE PROCEDURE DN_DDRejectActionSaveSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDRejectActionSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Singapore Direct Debit Reject Actions
-- Author       : D Richardson
-- Date         : 13 July 2005
--
-- To update a payment with the user Reject Action
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piRejectAction     CHAR(1),
    @piPaymentId        INTEGER,
    @piMandateId        INTEGER,
    @piPaymentType      CHAR(1),
    @piOrigMonth        SMALLINT,
    @piAmount           MONEY,
    @piCurRejectAction  CHAR(1),
    @poSessionConflict  SMALLINT OUT,
    @Return             INTEGER  OUT

AS DECLARE
    -- Local variables
    @RowCount            INTEGER

BEGIN

    SET @poSessionConflict = 0;

    -- Update the row, matching every column in case it
    -- has been changed by another session.

    UPDATE DDPayment
    SET    RejectAction = @piRejectAction
    WHERE  PaymentId     = @piPaymentId
    AND    MandateId     = @piMandateId
    AND    PaymentType   = @piPaymentType
    AND    OrigMonth     = @piOrigMonth
    AND    Amount        = @piAmount
    AND    Status        = 'R'
    AND    RejectAction  = @piCurRejectAction

    SET @RowCount = @@ROWCOUNT
    IF @RowCount = 0
    BEGIN
         -- Another session must have updated this row
         SET @poSessionConflict = 1;
    END
 
    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_DDRejectActionSaveSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
