SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_FreeInstalmentSaveSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_FreeInstalmentSaveSP
END
GO


CREATE PROCEDURE DN_FreeInstalmentSaveSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_FreeInstalmentSaveSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Record a free loyalty club instalment/voucher
-- Author       : D Richardson
-- Date         : 27 Mar 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @acctNo            CHAR(12),
    @branchNo          INTEGER,
    @dateIssued        DATETIME,
    @amount            MONEY,
    @giftVoucher       CHAR(1),
    @return            INTEGER OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN

    SET NOCOUNT ON
    SET @return = 0

    INSERT INTO FreeInstalment
        (AcctNo,
         BranchNo,
         DateIssued,
         Amount,
         GiftVoucher)
    VALUES
        (@acctNo,
         @branchNo,
         @dateIssued,
         @amount,
         @giftVoucher)

    SET @return = @@error
    SET NOCOUNT OFF
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
