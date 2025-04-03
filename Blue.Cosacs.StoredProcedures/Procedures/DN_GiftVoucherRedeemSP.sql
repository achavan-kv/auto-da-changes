SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GiftVoucherRedeemSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GiftVoucherRedeemSP]
GO

CREATE PROCEDURE 	dbo.DN_GiftVoucherRedeemSP
			@reference varchar(32),
			@courts smallint,
			@empeenoredeemed int,
			@dateredeemed datetime,
			@empeenoauth int,
			@acctnoredeemed varchar(12),
			@acctnocompany varchar(12),
			@transrefnoredeemed int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF(@courts = 1)
	BEGIN
		UPDATE	GiftVoucherCourts
		SET		empeenoredeemed = @empeenoredeemed,
				dateredeemed = @dateredeemed,	
				empeenoauth = @empeenoauth,
				acctnoredeemed = @acctnoredeemed,
				transrefnoredeemed = @transrefnoredeemed
		WHERE	reference = @reference
	END
	ELSE
	BEGIN
		UPDATE	GiftVoucherOther
		SET		dateredeemed = @dateredeemed,
				acctnoredeemed = @acctnoredeemed,
				transrefnoredeemed = @transrefnoredeemed
		WHERE	reference = @reference
		AND		acctnocompany = @acctnocompany
	END
	
	INSERT INTO GiftVoucherRedeemed
	(
		reference,
		acctnoredeemed,
		transrefnoredeemed
	)
	VALUES
	(
		@reference,
		@acctnoredeemed,
		@transrefnoredeemed
	)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

