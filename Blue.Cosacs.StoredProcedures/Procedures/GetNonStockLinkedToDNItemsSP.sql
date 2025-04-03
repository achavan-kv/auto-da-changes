SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'GetNonStockLinkedToDNItemsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE GetNonStockLinkedToDNItemsSP
END
GO


CREATE PROCEDURE GetNonStockLinkedToDNItemsSP
--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : GetNonStockLinkedToDNItemsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : GetNonStockLinkedToDNItemsSP.sql
-- Description  : Retrieve the warranties linked to items that are on a delivery note
--				  which is being deleted from the Delivery Notification screen.
-- Author       : Ilyas Parker
-- Date         : 21st February 2009
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 17/05/11  ip  RI Integration changes - CR1212 - #3627 - Changed joins to join on ItemID, selecting ItemNo as IUPC from stockitem.
--------------------------------------------------------------------------------

    -- Parameters
    @stockLocn        INT,
    @buffNo           INT,
    @return           INT  out

AS


BEGIN

   
    SELECT l.acctno, l.agrmtno, si.IUPC as itemno, l.stocklocn, l.contractno, l.parentitemno, l.ItemID, l.ParentItemID	--IP - 17/05/11 - CR1212 - #3627 - select itemno from stockitem as itemno
	FROM lineitem l, schedule s, stockinfo si				--IP - 17/05/11 - CR1212 - #3627 - added join to stockinfo
	WHERE s.buffno = @buffNo
	AND	  s.stocklocn = @stockLocn
	AND   l.acctno = s.acctno
	AND	  l.agrmtno = s.agrmtno
	AND   l.stocklocn = s.stocklocn
	--AND   l.parentitemno = s.itemno
	AND   l.ParentItemID = s.ItemID			--IP - 17/05/11 - CR1212 - #3627
	AND	  l.ItemID = si.ID				--IP - 17/05/11 - CR1212 - #3627
	AND	  s.loadno != 0



    SET @Return = @@ERROR
    RETURN @Return
END

GO
GRANT EXECUTE ON GetNonStockLinkedToDNItemsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
