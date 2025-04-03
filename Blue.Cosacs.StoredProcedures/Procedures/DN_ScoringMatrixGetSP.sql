SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScoringMatrixGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringMatrixGetSP]
GO




CREATE PROCEDURE 	dbo.DN_ScoringMatrixGetSP
			@country char(2),
			@region varchar(3),
			@scoretype CHAR(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	countrycode,
			score,
			income,
			furnlimit,
			eleclimit,
			region
	FROM		rfcreditlimit
	WHERE	countrycode = @country
	AND		region = @region
	AND scoretype = @scoretype
	AND	dateimported = 
	--IP - 09/04/10 - CR1034 - Removed
		--(SELECT	MAX(dateimported)
		--FROM	rfcreditlimit R
		--WHERE	R.countrycode = @country
		--AND	R.region = @region
		--AND R.scoretype = rfcreditlimit.scoretype) --SC CR1034 Behavioural Scoring 15/02/2010 
		
		(SELECT	MAX(dateimported)
		FROM	rfcreditlimit R
		WHERE	R.countrycode = @country
		AND	R.region = @region
		AND R.scoretype = rfcreditlimit.scoretype) --SC CR1034 Behavioural Scoring 15/02/2010 

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
