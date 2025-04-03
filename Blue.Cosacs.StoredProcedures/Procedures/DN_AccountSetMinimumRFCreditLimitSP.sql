
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_AccountSetMinimumRFCreditLimitSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountSetMinimumRFCreditLimitSP]
GO

CREATE PROCEDURE dbo.DN_AccountSetMinimumRFCreditLimitSP
            @country char(2),
            @custid varchar(20),
            @dateprop datetime,
            @score int,
            @region varchar(3),
            @creditlimit money OUT,
            @return int OUTPUT

AS

	SET     @return = 0            --initialise return code

	DECLARE @category char(12)

	SELECT	@category = ISNULL(C.category,'PCF')
	FROM    proposal P LEFT JOIN code C ON CAST(P.rfcategory as varchar(4)) = C.code 
	AND     C.category in ('PCF', 'PCO', 'PCE', 'PCW')
	WHERE   P.custid = @custid
	AND     P.dateprop = @dateprop

	IF ((@category = 'PCF') or (@category = 'PCO'))
	BEGIN
		SELECT  @creditlimit = MIN(furnlimit)
		FROM    rfcreditlimit
		WHERE   countrycode = @country
		AND     region = @region
		AND     @score >=
		        (SELECT MIN(score)
		         FROM   rfcreditlimit
		         WHERE  countrycode = @country
		         AND    region = @region
		         AND	dateimported = (SELECT MAX(dateimported)	 -- SC 24/4/2007 Issue:68946
									  FROM   rfcreditlimit
		                              WHERE  countrycode = @country
		                              AND    region = @region))
		AND     dateimported =
		        (SELECT MAX(dateimported)
		         FROM   rfcreditlimit
		         WHERE  countrycode = @country
		         AND    region = @region)
	END

	IF ((@category = 'PCE') or (@category = 'PCW'))
	BEGIN
		SELECT  @creditlimit = MIN(eleclimit)
		FROM    rfcreditlimit
		WHERE   countrycode = @country
		AND     region = @region
		AND     @score >=
		        (SELECT MIN(score)
		         FROM   rfcreditlimit
		         WHERE  countrycode = @country
		         AND    region = @region
				 AND	dateimported = (SELECT MAX(dateimported)	 -- SC 24/4/2007 Issue:68946
									  FROM   rfcreditlimit
		                              WHERE  countrycode = @country
		                              AND    region = @region))
		AND     dateimported =
		        (SELECT MAX(dateimported)
		         FROM   rfcreditlimit
		         WHERE  countrycode = @country
		         AND    region = @region)
	END

	SELECT @creditlimit = ISNULL(@creditlimit, 0)

	SET @return = @@error
GO

SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO