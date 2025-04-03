SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_StaffUpdateAllocCountSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_StaffUpdateAllocCountSP]
GO

CREATE PROCEDURE 	dbo.DN_StaffUpdateAllocCountSP
			@empeeno int,
			@count int,
			@return int OUTPUT

AS
	SET 	@return = 0			--initialise return code

	UPDATE	courtsperson
	SET		alloccount = alloccount + @count
	WHERE 	userid = @empeeno

	SET @return = @@error
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

