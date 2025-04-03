SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeMatrixDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeMatrixDeleteSP]
GO

--if exists (select * from dbo.sysobjects
--where id = object_id('[dbo].[DN_ScoreBandMatrixDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
--drop procedure DN_ScoreBandMatrixDeleteSP
--GO


--CREATE PROCEDURE dbo.DN_ScoreBandMatrixDeleteSP		--IP - 09/04/10 - CR1034 - Removed
CREATE PROCEDURE dbo.DN_TermsTypeMatrixDeleteSP
    @CountryCode    CHAR(2),
    --@scoretype		CHAR(1),						--IP - 09/04/10 - CR1034 - Removed
    @Return         INTEGER OUTPUT

AS DECLARE

    @LastImport     DATETIME

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    SELECT @LastImport = MAX(DateImported)
    FROM   TermsTypeBand
    WHERE  Countrycode = @countryCode
    --AND scoretype = @scoretype						--IP - 09/04/10 - CR1034 - Removed

    DELETE FROM TermsTypeBand
    WHERE  Countrycode = @countryCode
    AND    DateImported = @LastImport
    --AND scoretype = @scoretype						--IP - 09/04/10 - CR1034 - Removed

    SET @Return = @@error
    
    SET NOCOUNT OFF
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

