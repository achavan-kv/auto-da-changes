SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScoringRulesSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringRulesSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_ScoringRulesSaveSP
			@country char(2),
			@rules ntext,
			@dateImported datetime,
			@importedBy int,
			@region varchar(3),
			@newimport bit,
			@filename varchar(100),
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF(@newimport = 1)
	BEGIN
		DELETE
		FROM	ScoringRules
		WHERE	CountryCode = @country
		AND	Region = @region
		AND	DateImported = @dateImported
	
		INSERT	
		INTO	ScoringRules	(CountryCode, RulesXML, DateImported, ImportedBy, Region, FileName)
		VALUES	(@country, @rules, @dateImported, @importedBy, @region, @filename)
	END
	ELSE
	BEGIN		/* if it's not a new import just update the most recently imported */
		UPDATE	ScoringRules
		SET	RulesXML = @rules,
			ImportedBy = @importedBy
		WHERE	CountryCode = @country
		AND	Region = @region
		AND	DateImported = (	SELECT	MAX(DateImported)
								FROM	ScoringRules
								WHERE	CountryCode = @country
								AND		Region = @region	)		
		
		IF(@@rowcount=0)
		BEGIN
			INSERT	
			INTO	ScoringRules	(CountryCode, RulesXML, DateImported, ImportedBy, Region, FileName)
			VALUES	(@country, @rules, @dateImported, @importedBy, @region, @filename)
		END	
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

