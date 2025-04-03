SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_ScoringAcceptReferSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_ScoringAcceptReferSaveSP]
GO

CREATE PROCEDURE 	dbo.DN_ScoringAcceptReferSaveSP
			@country char(2),
			@region varchar(3),
			@datefrom datetime,
			@acceptscore int,
			@referscore int,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	IF NOT EXISTS(	SELECT 1
					FROM   AcceptReferScore		
					WHERE  CountryCode = @country
					AND    Region = @region)
	BEGIN
		INSERT	
		INTO	AcceptReferScore 
				(countrycode, region, acceptscore, referscore, datefrom)
		VALUES	(@country, @region, @acceptscore, @referscore, @datefrom)
	END
	ELSE
	BEGIN
		UPDATE	AcceptReferScore
		SET		dateto = GETDATE()
		WHERE	CountryCode = @country
		AND		Region = @region
		AND		datefrom = (SELECT  MAX(datefrom)
							FROM    AcceptReferScore
							WHERE   CountryCode = @country
							AND	    Region = @region
							AND	    ISNULL(dateto, '1/1/1900') = '1/1/1900'
							HAVING  DATEDIFF(day, max(datefrom), getdate()) >= 1)

		IF(@@rowcount > 0)
		BEGIN	
			INSERT	
			INTO	AcceptReferScore 
					(countrycode, region, acceptscore, referscore, datefrom)
			VALUES	(@country, @region, @acceptscore, @referscore, @datefrom)
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

