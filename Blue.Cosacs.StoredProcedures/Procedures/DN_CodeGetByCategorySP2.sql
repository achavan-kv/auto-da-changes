SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodeGetByCategorySP2]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodeGetByCategorySP2]
GO


CREATE PROCEDURE 	dbo.DN_CodeGetByCategorySP2
			@category1 varchar(4),
			@category2 varchar(4),
			@flag varchar(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	code,
			codedescript,
			additional
	FROM		code
	WHERE	category in ( @category1, @category2 )
	AND		statusflag = @flag
	--AND		code != ''
	order by sortorder	
	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

