SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_RemoveRefCodeCRSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_RemoveRefCodeCRSP]
GO


CREATE PROCEDURE DN_RemoveRefCodeCRSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS Transact ? 2002 Strategic Thought Ltd.
-- File Name    : DN_RemoveRefCodeCRSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Remove carriage returns from Warranty Band ref codes
-- Author       : D Richardson
-- Date         : 17 Jan 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 09/04/03 Alex Ayscough input parameter defaulted to zero in case not supplied
-- 09/09/11 IP   RI - #4690 - UAT35 - Can no longer update the StockItem view, therefore update the table StockPrice
-- 02/02/12 jec #9568 LW74644 - B'dos - Growth of the Live DB
--------------------------------------------------------------------------------

    -- Parameters
     @return int= 0 output

AS -- DECLARE
    -- Local variables

BEGIN

	SET @return = 0;

    --UPDATE StockItem
    UPDATE StockPrice											--IP - 09/09/11 - RI - #4690 - UAT35
    SET    RefCode = REPLACE(RefCode, CHAR(13), CHAR(32))
    where  Refcode like '%' + CHAR(13) + '%'		-- #9568 only update if refcode contains Carriage return
    
    UPDATE WarrantyBand
    SET    RefCode = REPLACE(RefCode, CHAR(13), CHAR(32))
    where  Refcode like '%' + CHAR(13) + '%'		-- #9568 only update if refcode contains Carriage return
    
    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

