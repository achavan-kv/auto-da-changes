SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeBandsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeBandsGetSP]
GO



CREATE PROCEDURE dbo.DN_TermsTypeBandsGetSP
    @CountryCode    CHAR(2),
    @Return         INTEGER OUTPUT

AS

    SET NOCOUNT ON
    SET @Return = 0

    SELECT  DISTINCT
            Band,
            PointsFrom,
            PointsTo,
            ServiceCharge
    FROM    TermsTypeBand T
    WHERE   CountryCode = @CountryCode
    AND     DateImported = 
            (SELECT MAX(DateImported)
             FROM   TermsTypeBand T2
             WHERE  CountryCode = @CountryCode
             AND    StartDate <= GETDATE()
             AND T.scoretype = T2.Scoretype  )
    ORDER BY Band
    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
