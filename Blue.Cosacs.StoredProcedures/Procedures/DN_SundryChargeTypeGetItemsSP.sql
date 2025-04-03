SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SundryChargeTypeGetItemsSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SundryChargeTypeGetItemsSP]
GO

CREATE PROCEDURE 	dbo.DN_SundryChargeTypeGetItemsSP
			@acctType varchar(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	chargecode,
			amount,
			ItemID
	FROM	sundchgtyp
	WHERE	accttype = @acctType

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

