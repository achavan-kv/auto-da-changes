SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StockItemMaintainStockSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StockItemMaintainStockSP]
GO

CREATE PROCEDURE 	dbo.DN_StockItemMaintainStockSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_StockItemMaintainStockSP
--
-- 
-- Change Control
-----------------
-- 05/07/11 IP - CR1212 - RI - #3976 - Update purchaseorderoutstanding table. Item was being removed
--			     from New Sales screen, stock qtyAvailable was increased, but quantityavailable on 
--				 purchaseorderoutstanding was not being updated.
-- =============================================
			@acctno varchar(12),
			--@itemno varchar(8),
			@itemID int,				--IP - 20/05/11 - CR1212 - RI - #3664
			@stocklocn smallint,
			@quantity float,
			@agreementno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @diff float,
			@stockavailable int
		
	SET	@diff = @quantity
	
	--IP - 05/07/11 - CR1212 - RI - #3976 - Get the qtyAvailable before update
	select @stockavailable = qtyAvailable from stockquantity where stocklocn = @stocklocn and ID = @itemID

	SELECT	@diff = @quantity - quantity
	FROM		lineitem_amend
	WHERE	acctno = @acctno
	--AND		itemno = @itemno
	AND		ItemID = @itemID			--IP - 20/05/11 - CR1212 - RI - #3664
	AND		stocklocn = @stocklocn
	AND		agrmtno = @agreementno

	--UPDATE	stockitem
	--SET		stockfactavailable = stockfactavailable - @diff
	--WHERE	stocklocn = @stocklocn
	----AND		itemno = @itemno
	--AND		ItemID = @itemID			--IP - 20/05/11 - CR1212 - RI - #3664
	
	
	UPDATE	stockquantity									--IP - 06/09/11 - RI  - Replaces the above
	SET		qtyAvailable = qtyAvailable - @diff
	WHERE	stocklocn = @stocklocn
	--AND		itemno = @itemno
	AND		ID = @itemID			--IP - 20/05/11 - CR1212 - RI - #3664
	
	--IP - 05/07/11 - CR1212 - RI - #3976 - Update purchaseorderoutstanding
    IF exists(select * from purchaseorderoutstanding
				where ItemID = @itemID
				and stocklocn = @stocklocn
				and @stockavailable <=0)
	begin
		UPDATE purchaseorderoutstanding
		SET		quantityavailable = quantityavailable - isnull(@diff,0)
		WHERE	stocklocn = @stocklocn
		AND		ItemID = @itemId
	end
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

