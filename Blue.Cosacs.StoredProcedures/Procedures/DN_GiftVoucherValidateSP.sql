SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GiftVoucherValidateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GiftVoucherValidateSP]
GO

CREATE PROCEDURE 	dbo.DN_GiftVoucherValidateSP
			@reference varchar(32),
			@courts smallint,
			@value money OUT,
			@expiry datetime OUT,
			@includeredeemed bit,
			@redeemed datetime OUT, 
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF(@courts = 1)				-- this is a courts voucher
	BEGIN
		SELECT	@value = isnull(value, 0),
				@expiry = dateexpiry,
				@redeemed = dateredeemed
		FROM		GiftVoucherCourts
		WHERE	reference = @reference		
		AND		(dateredeemed is null OR @includeredeemed = 1)
	END
	ELSE					-- this is not a courts voucher
	BEGIN
		SELECT	@value = isnull(value, 0),
				@redeemed = dateredeemed
		FROM		GiftVoucherOther
		WHERE	reference = @reference	
		AND		(dateredeemed is null OR @includeredeemed = 1)
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

