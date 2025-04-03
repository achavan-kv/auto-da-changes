SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DeliveryGetNonStockValueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DeliveryGetNonStockValueSP]
GO

CREATE PROCEDURE 	dbo.DN_DeliveryGetNonStockValueSP
			@acctno varchar(12),
			@value money OUT,
			@return int OUTPUT

AS
	SET 	@return = 0			--initialise return code
	SET	@value = 0

	SELECT	@value = sum(OrdVal)
	FROM		LINEITEM L, DELIVERY D
	WHERE	D.AcctNo	= @acctno
	AND		D.AcctNo 	= L.AcctNo
	AND		D.ItemNo 	= L.ItemNo
	AND		D.StockLocn 	= L.StockLocn
	AND		D.AgrmtNo 	= L.AgrmtNo
	AND		L.Price		= 0

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

