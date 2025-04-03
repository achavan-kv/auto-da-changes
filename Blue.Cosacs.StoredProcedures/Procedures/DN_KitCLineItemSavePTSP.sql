SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_KitCLineItemSavePTSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_KitCLineItemSavePTSP]
GO


CREATE PROCEDURE 	dbo.DN_KitCLineItemSavePTSP
			@AcctNo varchar(12),
			@KitNo varchar(8),
			@ComponentNo varchar(8),
			@StockLocn smallint,
			@Quantity float,
			@Price float,
			@OrdVal float,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	KitCLineItem
	SET		Acctno = @AcctNo,
			KitNo = @KitNo,
			ComponentNo = @ComponentNo,
			StockLocn = @StockLocn,	
			Quantity = Quantity + @Quantity,
			Price = @Price,
			OrdVal = OrdVal + @OrdVal
	WHERE	Acctno = @AcctNo
	AND		KitNo = @KitNo
	AND		ComponentNo = @ComponentNo
	AND		StockLocn = @StockLocn

	IF(@@rowcount=0)
	BEGIN
		INSERT
		INTO		KitCLineItem
				(AcctNo, KitNo, ComponentNo, StockLocn, Quantity, Price, OrdVal)
		VALUES	(@AcctNo, @KitNo, @ComponentNo, @StockLocn, @Quantity, @Price, @OrdVal)
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

