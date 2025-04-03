SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BailiffCommissionUpdateStatusSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BailiffCommissionUpdateStatusSP]
GO

CREATE PROCEDURE 	dbo.DN_BailiffCommissionUpdateStatusSP
			@empeeno int,
			@transrefno int,
			@datetrans datetime,
			@status char(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	UPDATE	bailiffcommn
	SET	status = @status
	WHERE	empeeno = @empeeno
	AND	transrefno = @transrefno
	AND	datetrans = @datetrans
	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO	