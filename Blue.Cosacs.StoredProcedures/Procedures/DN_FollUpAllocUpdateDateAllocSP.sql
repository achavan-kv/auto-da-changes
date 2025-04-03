SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocUpdateDateAllocSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocUpdateDateAllocSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocUpdateDateAllocSP
			@empeeno int,
			@return int OUTPUT

AS
	SET 	@return = 0			--initialise return code
	
	UPDATE	FOLLUPALLOC 
	SET		datealloc = getdate()
	WHERE	empeeno = @empeeno
	AND		datealloc is null
	AND		datedealloc is null

	SET	@return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

