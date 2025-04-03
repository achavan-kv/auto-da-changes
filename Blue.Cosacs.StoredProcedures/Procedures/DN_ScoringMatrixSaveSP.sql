SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScoringMatrixSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringMatrixSaveSP]
GO




CREATE PROCEDURE 	dbo.DN_ScoringMatrixSaveSP
			@filename char(120),
			@country char(2),
			@scoretype CHAR(1),
			@score int,
			@flimit money,
			@elimit money,
			@region varchar(3),
			@income money,
			@user int,
			@dateimported datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DELETE
	FROM		rfcreditlimit 
	WHERE		countrycode = @country
	AND		score = @score
	AND		region = @region
	AND		income = @income
	AND     scoretype = @scoretype             --SC CR1034 Behavioural Scoring 15/02/2010 
	AND		dateimported = @dateimported

	INSERT
	INTO	rfcreditlimit
			(filename,
			countrycode,
			scoretype,  --SC CR1034 Behavioural Scoring 15/02/2010 
			score,
			furnlimit,
			eleclimit,
			region,
			income, 
			importedby,
			dateimported )
	VALUES	(@filename,
	        @country,
	        @scoretype,
			@score,
			@flimit,
			@elimit,
			@region,
			@income,
			@user,
			@dateimported )

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

