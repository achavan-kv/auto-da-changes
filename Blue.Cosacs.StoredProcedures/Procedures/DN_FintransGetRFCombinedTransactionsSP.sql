SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_FintransGetRFCombinedTransactionsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FintransGetRFCombinedTransactionsSP]
GO


CREATE PROCEDURE DN_FintransGetRFCombinedTransactionsSP

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2003 Strategic Thought Ltd.
-- File Name    : DN_FintransGetRFCombinedTransactionsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load RF combined transactions for a customer
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

    -- Load RF combined transactions for a customer
    -- Discrete transactions are loaded for each RF account

    SELECT  F.AcctNo,
            F.TransRefNo,
            F.DateTrans,
            F.TransTypeCode,
            F.TransValue,
            ISNULL(C.CodeDescript,'') AS PayMethod,
            ISNULL(U.fullname,'') AS EmpeeName,
            F.EmpeeNo,
            F.FtNotes,
            F.TransPrinted
    FROM CustAcct CA, Acct A, Agreement AG, FinTrans F
    LEFT OUTER JOIN Admin.[User] u ON u.id = F.EmpeeNo
    LEFT OUTER JOIN Code C ON CONVERT(VARCHAR,F.PayMethod) = C.Code AND C.category = 'FPM'
    WHERE   CA.CustId       = @piCustId
    AND     CA.HldOrJnt     = 'H'
    AND     A.AcctNo        = CA.AcctNo
    AND     A.AcctType      = 'R'
    --AND     A.OutStBal      > 0
    AND     AG.AcctNo       = A.AcctNo
    AND     AG.DeliveryFlag = 'Y'
    AND     F.AcctNo        = A.AcctNo
    ORDER BY 1,2,3

    SET @Return = @@ERROR

END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

