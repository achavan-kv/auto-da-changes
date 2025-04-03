SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SafeBalanceGetSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SafeBalanceGetSP
END
GO


CREATE PROCEDURE DN_SafeBalanceGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SafeBalanceGetSP.PRC
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Available cash in the safe
-- Author       : D Richardson
-- Date         : 31 May 2005
--
-- The amount available for deposit is just given by the sum of movements to 
-- and from the safe. This is misleading because only money which has been 
-- totalled is actually available for deposit to the bank. The figure we 
-- are displaying is actually the amount of money physically in the safe,
-- not necessarily the amount available for deposit. This inconsistency
-- has been pointed out to no avail. 
--
-- Once again, all figures must be in local currency therefore foreign 
-- currency records in the cashier deposits table must be converted.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @BranchNo       SMALLINT,
    @InSafe         MONEY       OUTPUT,
    @Return         INTEGER     OUTPUT

AS -- DECLARE
    -- Local variables

BEGIN
    SET @Return = 0

    SELECT  @InSafe = cast(ISNULL(SUM(DepositValue),0) AS DECIMAL(12,2))
    FROM    CashierDeposits 
    WHERE   BranchNo = @BranchNo
    AND     Code = 'SAF'
    AND     DateVoided IS NULL
    AND     CONVERT(INT, PayMethod) < 100       -- local currency deposits

    SELECT  @InSafe = @InSafe + ISNULL(ROUND(SUM(CD.DepositValue * ISNULL(ER.Rate,1)),2),0)
    FROM    CashierDeposits CD, ExchangeRate ER
    WHERE   CD.BranchNo = @BranchNo    
    AND     CD.Code = 'SAF'
    AND     CD.DateVoided IS NULL
    AND     CONVERT(INT, PayMethod) >= 100      -- foreign currency deposits
    AND     ER.Currency = CD.PayMethod
    AND     ER.DateFrom <= CD.DateDeposit
    AND     ER.Status = 'C'


    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_SafeBalanceGetSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

