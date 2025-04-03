
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDConfirmMandateSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDConfirmMandateSP
END
GO


CREATE PROCEDURE DN_DDConfirmMandateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDConfirmMandateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Start a mandate with an Approval Date and a Date Delivered
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
    @piRunDate          SMALLDATETIME,
    @piEffectiveDate    SMALLDATETIME,
    @piAcctNo           CHAR(12),
    @piMandateId        INTEGER,
    @piApprovalDate     SMALLDATETIME,
    @piDueDay           INTEGER,
    @piDateFirst        SMALLDATETIME,
    @return             INTEGER OUTPUT

AS DECLARE
    -- Local variables
    @startDate          SMALLDATETIME

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* If Date First has been set successfully then set up the mandate Start Date.    */
    /* Note that Date First is always calculated after the Delivery Date. The mandate */
    /* Start Date will be on/after all of the Date First, the mandate Approval Date   */
    /* and the next earliest bank date.                                               */

    IF @piDateFirst > CONVERT(SMALLDATETIME,'01 Jan 1900',113)
    BEGIN

        /* Ignore any time element in the dates */
        SET @piApprovalDate = CONVERT(DATETIME,CONVERT(CHAR(10),@piApprovalDate,105),105)
        SET @piDateFirst    = CONVERT(DATETIME,CONVERT(CHAR(10),@piDateFirst,105),105)
        SET @startDate      = DATEADD(Day, @piDueDay-DAY(@piRunDate), @piRunDate)

        /* Make sure the Start Date is not before Approval Date, Date First, or the next Effective Date */
        WHILE @startDate < @piApprovalDate OR @startDate < @piDateFirst OR @startDate < @piEffectiveDate
        BEGIN
            SET @startDate = DATEADD(Month, 1, @startDate)
        END

        /* Start Date is set to the first payment date */
        UPDATE  DDMandate WITH (TABLOCKX)
        SET     StartDate = @startDate
        WHERE   MandateId = @piMandateId

        /* Create the Confirmation letter */
        IF NOT EXISTS (SELECT 1 FROM Letter l
                       WHERE  l.AcctNo = @piAcctNo
                       AND    l.DateAcctLttr = @piRunDate
                       AND    l.LetterCode = 'GC')     -- $DDLE_ConfirmMandate
        BEGIN
            INSERT INTO Letter
                    (AcctNo, DateAcctLttr, DateDue, LetterCode, AddToValue)
            VALUES
                    (@piAcctNo, @piRunDate, @startDate, 'GC', 0)    -- $DDLE_ConfirmMandate
        END
    END


    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDConfirmMandateSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
