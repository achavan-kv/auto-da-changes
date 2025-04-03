SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchGetTransRefNoSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchGetTransRefNoSP]
GO






CREATE PROCEDURE 	dbo.DN_BranchGetTransRefNoSP
			@branch smallint,
			@required int,
			@transno int OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

--	BEGIN TRANSACTION

		UPDATE	branch
		SET		@transno = hirefno = hirefno + @required
		WHERE	branchno = @branch

--		SELECT	@transno = hirefno
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

