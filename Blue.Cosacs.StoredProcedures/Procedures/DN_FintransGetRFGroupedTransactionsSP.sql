SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_FintransGetRFGroupedTransactionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetRFGroupedTransactionsSP]
GO


CREATE PROCEDURE DN_FintransGetRFGroupedTransactionsSP

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2003 Strategic Thought Ltd.
-- File Name    : DN_FintransGetRFGroupedTransactionsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load RF grouped transactions for a customer
-- Author       : D Richardson
-- Date         : 5 Feb 2003
--
--
-- Change Control
-- --------------
-- Date    ByDescription
-- ----    -------------
--
--
--------------------------------------------------------------------------------

    -- Parameters
    @piCustId   VARCHAR(20),

    @Return     INTEGER OUTPUT

AS
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;

    -- Load RF grouped transactions for a customer
    -- Group by date excluding time, by transaction type and whether printed

    SELECT  CONVERT(SMALLDATETIME, CONVERT(CHAR(10), F.DateTrans, 103), 103) AS DateTrans,
            F.TransTypeCode,
            SUM(F.TransValue) AS TransValue,
            F.TransPrinted
    FROM    CustAcct CA, Acct A, Agreement AG, Fintrans F
    WHERE   CA.CustId       = @piCustId
    AND     CA.HldOrJnt     = 'H'
    AND     A.AcctNo        = CA.AcctNo
    AND     A.AcctType      = 'R'
    --AND     A.OutStBal      > 0
    AND     AG.AcctNo       = A.AcctNo
    AND     AG.DeliveryFlag = 'Y'
    AND     F.AcctNo        = A.AcctNo
    GROUP BY    CONVERT(SMALLDATETIME, CONVERT(CHAR(10), F.DateTrans, 103), 103),
                F.TransTypeCode,
                F.TransPrinted
    ORDER BY 1,2,3

    SET @Return = @@ERROR

END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

