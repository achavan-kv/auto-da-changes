SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_TermsTypeGetVariableRatesSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeGetVariableRatesSP]
GO


CREATE PROCEDURE 	dbo.DN_TermsTypeGetVariableRatesSP
			@termsType varchar(2),
			@dateOpened datetime,
			@return int OUTPUT

AS

	SET 	@return = 0			--initialise return code

	SELECT	frommonth,
		tomonth,
		rate
	FROM	TermsTypeVariableRates
	WHERE	termstype = @termsType
    	AND     (@dateOpened BETWEEN intratefrom AND intrateto OR 
		(@dateOpened > intratefrom AND intrateto = '1/1/1900'))
	ORDER BY frommonth ASC

	IF (@@error != 0)
	BEGIN
		SET @return = @@error
	END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

