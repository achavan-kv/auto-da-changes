SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_SetsSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_SetsSaveSP]
GO


CREATE PROCEDURE 	dbo.DN_SetsSaveSP
			@setname varchar(64),
			@setdescript varchar(80),		
			@empeeno int,
			@tname varchar(24),
			@columntype char(1),
			@value money,			-- #13691
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	UPDATE	Sets
	SET		dateamend = GetDate(),
			columntype = @columntype,
			setdescript = @setdescript,
			Value=@value				-- #13691
	WHERE	setname = @setname
	AND		tname = @tname

	
	IF(@@rowcount = 0)
	BEGIN
		INSERT
		INTO	Sets
				(setname, empeeno, tname, dateamend, columntype, setdescript,Value)				-- #13691
		VALUES	(@setname, @empeeno, @tname, GetDate(), @columntype, @setdescript,@value)		-- #13691
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

