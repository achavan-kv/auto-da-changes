SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_ManualCDVExistsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_ManualCDVExistsSP
END
GO


CREATE PROCEDURE DN_ManualCDVExistsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_ManualCDVExistsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Return TRUE if a manual account no is on the ManualCDV table
-- Author       : D Richardson
-- Date         : 22 Mar 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo         VARCHAR(12),
    @poExists         SMALLINT OUTPUT,
    @Return           INTEGER OUTPUT

AS -- DECLARE
    -- Local variables

BEGIN
    SET @Return = 0
    SET @poExists = 0

    IF EXISTS (SELECT * FROM ManualCDV WHERE AcctNo = @piAcctNo)
        SET @poExists = 1

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_ManualCDVExistsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
