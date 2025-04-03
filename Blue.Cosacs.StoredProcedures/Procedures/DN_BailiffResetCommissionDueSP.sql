SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BailiffResetCommissionDueSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailiffResetCommissionDueSP]
GO

CREATE PROCEDURE 	dbo.DN_BailiffResetCommissionDueSP
			@empeeno int,
			@commvalue money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	UPDATE	courtsperson 
	SET		commndue = 0,
			lstcommn = @commvalue
	WHERE	userid	= @empeeno
	
	SET @return = @@error
	
	IF(@return = 0)
	BEGIN
		INSERT INTO CommissionPaid(empeeno, datepaid, amount)
		VALUES(@empeeno, GETDATE(), @commvalue)
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