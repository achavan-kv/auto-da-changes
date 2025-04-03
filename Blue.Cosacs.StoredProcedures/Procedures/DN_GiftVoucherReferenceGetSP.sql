SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_GiftVoucherReferenceGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_GiftVoucherReferenceGetSP]
GO

CREATE PROCEDURE 	dbo.DN_GiftVoucherReferenceGetSP
			@acctnoredeemed varchar(12),
			@refno int,
			@reference varchar(20) OUTPUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@reference = reference
	FROM		giftvouchercourts
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

