SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GiftVoucherPopulateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GiftVoucherPopulateSP]
GO

CREATE PROCEDURE 	dbo.DN_GiftVoucherPopulateSP
			@reference varchar(32),
			@courts smallint,
			@value money OUT,
			@empeenosold int OUT,
			@datesold datetime OUT,
			@empeenoredeemed int OUT,
			@dateredeemed datetime OUT,
			@empeenoauth int OUT,
			@acctnosold varchar(12) OUT,
			@acctnoredeemed varchar(12) OUT,
			@dateexpiry datetime OUT,
			@writtenoff bit OUT,
			@datewrittenoff datetime OUT,
			@acctnocompany varchar(12),
			@dateconfirmed datetime OUT,
			@datevoided datetime OUT,
			@transrefnoredeemed int OUT,
			@free bit OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF(@courts = 1)
	BEGIN
		SELECT	@value = value,
				@empeenosold = empeenosold,
				@datesold = datesold,
				@empeenoredeemed = empeenoredeemed,
				@dateredeemed = dateredeemed,
				@empeenoauth = empeenoauth,
				@acctnosold = acctnosold,
				@acctnoredeemed = acctnoredeemed,
				@dateexpiry = dateexpiry,
				@writtenoff = writtenoff,
				@datewrittenoff = datewrittenoff,
				@transrefnoredeemed = transrefnoredeemed,
				@free = free
		FROM		GiftVoucherCourts
		WHERE	reference = @reference
	END
	ELSE
	BEGIN
		SELECT	@value = value,
				@dateredeemed = dateredeemed,	
				@acctnoredeemed = acctnoredeemed,
				@dateconfirmed = dateconfirmed,
				@datevoided = datevoided,
				@transrefnoredeemed = transrefnoredeemed
		FROM		GiftVoucherOther
		WHERE	reference = @reference	
		AND		acctnocompany = @acctnocompany
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

