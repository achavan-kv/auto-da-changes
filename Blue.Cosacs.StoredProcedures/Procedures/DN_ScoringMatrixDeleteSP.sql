SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id(N'[dbo].[DN_TermsTypeMatrixDeleteSP]') and OBJECTPROPERTY(id, N'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeMatrixDeleteSP]
GO


if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScoringMatrixDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringMatrixDeleteSP]
GO

CREATE PROCEDURE 	dbo.DN_ScoringMatrixDeleteSP
			@country char(2),
			@region varchar(3),
			@scoretype CHAR(1),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	DECLARE	@mostrecent datetime

	SELECT	@mostrecent = MAX(dateimported)
	FROM	rfcreditlimit
	WHERE	countrycode = @country
	AND	region = @region
	AND scoretype = @scoretype --SC CR1034 Behavioural Scoring 15/02/2010 

	DELETE	
	FROM		rfcreditlimit
	WHERE		countrycode = @country
	AND		region = @region
	AND		dateimported = @mostrecent
	AND scoretype = @scoretype

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

