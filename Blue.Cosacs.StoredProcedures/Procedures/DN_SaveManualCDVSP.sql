SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SaveManualCDVSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SaveManualCDVSP
END
GO


CREATE PROCEDURE DN_SaveManualCDVSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SaveManualCDVSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Record a manual account no on the ManualCDV table
-- Author       : D Richardson
-- Date         : 21 Mar 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo         VARCHAR(12),
    @Return           INTEGER OUTPUT

AS -- DECLARE
    -- Local variables

BEGIN
    SET @Return = 0

    INSERT INTO ManualCDV (AcctNo)
    VALUES (@piAcctNo)

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_SaveManualCDVSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
