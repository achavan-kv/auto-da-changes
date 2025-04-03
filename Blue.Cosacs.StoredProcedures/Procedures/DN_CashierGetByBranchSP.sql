SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_CashierGetByBranchSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_CashierGetByBranchSP
END
GO


CREATE PROCEDURE DN_CashierGetByBranchSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_CashierGetByBranchSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load Cashier Overages and Shortages summary for a branch
-- Author       : D Richardson
-- Date         : 17 Mar 2005
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

    -- Load all the Cashiers with a Shortage or Overage on FinTrans
    -- for this Branch and in this date range.
    SELECT f.EmpeeNo, u.FullName as EmployeeName, ISNULL(ca.AcctNo,'') AS AcctNo,
           CONVERT(Money,0.0) AS Shortage,
           CONVERT(Money,0.0) AS Overage
    INTO   #tmpCashier
    FROM   FinTrans f
    INNER JOIN Admin.[User] u ON f.empeeno = u.id
    LEFT OUTER JOIN Custacct ca ON ca.CustId = 'SHORTAGE' + CONVERT(VARCHAR, u.Id)
    WHERE  f.BranchNo = @piBranchNo
    AND    f.DateTrans BETWEEN @piDateStart AND @piDateEnd
    AND    f.TransTypeCode IN ('SHO','OVE')
    GROUP BY f.EmpeeNo, u.FullName, ca.AcctNo

    -- Update each Cashier Shortage
    UPDATE #tmpCashier
    SET Shortage = ISNULL(-(SELECT SUM(f.TransValue)
                            FROM   FinTrans f
                            WHERE  f.BranchNo = @piBranchNo
                            AND    f.DateTrans BETWEEN @piDateStart AND @piDateEnd
                            AND    f.TransTypeCode = 'SHO'
                            AND    f.EmpeeNo  = #tmpCashier.EmpeeNo),0)

    -- Update each Cashier Overage
    UPDATE #tmpCashier
    SET Overage =   ISNULL(-(SELECT SUM(f.TransValue)
                             FROM   FinTrans f
                             WHERE  f.BranchNo = @piBranchNo
                             AND    f.DateTrans BETWEEN @piDateStart AND @piDateEnd
                             AND    f.TransTypeCode = 'OVE'
                             AND    f.EmpeeNo  = #tmpCashier.EmpeeNo),0)

    SELECT * FROM #tmpCashier

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_CashierGetByBranchSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
