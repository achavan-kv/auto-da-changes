SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_BranchGetDepositListSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_BranchGetDepositListSP
END
GO


CREATE PROCEDURE DN_BranchGetDepositListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS.NET
-- File Name    : DN_BranchGetDepositListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Return the list of deposit types and paymethods for a branch
-- Author       : D Richardson
-- Date         : 22 Feb 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 04/03/05  DSR Need to outer join category 'FPM' to BranchDeposit
--
--------------------------------------------------------------------------------

    -- Parameters
    @piBranchNo         INTEGER,
    @Return             INTEGER OUTPUT

AS
BEGIN
    SET @Return = 0

    -- Load everything from Category 'FPM' (except blank entry '0')
    -- and outer join to BranchDeposit
    SELECT @piBranchNo AS BranchNo, c.Code AS PayMethod, c.CodeDescript,
           ISNULL(b.Deposit,'') AS Deposit,
           ISNULL(t.Description,'') AS Description
    FROM   Code c
    LEFT   OUTER JOIN BranchDeposit b
    ON     b.BranchNo      = @piBranchNo
    AND    b.PayMethod     = c.Code
    LEFT   OUTER JOIN TransType t
    ON     t.TransTypeCode = b.Deposit
    WHERE  c.Category      = 'FPM'
    AND    c.Code         != '0'
    ORDER BY PayMethod

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_BranchGetDepositListSP TO PUBLIC
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

