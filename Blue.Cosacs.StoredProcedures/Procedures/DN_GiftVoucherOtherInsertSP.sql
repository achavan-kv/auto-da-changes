SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GiftVoucherOtherInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GiftVoucherOtherInsertSP]
GO

CREATE PROCEDURE 	dbo.DN_GiftVoucherOtherInsertSP
			@reference varchar(32),
			@value money,
			@acctnocompany varchar(12),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	giftvoucherother
	SET		value = @value
	WHERE	reference = @reference
	AND		acctnocompany = @acctnocompany

	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO		giftvoucherother
				(reference, value, dateredeemed, acctnoredeemed, acctnocompany, dateconfirmed, datevoided, transrefnoredeemed )
		VALUES	(@reference, @value, null, '', @acctnocompany, null, null, null)
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

