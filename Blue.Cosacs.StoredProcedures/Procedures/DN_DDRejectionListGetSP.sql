SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDRejectionListGetSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDRejectionListGetSP
END
GO


CREATE PROCEDURE DN_DDRejectionListGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDRejectionListGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Singapore Direct Debit Rejections list
-- Author       : D Richardson
-- Date         : 18 July 2005
--
-- To return a list of payments rejected by the bank
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piEffectiveDate    SMALLDATETIME,
    @Return             INTEGER OUT

AS --DECLARE
    -- Local variables
BEGIN

    SELECT
        man.MandateId,
        pay.PaymentId,
        man.AcctNo,
        RTRIM(customer.Title + ' '
              + customer.Name + ', '
              + customer.Firstname) AS CustomerName,
        pay.PaymentType,
        pay.OrigMonth,
        LEFT(DATENAME(Month, DATEADD(Month, pay.OrigMonth-1, 0)),3) AS MonthName,
        pay.DateEffective,
        pay.Amount,
        pay.RejectAction,
        pay.RejectAction AS CurRejectAction,
        c.CodeDescript AS RejectActionStr
    FROM  CustAcct, Customer, DDMandate man, DDPayment pay, Code c
    WHERE man.Status = 'C'
    AND   (   man.EndDate IS NULL
           OR DATEDIFF(Day, man.EndDate, @piEffectiveDate) < 0)
    AND   CustAcct.AcctNo = man.AcctNo
    AND   CustAcct.HldOrJnt = 'H'
    AND   Customer.CustId = CustAcct.CustId
    AND   pay.MandateId = man.MandateId
    AND   pay.Status = 'R'
    AND   c.Category = 'GRA'
    AND   c.Code = pay.RejectAction
    ORDER BY man.AcctNo, pay.OrigMonth, pay.PaymentType, pay.DateEffective

    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON DN_DDRejectionListGetSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
