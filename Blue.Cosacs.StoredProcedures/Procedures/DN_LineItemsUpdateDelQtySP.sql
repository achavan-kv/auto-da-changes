SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_LineItemsUpdateDelQtySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_LineItemsUpdateDelQtySP]
GO

CREATE PROCEDURE 	dbo.DN_LineItemsUpdateDelQtySP
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_LineItemsUpdateDelQtySP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get Delivery and Schedule details  
-- Author       : ??
-- Date         : ??
--
-- This procedure will update the delivery quantity on the Lineitem Details
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
-- 20/05/10 jec UAT160 Only update delqty if not > ordered quantity
-- 17/05/11 ip  RI Integration changes - CR1212 - #3627 - Changed joins to use ItemID rather than ItemNo and ParentItemID rather than ParentItemNo
-- ================================================
	-- Add the parameters for the stored procedure here
						@acctno varchar(12),
			@agreementno int,
			@stocklocn smallint,
			--@itemno varchar(8),
			@itemID int,							--IP - 17/05/11 - CR1212 - #3627
			@contractno varchar(10),
			@qty float,
			--@parentItemNo VARCHAR(8),
			@parentItemID int,						--IP - 17/05/11 - CR1212 - #3627
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	lineitem
	SET		delqty = delqty + @qty
	WHERE	acctno = @acctno
	AND		agrmtno = @agreementno
	AND		stocklocn = @stocklocn
	--AND		itemno = @itemno
	AND		ItemID = @itemID						--IP - 17/05/11 - CR1212 - #3627
	AND		contractno = @contractno
	--AND parentItemNo = @parentItemNo
	AND		ParentItemID = @parentItemID			--IP - 17/05/11 - CR1212 - #3627
	and delqty + @qty <= quantity				-- UAT160 -- only update if not > order quantity 

	IF(@@ROWCOUNT = 0 AND @qty = 0)
	BEGIN
		UPDATE lineitem
		SET delqty = 0
		WHERE	acctno = @acctno
		AND		agrmtno = @agreementno
		AND		stocklocn = @stocklocn
		--AND		itemno = @itemno
		AND		ItemID = @itemID					--IP - 17/05/11 - CR1212 - #3627
		AND		contractno = @contractno
		--AND parentItemNo = @parentItemNo
		AND		ParentItemID = @parentItemID		--IP - 17/05/11 - CR1212 - #3627	
		AND quantity = 0	 
	END

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

-- End End End End End End End End End End End End End End End End End End End End End End End End End End End