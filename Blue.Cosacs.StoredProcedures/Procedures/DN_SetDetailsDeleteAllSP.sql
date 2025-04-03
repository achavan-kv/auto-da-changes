SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetDetailsDeleteAllSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetDetailsDeleteAllSP]
GO


CREATE PROCEDURE 	dbo.DN_SetDetailsDeleteAllSP
			@setname varchar(64),
			@tname varchar(24),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code


	DELETE FROM SetDetails
	WHERE setname = @setname
	  AND tname = @tname

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

