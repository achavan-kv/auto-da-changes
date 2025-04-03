SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetDetailsInsertBranchSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetDetailsInsertBranchSP]
GO


CREATE PROCEDURE 	dbo.DN_SetDetailsInsertBranchSP
			@setname varchar(64),
			@tname varchar(24),
			@branchno smallint,
			@return int OUTPUT

AS

	SET @return = 0

    INSERT INTO SetByBranch
        (SetName, TName, BranchNo)
    VALUES (@setname, @tname, @branchno)

    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

