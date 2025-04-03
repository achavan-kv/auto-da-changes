SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CustomerDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CustomerDeleteSP]
GO

CREATE PROCEDURE 	dbo.DN_CustomerDeleteSP
			@custid varchar(20),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM		customer
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

