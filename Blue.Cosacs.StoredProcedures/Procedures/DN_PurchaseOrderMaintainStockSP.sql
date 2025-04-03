SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_PurchaseOrderMaintainStockSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_PurchaseOrderMaintainStockSP]
GO

CREATE PROCEDURE 	dbo.DN_PurchaseOrderMaintainStockSP
-- =============================================
-- Author:		????
-- Create date: ??
-- Title:	DN_PurchaseOrderMaintainStockSP
--
-- 
-- Change Control
-----------------
-- 04/07/11 IP - CR1212 - RI - #3976 - Update StockQuantity - diff
-- =============================================
			@acctno varchar(12),
			@itemId INT,
			@stocklocn smallint,
			@quantity float,
			@agreementno int,
			@purchaseordernumber varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE @diff float
		
	SET	@diff = @quantity

	SELECT	@diff = @quantity - quantity
	FROM	lineitem_amend
	WHERE	acctno = @acctno
	AND	itemId = @itemId
	AND	stocklocn = @stocklocn
	AND	agrmtno = @agreementno

	UPDATE	PurchaseOrderOutstanding
	SET		quantityavailable = quantityavailable - isnull(@diff,0)
	WHERE	stocklocn = @stocklocn
	AND		ItemID = @itemId
	--AND		purchaseordernumber = @purchaseordernumber					--IP - 05/07/11 - no longer used
	
	--IP - 04/07/11 - CR1212 - RI - #3976
	UPDATE StockQuantity
	SET qtyAvailable = qtyAvailable - @diff
	WHERE ID = @itemId
	AND stocklocn = @stocklocn
	

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

