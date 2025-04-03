SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GiftVoucherVoidOtherSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GiftVoucherVoidOtherSP]
GO

CREATE PROCEDURE 	dbo.DN_GiftVoucherVoidOtherSP
			@acctnoredeemed varchar(12),
			@refno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	giftvoucherother
	SET		datevoided = getdate()
	WHERE	acctnoredeemed = @acctnoredeemed
	AND		transrefnoredeemed = @refno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

