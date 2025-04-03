SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchGetBuffNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchGetBuffNoSP]
GO






CREATE PROCEDURE 	dbo.DN_BranchGetBuffNoSP
			@branch smallint,
			@buffno int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
--    BEGIN TRANSACTION

		UPDATE	branch
		SET		@buffno = hibuffno = hibuffno + 1
		WHERE	branchno = @branch

--		SELECT	@buffno = hibuffno
--		FROM		branch
--		WHERE	branchno = @branch
		
--	COMMIT	

--	IF (@@error != 0)
--	BEGIN
--		SET @return = @@error
--	END





GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

