SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeMatrixRowSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeMatrixRowSaveSP]
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScoreBandMatrixRowSaveSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure DN_ScoreBandMatrixRowSaveSP
GO

CREATE PROCEDURE DN_ScoreBandMatrixRowSaveSP
    @CountryCode    CHAR(2),
    @scoretype		CHAR(1),
    @Band           VARCHAR(32),
    @PointsFrom     SMALLINT,
    @PointsTo       SMALLINT,
    @ServiceCharge  FLOAT,
    @DateImported   DATETIME,
    @ImportedBy     INTEGER,
    @FileName       VARCHAR(12),
    @Return         INTEGER OUTPUT

AS

    SET @return = 0
    SET NOCOUNT ON

    DELETE
    FROM   TermsTypeBand
    WHERE  CountryCode = @CountryCode
    AND    Band = @Band
    AND    DateImported = @DateImported
    AND    ScoreType = @scoretype

    INSERT
    INTO TermsTypeBand
        (CountryCode,
         ScoreType,
         Band,
         PointsFrom,
         PointsTo,
         ServiceCharge,
         DateImported,
         ImportedBy,
         FileName,
         StartDate)
    VALUES (@CountryCode,
			@Scoretype,
            @Band,
            @PointsFrom,
            @PointsTo,
            @ServiceCharge,
            @DateImported,
            @ImportedBy,
            @FileName,
            GETDATE()) -- start from current date and time.

    SET @Return = @@error
    
    SET NOCOUNT OFF
    RETURN @Return

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO


