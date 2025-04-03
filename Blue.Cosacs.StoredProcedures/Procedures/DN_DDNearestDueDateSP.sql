

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDNearestDueDateSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDNearestDueDateSP
END
GO


CREATE PROCEDURE DN_DDNearestDueDateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDNearestDueDateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Get the nearest due date to today, either before or after today
-- Author       : D Richardson
-- Date         : 3 May 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @return             INTEGER       OUTPUT

AS  DECLARE
    -- Local variables
    @today              SMALLDATETIME,
    @thisMonthStart     SMALLDATETIME,
    @lastMonthStart     SMALLDATETIME,
    @nextMonthStart     SMALLDATETIME

BEGIN
    SET NOCOUNT ON
    SET @return = 0
    -- Use today's date without any time part
    SET @today = CONVERT(SMALLDATETIME, CONVERT(CHAR(10), GETDATE(), 103), 103);
    SET @thisMonthStart = DATEADD(Day, 1-DAY(@today), @today);
    SET @lastMonthStart = DATEADD(Month, -1, @thisMonthStart);
    SET @nextMonthStart = DATEADD(Month, +1, @thisMonthStart);

    SELECT TOP 1     -- Current month
           ABS(DATEDIFF(Day, @today, DATEADD(Day, day.DueDay, @thisMonthStart))) AS DayDiff,
           DATEADD(Day, day.DueDay-1, @thisMonthStart) AS NearestDueDate
    FROM   DDDueDay day
    WHERE  EndDate IS NULL
    UNION  SELECT    -- Last month
           ABS(DATEDIFF(Day, @today, DATEADD(Day, day.DueDay, @lastMonthStart))),
           DATEADD(Day, day.DueDay-1, @lastMonthStart)
    FROM   DDDueDay day
    WHERE  EndDate IS NULL
    UNION  SELECT    -- Next month
           ABS(DATEDIFF(Day, @today, DATEADD(Day, day.DueDay, @nextMonthStart))),
           DATEADD(Day, day.DueDay-1, @nextMonthStart)
    FROM   DDDueDay day
    WHERE  EndDate IS NULL
    ORDER BY DayDiff ASC
        
    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDNearestDueDateSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
