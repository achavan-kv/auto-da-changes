
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDGiroDateFirstSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDGiroDateFirstSP
END
GO


CREATE PROCEDURE DN_DDGiroDateFirstSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDGiroDateFirstSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve all mandates with an Approval Date and a Date Delivered
-- Author       : D Richardson
-- Date         : 5 April 2006
--
-- If the mandate is active then ensure Date First is set for Giro.
-- The Date First will not be changed if it is already set to a
-- date over a month old or if there is no Delivery Date.
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piAcctNo           CHAR(12),
    @piEffectiveDate    SMALLDATETIME,
    @piMinPeriod        INTEGER,
    @poDateFirst        SMALLDATETIME OUTPUT,
    @return             INTEGER OUTPUT

AS  DECLARE
    -- Local variables
    @dateFirst          SMALLDATETIME,
    @dueDay             INTEGER,
    @dueDate            SMALLDATETIME,
    @rowCount           INTEGER

BEGIN
    SET @return = 0
    SET NOCOUNT ON
    SET @poDateFirst = CONVERT(SMALLDATETIME,'01 Jan 1900',113)

    /* Retrieve the normal Date First after delivery and the giro Due Day.
    ** Data will only be returned for an active mandate with a Delivery Date.
    */
    SELECT  @dateFirst = DATEADD(Day, @piMinPeriod, agr.DateDel),
            @dueDay = day.DueDay
    FROM    DDMandate man, Agreement agr, DDDueDay day
    WHERE   man.AcctNo = @piAcctNo
    AND     man.Status = 'C'    -- $DDMS_Current
    AND     man.ApprovalDate IS NOT NULL
    AND     (   man.EndDate IS NULL
             OR DATEDIFF(Day, @piEffectiveDate, man.EndDate) > 0)
    AND     agr.AcctNo = man.AcctNo
    AND     agr.DateDel IS NOT NULL
    AND     agr.DateDel != CONVERT(SMALLDATETIME,'01 Jan 1900',113)
    AND     day.DueDayId = man.DueDayId

    SET @rowCount = @@ROWCOUNT

    IF @rowCount > 0 AND @dateFirst > CONVERT(SMALLDATETIME,'01 Jan 1905',113)
    BEGIN
        /* Ignore any time element in the dates */
        SET @dateFirst = CONVERT(DATETIME,CONVERT(CHAR(10),@dateFirst,105),105)
        SET @dueDate = DATEADD(Day, @dueDay-DAY(@dateFirst), @dateFirst)

        /* Make sure the first Due Date is on or after the normal Date First */
        WHILE @dueDate < @dateFirst
        BEGIN
            SET @dueDate = DATEADD(Month, 1, @dueDate)
        END;

        /* Only change Date First if it is blank or less than a month old */
        UPDATE  InstalPlan
        SET     DateFirst = @dueDate
        WHERE   AcctNo = @piAcctNo
        AND     (   DateFirst IS NULL OR DateFirst = CONVERT(SMALLDATETIME,'01 Jan 1900',113)
                 OR DATEDIFF(Month, DateFirst, GETDATE()) < 1)

        /* Return Giro Date First even though Instal Plan */
        /* may have a Date First over a month old.        */
        SET @poDateFirst = @dueDate;
    END


    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDGiroDateFirstSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
