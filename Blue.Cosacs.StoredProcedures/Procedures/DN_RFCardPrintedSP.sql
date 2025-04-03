

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_RFCardPrintedSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_RFCardPrintedSP
END
GO


CREATE PROCEDURE DN_RFCardPrintedSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_RFCardPrintedSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update the customers that have had their RF card printed
-- Author       : D Richardson
-- Date         : 19 May 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @return                 INTEGER       OUTPUT

AS  -- DECLARE
    -- Local variables

BEGIN
    SET NOCOUNT ON
    SET @return = 0

	-- Mark the RF Customers as printed
	UPDATE Customer
	SET    RFCardPrinted = 'Y',
	       RFCardSeqNo = RFCardPrint.RFCardSeqNo
	FROM   RFCardPrint
	WHERE  RFCardPrint.CustId = Customer.CustId

	TRUNCATE TABLE RFCardPrint

    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_RFCardPrintedSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
