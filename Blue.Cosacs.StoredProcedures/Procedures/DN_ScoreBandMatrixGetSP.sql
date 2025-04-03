SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeMatrixGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeMatrixGetSP]
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScoreBandMatrixGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure DN_ScoreBandMatrixGetSP
GO




CREATE PROCEDURE DN_ScoreBandMatrixGetSP
    @CountryCode    CHAR(2),
    @scoretype CHAR(1),
    @Return         INTEGER OUTPUT

AS
	SET NOCOUNT ON
    SET @Return = 0

    SELECT  CountryCode,
            Band,
            PointsFrom,
            PointsTo,
            ServiceCharge,
            StartDate
    FROM    TermsTypeBand
    WHERE   CountryCode = @CountryCode
    AND ScoreType = @scoretype
    AND     DateImported = 
            (SELECT MAX(DateImported)
             FROM   TermsTypeBand
             WHERE  CountryCode = @CountryCode
             AND ScoreType = @scoretype)
    ORDER BY PointsFrom DESC

    SET @Return = @@error

	SET NOCOUNT OFF
	RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO