SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_BranchSaveDepositTypeSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_BranchSaveDepositTypeSP
END
GO


CREATE PROCEDURE DN_BranchSaveDepositTypeSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS.NET
-- File Name    : DN_BranchSaveDepositTypeSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save the deposit type for a branch and paymethod
-- Author       : D Richardson
-- Date         : 23 Feb 2005
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
    @piDeposit          VARCHAR(3),
    @Return             INTEGER OUTPUT

AS
BEGIN
    SET @Return = 0

    UPDATE  BranchDeposit
    SET     Deposit = @piDeposit
    WHERE   BranchNo  = @piBranchNo
    AND     PayMethod = @piPayMethod

    IF (@@ROWCOUNT = 0)
    BEGIN
        INSERT INTO BranchDeposit (BranchNo, PayMethod, Deposit)
        VALUES (@piBranchNo, @piPayMethod, @piDeposit)
    END

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_BranchSaveDepositTypeSP TO PUBLIC
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
