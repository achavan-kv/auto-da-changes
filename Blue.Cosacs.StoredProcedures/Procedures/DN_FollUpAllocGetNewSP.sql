SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_FollUpAllocGetNewSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_FollUpAllocGetNewSP]
GO

CREATE PROCEDURE 	dbo.DN_FollUpAllocGetNewSP
			@empeeno int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	AcctNo,
			AllocNo,
			EmpeeNo
	FROM 		FOLLUPALLOC
	WHERE 	empeeno = @empeeno
	AND  		DateAlloc IS NULL
	AND		DateDeAlloc IS NULL

	SET 	@return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

