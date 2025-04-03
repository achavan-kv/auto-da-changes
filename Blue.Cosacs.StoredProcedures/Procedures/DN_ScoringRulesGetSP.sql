SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON
GO


if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScoringRulesGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringRulesGetSP]
GO


CREATE PROCEDURE 	dbo.DN_ScoringRulesGetSP
			@country char(2),
			@scoretype char(1),
			@region varchar(3),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SET ARITHABORT ON -- Need to do this so XML in subprocedure does not error.
	
	EXEC DN_ScoringRulesGet_InnerSP @country = @country, 
	    @scoretype = @scoretype, 
	    @region = @region 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
