SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeMatrixDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeMatrixDeleteSP]
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScoreBandMatrixDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure DN_ScoreBandMatrixDeleteSP
GO


CREATE PROCEDURE dbo.DN_ScoreBandMatrixDeleteSP
    @CountryCode    CHAR(2),
    @scoretype		CHAR(1),
    @Return         INTEGER OUTPUT

AS DECLARE

    @LastImport     DATETIME

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    SELECT @LastImport = MAX(DateImported)
    FROM   TermsTypeBand
    WHERE  Countrycode = @countryCode
    AND scoretype = @scoretype

    DELETE FROM TermsTypeBand
    WHERE  Countrycode = @countryCode
    AND    DateImported = @LastImport
    AND scoretype = @scoretype

    SET @Return = @@error
    
    SET NOCOUNT OFF
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


