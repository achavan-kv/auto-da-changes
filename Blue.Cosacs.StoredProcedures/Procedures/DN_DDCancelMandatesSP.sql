SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDCancelMandatesSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDCancelMandatesSP
END
GO


CREATE PROCEDURE DN_DDCancelMandatesSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDCancelMandatesSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Cancel mandates not approved or with a cancelled rejection
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
    @piNextDueDate      SMALLDATETIME,
    @piMaxRejections    INTEGER,
    @return             INTEGER OUTPUT

AS  DECLARE
    -- Local variables
    @processId          INTEGER

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* Ignore any time element in the run date */
    SET @piRunDate = CONVERT(DATETIME,CONVERT(CHAR(10),@piRunDate,105),105)

    /* Use a Process Id so that confirmation letters can be created at the end of this run.
    ** This allows set processing to be used instead of Cursor Loops.
    */
    SELECT @processId = ISNULL(MAX(ProcessId)+1,1) FROM DDMandate WITH (TABLOCKX)

    /* The sequence of the following queries is significant, because where the
    ** same mandate satisfies more than one query, then the cancel reason
    ** will be set by the first query.
    */

    /* Cancel mandates with a settled account */
    UPDATE  DDMandate WITH (TABLOCKX)
    SET     ProcessId = @processId,
            EndDate = @piEffectiveDate,
            CancelReason = 'S'       -- $DDCR_Settled
    FROM    Acct
    WHERE   DDMandate.Status = 'C'   -- $DDMS_Current
    AND     (   DDMandate.EndDate IS NULL
             OR DATEDIFF(Day, @piEffectiveDate, DDMandate.EndDate) > 0)
    AND     Acct.AcctNo = DDMandate.Acctno
    AND     ISNULL(Acct.CurrStatus, 'X') = 'S'


    /* Cancel mandates with a cancelled rejection on the DDPayment table */
    UPDATE  DDMandate WITH (TABLOCKX)
    SET     ProcessId = @processId,
            EndDate = @piEffectiveDate,
            CancelReason = 'C'      -- $DDCR_CancelledReject
    FROM    DDPayment pay
    WHERE   DDMandate.Status = 'C'  -- $DDMS_Current
    AND     (   DDMandate.EndDate IS NULL
             OR DATEDIFF(Day, @piEffectiveDate, DDMandate.EndDate) > 0)
    AND     pay.MandateId = DDMandate.MandateId
    AND     pay.Status = 'R'        -- $DDST_Rejected
    AND     pay.RejectAction = 'C'  -- $DDRA_Cancel


    /* Cancel mandates with a rejection count that is greater than
    ** the country setting - max reject count (CR350)
    */
    UPDATE  DDMandate WITH (TABLOCKX)
    SET     ProcessId = @processId,
            EndDate = @piEffectiveDate,
            CancelReason = 'M'      -- $DDCR_MaxRejections
    WHERE   DDMandate.Status = 'C'  -- $DDMS_Current
    AND     (   DDMandate.EndDate IS NULL
             OR DATEDIFF(Day, @piEffectiveDate, DDMandate.EndDate) > 0)
    AND     RejectCount >= @piMaxRejections


    /* Make sure there is not a pending payment for a cancelled mandate.
    ** Pending payments already submitted to the bank will be left so that
    ** they can be subsequently rejected or the financial transaction created.
    ** Find any mandate with an End Date on or before the next Due Date so
    ** that user entered end dates are included.
    ** Do not cancel -1 amounts which are 'delete' records.
    */
    UPDATE  DDPayment WITH (TABLOCKX)
    SET     Status = 'X'    -- $DDST_Cancelled
    FROM    DDMandate man
    WHERE   DATEDIFF(Day, @piNextDueDate, man.EndDate) <= 0
    AND     DDPayment.MandateId = man.MandateId
    AND     DDPayment.Status IN ('I','R')   -- $DDST_Init, $DDST_Rejected
    AND     DDPayment.Amount != -1.0


    /* Finally, issue a cancellation letter for each mandate cancelled on this run */
    INSERT INTO Letter
            (AcctNo, DateAcctLttr, DateDue, LetterCode, AddToValue)
    SELECT
            m.AcctNo, @piRunDate, m.EndDate, 'GX', 0    -- $DDLE_CancelMandate
    FROM    DDMandate m
    WHERE   m.ProcessId = @processId
    AND     NOT EXISTS (SELECT 1 FROM Letter l
                        WHERE  l.AcctNo = m.AcctNo
                        AND    l.DateAcctLttr = @piRunDate
                        AND    l.LetterCode = 'GX')     -- $DDLE_CancelMandate


    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return

END

GO
GRANT EXECUTE ON DN_DDCancelMandatesSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

