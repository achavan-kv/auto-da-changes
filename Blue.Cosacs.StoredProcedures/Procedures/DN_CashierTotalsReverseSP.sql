
DROP PROCEDURE DN_CashierTotalsReverseSP
GO

CREATE PROCEDURE DN_CashierTotalsReverseSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS dotNET
-- File Name    : DN_CashierTotalsReverseSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : To reverse Cashier Totals
-- Author       : D Richardson
-- Date         : 7 June 2004
--
-- Note that the 'Difference' is returned to be posted as a
-- reversal onto either the Overage or Shortage account.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piEmployeeNo      INTEGER,
    @poDifference      MONEY   OUTPUT,
    @return            INT     OUTPUT

AS DECLARE
    -- Local variables
    @Id                INTEGER,
    @DateFrom          DATETIME,
    @DateTo            DATETIME

BEGIN

    SET @return = 0
    
    BEGIN TRANSACTION

        -- Determine the latest totals for this employee
        SELECT @Id           = Id,
               @DateFrom     = DateFrom,
               @DateTo       = DateTo,
               @poDifference = Difference
        FROM   CashierTotals
        WHERE  EmpeeNo  = @piEmployeeNo
        AND    DateTo   = (SELECT MAX(DateTo) FROM CashierTotals
                           WHERE  EmpeeNo = @piEmployeeNo)


        -- Reverse Cashier Totals
        INSERT INTO CashierTotalsReversed
              (id, datefrom, dateto, empeeno, runno, empeenoauth, usertotal, systemtotal, difference, branchno, deposittotal)
        SELECT id, datefrom, dateto, empeeno, runno, empeenoauth, usertotal, systemtotal, difference, branchno, deposittotal
        FROM   CashierTotals
        WHERE  Id = @Id
        
        DELETE FROM CashierTotals
        WHERE  Id = @Id


        -- Reverse Cashier Totals Breakdown
        INSERT INTO CashierTotalsBreakdownReversed
              (cashiertotalid, paymethod, systemtotal, usertotal, deposit, difference, reason, securitisedtotal)
        SELECT cashiertotalid, paymethod, systemtotal, usertotal, deposit, difference, reason, securitisedtotal
        FROM   CashierTotalsBreakdown
        WHERE  CashierTotalId = @Id
        
        DELETE FROM CashierTotalsBreakdown
        WHERE  CashierTotalId = @Id


        -- Reverse income transactions
        INSERT INTO Fintrans_New_Income
              (branchno, acctno,transrefno, datetrans, transtypecode, empeeno, transvalue, bankcode, bankacctno, chequeno, paymethod)
        SELECT branchno, acctno,transrefno, datetrans, transtypecode, empeeno, transvalue, bankcode, bankacctno, chequeno, paymethod
        FROM   CashierTotalsIncome
        WHERE  EmpeeNo     = @piEmployeeNo
        AND    DateTrans   > @DateFrom
        AND    DateTrans  <= @DateTo
        
        DELETE FROM CashierTotalsIncome
        WHERE  EmpeeNo     = @piEmployeeNo
        AND    DateTrans   > @DateFrom
        AND    DateTrans  <= @DateTo


        -- Reduce the deposit outstanding by the amount of the reversal
        UPDATE CashierOutstanding
        SET    DepositOutstanding = DepositOutstanding - ctb.UserTotal
        FROM   CashierTotalsBreakdownReversed ctb
        WHERE  CashierOutstanding.EmpeeNo   = @piEmployeeNo
        AND    CashierOutstanding.PayMethod = ctb.PayMethod
        AND    ctb.CashierTotalId = @Id


        -- Reset Cashier Deposits linked to the reversed totals
        UPDATE CashierDeposits
        SET    CashierTotalId = 0
        WHERE  CashierTotalId = @Id


        -- Reset the date of the last audit for this employee
        UPDATE CourtsPerson
        SET    DateLstAudit = (SELECT ISNULL(MAX(DateTo),'') FROM CashierTotals
                               WHERE  EmpeeNo = @piEmployeeNo)

    COMMIT
          
    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_CashierTotalsReverseSP TO PUBLIC

