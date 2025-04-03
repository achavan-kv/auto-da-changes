SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_TermsTypeBandsAdjustSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeBandsAdjustSP]
GO



CREATE PROCEDURE dbo.DN_TermsTypeBandsAdjustSP
    @AdjustDate        SMALLDATETIME,
    @AdjustIns         FLOAT,
    @AdjustSC          FLOAT,
    @EmpeeNo           INTEGER,
    @Return            INTEGER OUTPUT

AS DECLARE

    @Today			   DATETIME,
    @EndDate           DATETIME

BEGIN
    SET NOCOUNT ON
    SET @Return = 0
    
    SET @Today = GETDATE()
    
    -- The start date is always at midnight
    SET @AdjustDate = CONVERT(DATETIME,CONVERT(CHAR(10),@AdjustDate,105),105)
    -- An end date is always one minute before midnight
    -- Note: SMALLDATETIME would round 23:59:59 up to 00:00:00 next day
    SET @EndDate = DATEADD(Minute,-1, @AdjustDate)

    --
	-- Update all rates with an end date after the adjustment date (or a blank end date)
	--
	-- Don't need a history record for terms not yet started so just update

	UPDATE IntrateHistory
	SET    IntRate = IntRate + @AdjustSC,
		   InsPcent = InsPcent + @AdjustIns,
		   EmpeeNoChange = @EmpeeNo,
		   DateChange = @Today
	WHERE  Band != ''
	AND    DATEDIFF(Day, DateFrom, @Today) <= 0
	AND    (DateTo >= @AdjustDate
			OR DateTo = CONVERT(DATETIME,'1-Jan-1900',106))

	-- Terms already started require a new record

	INSERT INTO IntrateHistory
		(TermsType, DateFrom, DateTo,
		 IntRate, InsPcent, AdminPcent,
		 InsIncluded, IncludeWarranty, PaymentHolidayMin,
		 PaymentHolidayArrears, RateType, DtNetFirstIn, IntRate2,
		 EmpeeNoChange, DateChange, Band, PointsFrom, PointsTo)
	SELECT
		 TermsType, @AdjustDate, DateTo,
		 IntRate + @AdjustSC, InsPcent + @AdjustIns, AdminPcent,
		 InsIncluded, IncludeWarranty, PaymentHolidayMin,
		 PaymentHolidayArrears, RateType, DtNetFirstIn, IntRate2,
		 @EmpeeNo, @Today, Band, PointsFrom, PointsTo
	FROM   IntrateHistory
	WHERE  Band != ''
	AND    DATEDIFF(Day, DateFrom, @Today) > 0
	AND    (DateTo >= @AdjustDate
			OR DateTo = CONVERT(DATETIME,'1-Jan-1900',106))

	-- Terminate the history record for terms already started

	UPDATE IntrateHistory
	SET    DateTo = @EndDate,
		   EmpeeNoChange = @EmpeeNo,
		   DateChange = @Today
	WHERE  Band != ''
	AND    DATEDIFF(Day, DateFrom, @Today) > 0
	AND    (DateTo >= @AdjustDate
			OR DateTo = CONVERT(DATETIME,'1-Jan-1900',106))
			

    SET @Return = @@error

    SET NOCOUNT OFF
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

