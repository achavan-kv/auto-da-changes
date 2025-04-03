SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetsForTnameBranchGetAllSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetsForTnameBranchGetAllSP]
GO

CREATE PROCEDURE 	dbo.DN_SetsForTnameBranchGetAllSP
			@tname varchar(24),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	sb.branchno,
	        s.setname,
	        s.setdescript,
	        s.empeeno,
			s.dateamend,
			s.columntype
	FROM 	Sets s, SetByBranch sb
	WHERE 	s.tname = @tname
	AND     sb.tname = s.tname
	AND     sb.setname = s.setname
	ORDER BY sb.branchno, s.setname ASC

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

