SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetsForTnameBranchGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetsForTnameBranchGetSP]
GO

CREATE PROCEDURE 	dbo.DN_SetsForTnameBranchGetSP
			@tname varchar(24),
			@branchno smallint,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	s.setname,
	        s.setdescript,
	        s.empeeno,
			s.dateamend,
			s.columntype
	FROM 	Sets s, SetByBranch sb
	WHERE 	s.tname = @tname
	AND     sb.tname = s.tname
	AND     sb.setname = s.setname
	AND     sb.branchno = @branchno
	ORDER BY s.setname ASC

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

