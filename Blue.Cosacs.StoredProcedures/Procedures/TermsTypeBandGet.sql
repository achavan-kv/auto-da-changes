SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO


IF EXISTS (SELECT * 
           FROM dbo.sysobjects
		   WHERE name = 'TermsTypeBandListGet'
		   AND xtype = 'P')
DROP PROCEDURE TermsTypeBandListGet
GO

CREATE PROCEDURE TermsTypeBandListGet
    @Return         INTEGER OUTPUT

AS

    SET @Return = 0

    SELECT DISTINCT Band,Scoretype 
    FROM   TermsTypeBand
    WHERE countrycode = (SELECT CountryCode FROM country)
    ORDER BY Band

    SET @Return = @@error

GO

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


