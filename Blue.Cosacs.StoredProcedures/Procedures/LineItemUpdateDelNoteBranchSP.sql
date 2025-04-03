
/****** Object:  StoredProcedure [dbo].[LineItemUpdateDelNoteBranchSP]    Script Date: 07/26/2007 10:03:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF  EXISTS (SELECT * FROM sysobjects WHERE id = OBJECT_ID('[dbo].[LineItemUpdateDelNoteBranchSP]') AND type in ('P', 'PC'))
DROP PROCEDURE [dbo].[LineItemUpdateDelNoteBranchSP]
GO
-- =============================================
-- Author:		Jez Hemans
-- Create date: 25/07/2007
-- Description:	69105 The lineitem table needs to be updated so that the delnotebranch field is the same as the return stock location for returns
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 03/08/11  IP  RI - System Integration changes
-- 14/10/11  jec #8409 LW74095 - CoSACS to RI Export - Committed Stock
-- =============================================
CREATE PROCEDURE [dbo].[LineItemUpdateDelNoteBranchSP]
	@acctno VARCHAR(12),
    --@itemno VARCHAR(8),
    @itemID INTEGER,													--IP - 03/08/11 - RI
    @retstocklocn SMALLINT,
    @return INT OUTPUT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    SET 	@return = 0
    
    UPDATE lineitem
    SET delnotebranch = @retstocklocn
    --WHERE acctno = @acctno AND itemno = @itemno
    WHERE acctno = @acctno AND ItemID = @itemID						--IP - 03/08/11 - RI
    
    -- Update lineitemaudit delnote branch			-- #8409
    UPDATE lineitemaudit
    SET delnotebranch = @retstocklocn
    from lineitemaudit la INNER JOIN lineitem li on la.acctno = li.acctno and la.agrmtno = li.agrmtno and la.ItemID = li.ItemID    
    WHERE li.acctno = @acctno AND li.ItemID = @itemID
    and source not in ('NewAccount','Revise') 
    and la.Datechange> li.datereqdel
		
    IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
END

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End End