SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AccountGetLastCashierDifferenceSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountGetLastCashierDifferenceSP]
GO

CREATE PROCEDURE dbo.DN_AccountGetLastCashierDifferenceSP
    @piEmpeeNo int,
    @poLastDifference MONEY OUTPUT,
    @Return int OUTPUT

AS

    SET     @return = 0

    SELECT  @poLastDifference = difference
    FROM    CashierTotals
    WHERE   EmpeeNo = @piEmpeeNo
    AND     DateTo = (SELECT MAX(DateTo) FROM CashierTotals WHERE EmpeeNo = @piEmpeeNo)
    
    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

