SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BailiffCommnEraseSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailiffCommnEraseSP]
GO

CREATE PROCEDURE 	dbo.DN_BailiffCommnEraseSP
			@transrefno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE	
	FROM		bailiffcommn
	WHERE	transrefno = @transrefno

	SET	@return = @@rowcount

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

