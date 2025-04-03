SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS OFF 
GO

if exists (select * from dbo.sysobjects
where id = object_id('[dbo].[DN_IntRateHistoryDeleteSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_IntRateHistoryDeleteSP]
GO

CREATE PROCEDURE dbo.DN_IntRateHistoryDeleteSP
    @termstype varchar(4),
    @return int OUTPUT

AS
    SET NOCOUNT ON
    SET @Return = 0

    DELETE  FROM intratehistorydeletedtemp 
    WHERE   termstype = @termstype
    -- CR806 Terms Type screen only edits current/future rates
    AND   (DATEDIFF(Day, DateTo, GETDATE()) < 0 OR DateTo = CONVERT(DATETIME,'1 Jan 1900',106))

    INSERT INTO intratehistorydeletedtemp
       (termstype,
        datefrom,
        dateto,
        intrate, 
        inspcent,
        AdminPcent,
        InsIncluded,
        IncludeWarranty, 
        paymentholidaymin,
        paymentholidayarrears,
        ratetype,
        DtNetFirstIn, 
        intrate2,
        AdminValue)
    SELECT termstype,
        datefrom,
        dateto,
        intrate, 
        inspcent,
        AdminPcent,
        InsIncluded,
        IncludeWarranty, 
        paymentholidaymin,
        paymentholidayarrears,
        ratetype,
        DtNetFirstIn,
        intrate2,
        AdminValue
    FROM intratehistory 
    WHERE termstype = @termstype
    -- CR806 Terms Type screen only edits current/future rates
    AND   (DATEDIFF(Day, DateTo, GETDATE()) < 0 OR DateTo = CONVERT(DATETIME,'1 Jan 1900',106))


    DELETE FROM intratehistory
    WHERE termstype = @termstype
    -- CR806 Terms Type screen only edits current/future rates
    AND   (DATEDIFF(Day, DateTo, GETDATE()) < 0 OR DateTo = CONVERT(DATETIME,'1 Jan 1900',106))


    SET @return = @@error
    SET NOCOUNT OFF
    RETURN @Return
GO



SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

