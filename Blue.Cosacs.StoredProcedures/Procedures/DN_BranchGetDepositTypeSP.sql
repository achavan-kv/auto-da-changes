SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_BranchGetDepositTypeSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_BranchGetDepositTypeSP
END
GO


CREATE PROCEDURE DN_BranchGetDepositTypeSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS.NET
-- File Name    : DN_BranchGetDepositTypeSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Return the deposit type for a branch and paymethod
-- Author       : D Richardson
-- Date         : 22 Feb 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piBranchNo         INTEGER,
    @piPayMethod        VARCHAR(12),
    @poDeposit          VARCHAR(3) OUTPUT,
    @Return             INTEGER OUTPUT

AS
BEGIN
    SET @Return = 0

    SELECT @poDeposit = Deposit
    FROM   BranchDeposit
    WHERE  BranchNo  = @piBranchNo
    AND    PayMethod = @piPayMethod

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_BranchGetDepositTypeSP TO PUBLIC
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
