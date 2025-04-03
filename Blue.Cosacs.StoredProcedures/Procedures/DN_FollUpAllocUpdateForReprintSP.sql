SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocUpdateForReprintSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocUpdateForReprintSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocUpdateForReprintSP
			@acctno varchar(12),
			@empeeno int,
			@datealloc datetime,
			@batch bit,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	IF (@batch = 1) 
	BEGIN
		UPDATE	follupalloc
		SET	datealloc = null
		WHERE	empeeno = @empeeno
		AND	datealloc = @datealloc
	END
	ELSE
	BEGIN
		UPDATE 	follupalloc
		SET 	datealloc = null
		WHERE	acctno = @acctno
		AND	empeeno = @empeeno
		AND	datealloc = @datealloc
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