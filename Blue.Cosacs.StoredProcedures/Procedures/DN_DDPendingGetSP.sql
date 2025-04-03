SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDPendingGetSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDPendingGetSP
END
GO


CREATE PROCEDURE DN_DDPendingGetSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDPendingGetSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Singapore Direct Debit Extra Payments list
-- Author       : P Njie
-- Date         : 28 June 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @acctno    varchar(12),
	@pending   money OUT,
    @return    integer OUT

AS --DECLARE

        

    /* Calculate the DDPending amount for each mandate */
    SELECT @pending = ISNULL(ROUND(SUM(pay.Amount),2),0)
    FROM   DDMandate man, DDPayment pay 
    WHERE  man.AcctNo = @acctno
    AND    pay.MandateId = man.MandateId 
    AND    ( pay.Status = 'I' OR pay.Status = 'S' 
             OR ( pay.Status = 'R' AND pay.RejectAction = 'R'))


    SET @return = @@ERROR
    RETURN @return

GO
GRANT EXECUTE ON DN_DDPendingGetSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

