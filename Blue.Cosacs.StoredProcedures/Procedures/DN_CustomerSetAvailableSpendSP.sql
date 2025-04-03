SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerSetAvailableSpendSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerSetAvailableSpendSP]
GO


CREATE PROCEDURE 	dbo.DN_CustomerSetAvailableSpendSP
			@custid varchar(20),
			@available money,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	customer
	SET	availablespend = @available
	WHERE	custid = @custid

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

