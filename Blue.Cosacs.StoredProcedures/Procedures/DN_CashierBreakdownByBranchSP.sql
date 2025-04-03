SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_CashierBreakdownByBranchSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_CashierBreakdownByBranchSP
END
GO


CREATE PROCEDURE DN_CashierBreakdownByBranchSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_CashierBreakdownByBranchSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load Cashier Shortage and Overage breakdown for a whole branch
-- Author       : D Richardson
-- Date         : 18 Mar 2005
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piBranchNo       SMALLINT,
    @piDateStart      DATETIME,
    @piDateEnd        DATETIME,
    @Return           INTEGER OUTPUT

AS -- DECLARE
    -- Local variables

BEGIN
    SET @Return = 0

    SELECT ct.EmpeeNo, ct.DateTo,
           CASE WHEN ctb.Difference < 0 THEN 'SHO' ELSE 'OVE' END AS TransTypeCode,
           c.CodeDescript AS PayMethod,
           ctb.Difference,
           ctb.Reason
    FROM   CashierTotals ct, CashierTotalsBreakdown ctb, Code c
    WHERE  ct.BranchNo = @piBranchNo
    AND    ct.DateTo BETWEEN @piDateStart AND @piDateEnd
    AND    ctb.CashierTotalId = ct.Id
    AND    ctb.Difference != 0
    AND    c.Category = 'FPM'
    AND    c.Code = ctb.PayMethod

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_CashierBreakdownByBranchSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
