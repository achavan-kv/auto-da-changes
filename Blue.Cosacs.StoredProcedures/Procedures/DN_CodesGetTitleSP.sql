SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetTitleSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetTitleSP]
GO






CREATE PROCEDURE 	dbo.DN_CodesGetTitleSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	upper(title) as title
	FROM		title
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

