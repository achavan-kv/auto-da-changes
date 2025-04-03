
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDNextDueDateSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDNextDueDateSP
END
GO


CREATE PROCEDURE DN_DDNextDueDateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDNextDueDateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Calculate the next Due Date
-- Author       : D Richardson
-- Date         : 5 April 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piEffectiveDate    SMALLDATETIME,
    @poDueDate          SMALLDATETIME OUTPUT,
    @poDueDayId         INTEGER       OUTPUT,
    @poDueDay           SMALLINT      OUTPUT,
    @return             INTEGER       OUTPUT

AS DECLARE
    -- Local variables
    @rowCount           INTEGER

BEGIN
    SET @return = 0
    SET NOCOUNT ON


    /* Work out the next due date and day of the month */
    /* on or after the next earliest effective date.   */
    SELECT  @poDueDayId = DueDayId,
            @poDueDay = DueDay,
            @poDueDate = DATEADD(Day, DueDay-DAY(@piEffectiveDate), @piEffectiveDate)
    FROM    DDDueDay
    WHERE   EndDate IS NULL
    AND     DueDay = (SELECT MIN(DueDay)
                      FROM   DDDueDay
                      WHERE  EndDate IS NULL
                      AND    DueDay >= DAY(@piEffectiveDate))

    SET @rowCount = @@ROWCOUNT

    /* Might need to start at the beginning of the next month */
    IF @rowCount = 0
    BEGIN
        /* Work out the next due date and day of next month */
        SELECT  @poDueDayId = DueDayId,
                @poDueDay = DueDay,
                @poDueDate = DATEADD(Month, 1, DATEADD(Day, DueDay-DAY(@piEffectiveDate), @piEffectiveDate))
        FROM    DDDueDay
        WHERE   EndDate IS NULL
        AND     DueDay = (SELECT MIN(DueDay) FROM DDDueDay)
    END

    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return

END

GO
GRANT EXECUTE ON DN_DDNextDueDateSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

