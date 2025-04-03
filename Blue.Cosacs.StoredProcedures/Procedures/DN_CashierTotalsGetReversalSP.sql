
SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CashierTotalsGetReversalSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CashierTotalsGetReversalSP]
GO


CREATE PROCEDURE  dbo.DN_CashierTotalsGetReversalSP
            @empeeno int,
            @return int OUTPUT

AS

    SET     @return = 0            --initialise return code

    SELECT ISNULL(ct.id,0)            AS Id,
           ISNULL(ct.datefrom,'')     AS DateFrom,
           ISNULL(ct.dateto,'')       AS DateTo,
           ISNULL(ctb.paymethod,'')   AS PayMethod,
           ISNULL(c.codedescript,'')  AS CodeDescript,
           ISNULL(ctb.difference,0)   AS Difference,
           ISNULL(ctb.systemtotal,0)  AS SystemTotal,
           ISNULL(ctb.deposit,0)      AS Deposit,
           ISNULL(ctb.usertotal,0)    AS UserTotal,
           ISNULL(ctb.reason,'')      AS Reason
    FROM   CashierTotals ct
    LEFT OUTER JOIN CashierTotalsBreakdown ctb ON ctb.cashiertotalid = ct.Id
    LEFT OUTER JOIN Code c ON c.category = 'FPM' AND c.code = ctb.paymethod
    WHERE  ct.id = (SELECT MAX(Id) FROM CashierTotals
                    WHERE  EmpeeNo = @empeeno)
    ORDER BY paymethod ASC

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

