SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetDetailsDeleteBranchSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetDetailsDeleteBranchSP]
GO


CREATE PROCEDURE 	dbo.DN_SetDetailsDeleteBranchSP
			@setname varchar(64),
			@tname varchar(24),
			@return int OUTPUT
AS

	SET @return = 0

	DELETE FROM SetByBranch
	WHERE  setname = @setname
	AND    tname   = @tname

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

