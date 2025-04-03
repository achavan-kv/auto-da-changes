if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_ScoreBandMatrixApplySP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure DN_ScoreBandMatrixApplySP
GO 

CREATE PROCEDURE dbo.DN_ScoreBandMatrixApplySP
    @StartDate      DATETIME,
    @EmpeeNo        INTEGER,
    @scoretype CHAR(1),
    @Return         INTEGER OUTPUT

AS DECLARE

    @Today          DATETIME,
    @CountryCode    CHAR(1),
    @DateImported   DATETIME,
    @EndDate        DATETIME,
    @Band           VARCHAR(32)

BEGIN

    SET NOCOUNT ON
    SET @return = 0
    
    SET @Today = GETDATE()
    
    -- The start date is always at midnight
    SET @StartDate = CONVERT(DATETIME,CONVERT(CHAR(10),@StartDate,105),105)
    -- An end date is always one minute before midnight
    -- Note: SMALLDATETIME would round 23:59:59 up to 00:00:00 next day
    SET @EndDate = DATEADD(Minute,-1, @StartDate)


    SELECT @CountryCode = MIN(c.Value),
           @DateImported = MAX(b.DateImported)
    FROM   CountryMaintenance c, TermsTypeBand b
    WHERE  c.CodeName = 'countrycode'
    AND    b.CountryCode = c.Value
    AND b.ScoreType = @scoretype

    -- Mark this matrix with the start date for the terms types
    UPDATE TermsTypeBand
    SET    StartDate = @StartDate
    WHERE  CountryCode = @CountryCode
    AND    DateImported = @DateImported
    AND ScoreType = @scoretype

    --
    -- End all the existing terms types
    --

    -- Delete future terms types not started before the new start date
    DELETE FROM IntrateHistory
    WHERE  DateFrom > @EndDate
    AND EXISTS (SELECT * FROM TermsTypeBand t
                WHERE ScoreType = @scoretype
                AND intratehistory.Band = t.band)

    -- End date current terms types to end a second before midnight on the new start date
    UPDATE IntrateHistory
    SET    DateTo = @EndDate,
           EmpeeNoChange = @EmpeeNo,
           DateChange = @Today
    FROM intratehistory i
    WHERE  DateFrom <= @EndDate
    AND    (ISNULL(DateTo,CONVERT(DATETIME,'01-Jan-1900',113)) = CONVERT(DATETIME,'01-Jan-1900',113)
            OR DateTo > @EndDate)
    AND EXISTS (SELECT * FROM TermsTypeBand t  
                WHERE ScoreType = @scoretype
                AND t.band = i.band)


    -- The bands may or may not have been populated before,
    -- so select one band to initially copy from
   
    -- Populate the new bands
    INSERT INTO IntrateHistory
       (TermsType,
        DateFrom,
        DateTo,
        IntRate,
        InsPcent,
        AdminPcent,
        InsIncluded,
        IncludeWarranty,
        PaymentHolidayMin,
        PaymentHolidayArrears,
        RateType,
        DtNetFirstIn,
        Intrate2,
        EmpeeNoChange,
        DateChange,
        Band,
        PointsFrom,
        PointsTo)
    SELECT DISTINCT    -- Distinct in case of duplicates in history data
        i.TermsType,
        @StartDate,
        CONVERT(DATETIME,'01-Jan-1900',113),
        b.ServiceCharge,
        i.InsPcent,
        i.AdminPcent,
        i.InsIncluded,
        i.IncludeWarranty,
        i.PaymentHolidayMin,
        i.PaymentHolidayArrears,
        i.RateType,
        i.DtNetFirstIn,
        i.Intrate2,
        @EmpeeNo,
        @Today,
        b.Band,
        b.PointsFrom,
        b.PointsTo
    FROM  IntrateHistory i, TermsTypeBand b
    WHERE i.Band = @band

    AND   b.CountryCode = @CountryCode
    AND   b.DateImported = @DateImported
    AND i.DateTo = @EndDate
    AND b.ScoreType = @scoretype

   -- Update where the same bands were used before
    UPDATE IntrateHistory
    SET InsPcent               = i.InsPcent,
        AdminPcent             = i.AdminPcent,
        InsIncluded            = i.InsIncluded,
        IncludeWarranty        = i.IncludeWarranty,
        PaymentHolidayMin      = i.PaymentHolidayMin,
        PaymentHolidayArrears  = i.PaymentHolidayArrears,
        RateType               = i.RateType,
        DtNetFirstIn           = i.DtNetFirstIn,
        Intrate2               = i.Intrate2
    FROM  IntrateHistory, IntrateHistory i
WHERE IntrateHistory.DateFrom = @StartDate
    AND   i.DateTo = @EndDate
    AND   i.TermsType = IntrateHistory.TermsType
    AND   i.Band = IntrateHistory.Band
    
	-- add functionality to insert bands for new TT's
	-- that have not had interesthistory saved against them
INSERT INTO intratehistory
(termstype, datefrom, dateto, intrate,empeenochange,datechange,band,pointsFrom, PointsTo)
SELECT T.TermsType, @StartDate, CONVERT(DATETIME,'01-Jan-1900',113), TB.ServiceCharge, @EmpeeNo,
		@Today, TB.Band, TB.PointsFrom,TB.PointsTo 
FROM TermsTypeTable T
CROSS JOIN TermsTypeBand TB
WHERE T.countrycode= @CountryCode
and StartDate = @StartDate
AND TB.CountryCode = @CountryCode
AND DateImported = @DateImported
AND ScoreType = @scoretype
AND NOT EXISTS (SELECT * FROM intratehistory i
                WHERE i.termstype = T.termstype
                AND i.band = TB.Band
                AND i.datechange = @DateImported)



    SET @Return = @@error
    
    SET NOCOUNT OFF
    RETURN @Return
END

GO 





