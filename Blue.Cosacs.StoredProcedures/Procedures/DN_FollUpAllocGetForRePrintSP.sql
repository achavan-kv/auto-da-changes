SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocGetForRePrintSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocGetForRePrintSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocGetForRePrintSP
			@empeeno int,
			@months int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	SELECT	empeeno,
               	datealloc,
               	count(*) as total
	FROM 	FOLLUPALLOC
	WHERE 	empeeno = @empeeno 
    	AND   	(datealloc != '' AND datealloc is not null)
	AND 	DATEADD(month, @months, datealloc) > GETDATE()
	and	datedealloc is null		-- only select accounts that are currenty allocated (UAT 5.0 iss 20 jec 28/03/07)
    	GROUP BY datealloc, empeeno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END


GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

