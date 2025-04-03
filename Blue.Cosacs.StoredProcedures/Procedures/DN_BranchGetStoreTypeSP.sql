SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_BranchGetStoreTypeSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_BranchGetStoreTypeSP]
GO

CREATE PROCEDURE 	dbo.DN_BranchGetStoreTypeSP
			@branch smallint,
			@storetype VARCHAR(2) OUT,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	@storetype = storetype
	FROM	branch
	WHERE	branchno = @branch

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

