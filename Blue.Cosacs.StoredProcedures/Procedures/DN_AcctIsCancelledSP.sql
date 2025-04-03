if exists (select * from sysobjects where name = 'DN_AcctIsCancelledSP')

DROP PROCEDURE DN_AcctIsCancelledSP
GO

CREATE PROCEDURE DN_AcctIsCancelledSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS
-- File Name    : DN_AcctIsCancelledSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check whether an account is cancelled
-- Author       : D Richardson
-- Date         : 17 May 2004
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo          VARCHAR(12),
    @poIsCancelled     INTEGER OUTPUT,
    @return            INTEGER OUTPUT

AS --DECLARE
    -- Local variables

BEGIN
    SELECT @poIsCancelled = COUNT(*)
    FROM   Cancellation
    WHERE  AcctNo = @piAcctNo

    SET @Return = @@ERROR
    RETURN 0
END

GO
GRANT EXECUTE ON DN_AcctIsCancelledSP TO PUBLIC


