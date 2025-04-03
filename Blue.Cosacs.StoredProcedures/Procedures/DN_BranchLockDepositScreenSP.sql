SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchLockDepositScreenSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchLockDepositScreenSP]
GO

CREATE PROCEDURE 	dbo.DN_BranchLockDepositScreenSP
			@branchno smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	DECLARE	@depositscreenlocked bit
	SET		@depositscreenlocked = 0

	/* get update lock */
	UPDATE	branch
	SET		depositscreenlocked = depositscreenlocked
	WHERE	branchno = @branchno

	SELECT	@depositscreenlocked = depositscreenlocked 
	FROM		branch
	WHERE	branchno = @branchno

	IF(@depositscreenlocked = 1)
	BEGIN
		RAISERROR('Deposit screen is locked for branch %d', 16, 1, @branchno)
	END
	ELSE
	BEGIN
		UPDATE	branch
		SET		depositscreenlocked = 1
		WHERE	branchno = @branchno
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

