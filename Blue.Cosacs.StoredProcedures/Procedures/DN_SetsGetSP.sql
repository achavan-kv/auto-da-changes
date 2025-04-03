SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetsGetSP]
GO

CREATE PROCEDURE 	dbo.DN_SetsGetSP
            @setname varchar(64),
			@tname varchar(24),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	setname,
	        setdescript,
	        empeeno,
			dateamend,
			columntype,
			Value				-- #13691
	FROM 		Sets 
	WHERE 	tname = @tname
	  AND   setname = @setname

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

