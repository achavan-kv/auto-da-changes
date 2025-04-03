SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_CodesGetWriteOffCategoriesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_CodesGetWriteOffCategoriesSP]
GO

CREATE PROCEDURE 	dbo.DN_CodesGetWriteOffCategoriesSP
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	code,
			codedescript,
			category
	FROM		code
	WHERE	category IN ('BDP', 'BDR', 'BDM', 'BDD','TBD')	 --TBD Tallyman write off codes cr 724				

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

