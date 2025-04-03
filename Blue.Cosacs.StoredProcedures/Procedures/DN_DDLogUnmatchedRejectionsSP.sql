

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDLogUnmatchedRejectionsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDLogUnmatchedRejectionsSP
END
GO


CREATE PROCEDURE DN_DDLogUnmatchedRejectionsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDLogUnmatchedRejectionsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Log rejection records that do NOT match any payments
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
    @piInterface            VARCHAR(12),
    @piRunNo                INTEGER,
    @return                 INTEGER       OUTPUT

AS  -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON
    SET @return = 0


    /* Any rejections not matched to a payment are in error and are logged */
    INSERT INTO InterfaceError (
        Interface,
        RunNo,
        ErrorDate,
        ErrorText,
        Severity)
    SELECT
        @piInterface,
        @piRunNo,
        GETDATE(),
        'Unmatched rejection record at line ' +
        CAST (RejectId AS VARCHAR) + ':' +
        ' Courts acct no='   + CAST (AcctNo AS VARCHAR) +
        ' Bank acct name='   + CAST (BankAcctName AS VARCHAR) +
        ' Bank code='        + CAST (BankCode AS VARCHAR) +
        ' Bank branch no='   + CAST (BankBranchNo AS VARCHAR) +
        ' Bank acct no='     + CAST (BankAcctNo AS VARCHAR) +
        ' Amount='           + CAST (Amount AS VARCHAR) +
        ' Pay code='         + CAST (RejectCode AS VARCHAR) +
        ' Reject reason='    + CAST (RejectReason AS VARCHAR) +
        ' Reject reason2='   + CAST (RejectReason2 AS VARCHAR),
        'W'
    FROM  DDRejections
    WHERE PaymentId IS NULL
    ORDER BY RejectId


    /* Delete the unmatched records */
    DELETE FROM DDRejections
    WHERE PaymentId IS NULL
        

    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDLogUnmatchedRejectionsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
