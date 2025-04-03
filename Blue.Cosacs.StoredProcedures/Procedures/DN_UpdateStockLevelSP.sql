SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO
if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UpdateStockLevelSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UpdateStockLevelSP]
GO

CREATE PROCEDURE 	dbo.DN_UpdateStockLevelSP
			@itemno varchar(8),
			@stocklocn smallint,
			@quantity float,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	-- If Mauritius have applied Oracle upgrade then update the Stockquantity table instead of stockitem
	UPDATE StockQuantity
	SET		qtyAvailable= qtyAvailable+ @quantity,
	dateupdated = GETDATE(),
	LastOperationSource = '' 
	WHERE	stocklocn = @stocklocn
	AND		itemno = @itemno
	-- otherwise update the stockitem table. 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

