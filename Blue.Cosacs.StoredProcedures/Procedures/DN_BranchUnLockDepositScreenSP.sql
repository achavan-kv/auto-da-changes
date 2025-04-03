SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchUnLockDepositScreenSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchUnLockDepositScreenSP]
GO

CREATE PROCEDURE 	dbo.DN_BranchUnLockDepositScreenSP
			@branchno smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code
	
	UPDATE	branch
	SET		depositscreenlocked = 0
	WHERE	branchno = @branchno

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

