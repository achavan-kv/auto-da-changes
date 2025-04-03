SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_KitLineItemSavePTSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_KitLineItemSavePTSP]
GO


CREATE PROCEDURE 	dbo.DN_KitLineItemSavePTSP
			@AcctNo varchar(12),
			@KitNo varchar(8),
			@StockLocn smallint,
			@Quantity float,
			@DSPrice float,
			@DSOrdVal float,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	KitLineItem
	SET		Acctno = @AcctNo,
			KitNo = @KitNo,
			StockLocn = @StockLocn,	
			Quantity = Quantity + @Quantity,
			DSPrice = @DSPrice,
			DSOrdVal = @DSOrdVal
	WHERE	Acctno = @AcctNo
	AND		KitNo = @KitNo
	AND		StockLocn = @StockLocn

	IF(@@rowcount=0)
	BEGIN
		INSERT
		INTO		KitLineItem
				(AcctNo, KitNo, StockLocn, Quantity, DSPrice, DSOrdVal)
		VALUES	(@AcctNo, @KitNo, @StockLocn, @Quantity, @DSPrice, @DSOrdVal)
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

