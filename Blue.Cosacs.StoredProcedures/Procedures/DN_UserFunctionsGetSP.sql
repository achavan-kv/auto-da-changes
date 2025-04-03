SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_UserFunctionsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_UserFunctionsGetSP]
GO





CREATE PROCEDURE 	dbo.DN_UserFunctionsGetSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	Id,
			Name
	FROM		Admin.Permission
	ORDER BY	Name

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END




GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

