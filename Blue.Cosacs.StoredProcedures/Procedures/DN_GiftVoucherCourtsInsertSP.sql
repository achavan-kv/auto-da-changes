SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GiftVoucherCourtsInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GiftVoucherCourtsInsertSP]
GO

CREATE PROCEDURE 	dbo.DN_GiftVoucherCourtsInsertSP
			@reference varchar(20),
			@value money,
			@empeenosold int,
			@dateexpiry datetime,
			@countrycode varchar(2),
			@free bit,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	DECLARE	@voucheracct varchar(12)

	UPDATE	giftvouchercourts
	SET		reference = @reference
	WHERE	reference = @reference

	IF(@@rowcount != 0)
	BEGIN								
		RAISERROR('Gift voucher already exists with reference %s', 16, 1, @reference)
	END
	ELSE
	BEGIN
		SELECT	@voucheracct = giftvoucheraccount 
		FROM		country
		WHERE	countrycode = @countrycode

		INSERT
		INTO		giftvouchercourts
				(reference, value, empeenosold, datesold, empeenoredeemed,	
				dateredeemed, empeenoauth, acctnosold, acctnoredeemed, 
				dateexpiry, writtenoff, datewrittenoff, transrefnoredeemed, free )
		VALUES	(@reference, @value, @empeenosold, getdate(), 0, 
				null, 0, @voucheracct, '', @dateexpiry, 0, null, null, @free )
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

