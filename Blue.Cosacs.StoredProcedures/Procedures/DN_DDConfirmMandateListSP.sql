
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDConfirmMandateListSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDConfirmMandateListSP
END
GO


CREATE PROCEDURE DN_DDConfirmMandateListSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDConfirmMandateListSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Retrieve all mandates with an Approval Date and a Date Delivered
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
    @return             INTEGER OUTPUT

AS -- DECLARE
    -- Local variables

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* Retrieve the Approval Date and the due day for each mandate ready to start */
    SELECT  man.MandateId,
            man.AcctNo,
            man.ApprovalDate,
            day.DueDay
    FROM    DDMandate man WITH (TABLOCKX), DDDueDay day, Agreement agr
    WHERE   man.Status = 'C'    -- $DDMS_Current
    AND     man.ApprovalDate IS NOT NULL
    AND     man.StartDate IS NULL
    AND     (   man.EndDate IS NULL
             OR DATEDIFF(Day, @piEffectiveDate, man.EndDate) > 0)
    AND     day.DueDayId = man.DueDayId
    AND     agr.AcctNo = man.AcctNo
    AND     agr.DateDel IS NOT NULL
    AND     agr.DateDel != CONVERT(SMALLDATETIME,'01 Jan 1900',113)


    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return

END


GO
GRANT EXECUTE ON DN_DDConfirmMandateListSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
