SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_Fintrans_GetByAcctNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_Fintrans_GetByAcctNo]
GO


CREATE PROCEDURE DN_Fintrans_GetByAcctNo

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS r 2002 Strategic Thought Ltd.
-- File Name    : DN_Fintrans_GetByAcctNo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load financial transactions for immediate delivery
-- Author       : D Richardson
-- Date         : 11 November 2002
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo		VARCHAR(12),

    @Return         INTEGER OUTPUT

AS
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;

    -- Load Financial Transactions
    
    SELECT AcctNo,
           TransRefNo,
           DateTrans,
           TransTypeCode,
           EmpeeNo,
           TransUpdated,
           TransPrinted,
           round(TransValue,2) as Transvalue,
           ChequeNo,
           BankAcctNo,
           BankCode,
           FTNotes,
           isnull(PayMethod,0) as PayMethod,
           Source,
           RunNo
    FROM   Fintrans  
    WHERE  AcctNo = @piAcctNo
    ORDER BY TransRefNo
    
    SET @Return = @@ERROR
    
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

