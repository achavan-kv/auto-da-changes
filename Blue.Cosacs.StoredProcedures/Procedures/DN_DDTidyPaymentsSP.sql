
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDTidyPaymentsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDTidyPaymentsSP
END
GO


CREATE PROCEDURE DN_DDTidyPaymentsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDTidyPaymentsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Delete data over two months old from the DDPayment table
-- Author       : D Richardson
-- Date         : 6 April 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @return             INTEGER OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* Delete payments before the previous two months */
    DELETE  FROM DDPayment WITH (TABLOCKX)
    WHERE   (   Status = 'C'     -- $DDST_Complete
             OR Status = 'X'     -- $DDST_Cancelled
             OR (Status = 'R' AND RejectAction = 'N'))    -- $DDST_Rejected AND $DDRA_NotRepresent
    AND     DATEDIFF(Month, DateEffective, GETDATE()) > 2


    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDTidyPaymentsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
