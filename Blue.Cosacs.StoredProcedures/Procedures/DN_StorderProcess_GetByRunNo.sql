-- Author:  John Croft  
-- Date:    March 2006

/*    This procedure will get the Standing Order Process transactions for a specific RunNo
       

*/
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StorderProcess_GetByRunNo]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StorderProcess_GetByRunNo]
GO


CREATE PROCEDURE DN_StorderProcess_GetByRunNo
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_StorderProcess_GetByRunNo.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : DN_StorderProcess_GetByRunNo 
-- Author       : John Croft
-- Date         : 2007
--
-- This procedure will validate the payment files from banks.
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 06/09/10  ip CR1112 - Tallyman Interest Charges
-- 06/03/12  ip #9665 - LW74273 - Standing Order Postings (rebate)
-- 03/04/12  ip/jc #9665 - LW74273 - Need to calculate Rebate 
-- 24/01/13  ip #11729 - LW75565 - Error caused when running End Of Day as SettlementFigure
--				was returned as NULL for non existing accounts.
-- ================================================

    -- Parameters
    @RunNo	int,
    @Return         INTEGER OUTPUT

AS
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;

    -- Get Standing Order Process transactions
    
    SELECT Runno,
           s.Acctno,
           TransRefNo,
           round(TransValue,2) as Transvalue,
           DateTrans,
           Error,
           s.BankName,
           s.IsInterest,													--IP - 06/09/10 - CR1112 
           cast(0 as decimal) as Rebate,			--IP/JC - 03/04/12 - #9665
           --a.outstbal as SettlementFigure			--IP/JC - 03/04/12 - #9665 
           ISNULL(a.outstbal,0) as SettlementFigure,	--IP - 24/01/13 - #11729 - LW75565		--IP/JC - 03/04/12 - #9665 
		   sc.PaymentMethod
           
    FROM   StOrderProcess s LEFT JOIN acct a on s.acctno = a.acctno		--IP - 06/03/12 - #9665 - LW74273
	INNER JOIN
		stordercontrol sc on s.bankname = sc.bankname
    --LEFT JOIN Rebates r on s.acctno = r.acctno							--IP - 06/03/12 - #9665 - LW74273
    WHERE  RunNo = @RunNo
    ORDER BY RunNo,BankName
    
    SET @Return = @@ERROR
    
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO


