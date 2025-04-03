SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetDetailsInsertSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetDetailsInsertSP]
GO


CREATE PROCEDURE 	dbo.DN_SetDetailsInsertSP
			@setname varchar(64),
			@data varchar(32),
			@tname varchar(24),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code


	INSERT
	INTO	SetDetails
			(setname, data, tname)
	VALUES	(@setname, @data, @tname)

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

