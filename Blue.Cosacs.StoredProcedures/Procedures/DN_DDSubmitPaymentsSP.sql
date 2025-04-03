

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDSubmitPaymentsSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDSubmitPaymentsSP
END
GO


CREATE PROCEDURE DN_DDSubmitPaymentsSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDSubmitPaymentsSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Add new due payments to the DDPAYMENT table 
-- Author       : D Richardson
-- Date         : 6 April 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piDueDayId         INTEGER,
    @piDueDate          SMALLDATETIME,
    @piEffectiveDate    SMALLDATETIME,
    @piMinPeriod        INTEGER,
    @return             INTEGER OUTPUT

AS  --DECLARE
    -- Local variables

BEGIN
    SET @return = 0
    SET NOCOUNT ON

    /* Create a temporary table of due payments */
    CREATE TABLE #DDTempPayment (
        MandateID       INT             PRIMARY KEY NOT NULL,
        AcctNo          CHAR(12)        NOT NULL,
        Amount          MONEY           NOT NULL,
        DateEffective   SMALLDATETIME   NULL DEFAULT NULL,
        Status          CHAR(1)         NOT NULL DEFAULT 'I',       -- $DDST_Init
        DeleteRow       TINYINT         NOT NULL DEFAULT 0)


    /* Populate the temp table with the active mandates after delivery.
    ** Use DATEDIFF to compare dates and ignore the time element.
    */
    INSERT INTO #DDTempPayment
        (MandateId,
         AcctNo,
         Amount)
    SELECT
        man.MandateId,
        man.Acctno,
        ROUND(ISNULL(inp.InstalAmount,0),2)
    FROM  DDMandate man, Agreement agr, InstalPlan inp
    WHERE man.Status = 'C'      -- $DDMS_Current
    AND   man.DueDayId = @piDueDayId
    AND   DATEDIFF(Day, man.StartDate, @piDueDate) >= 0
    AND   (   man.EndDate IS NULL
            OR DATEDIFF(Day, man.EndDate, @piDueDate) < 0)
    AND   agr.AcctNo = man.AcctNo
    AND   agr.AgrmtNo = 1
    AND   DATEDIFF(Day, agr.DateDel, @piDueDate) >= @piMinPeriod
    AND   agr.DateDel != CONVERT(SMALLDATETIME,'01 Jan 1900',113)
    AND   inp.AcctNo = man.AcctNo
    AND   inp.AgrmtNo = 1
    AND   DATEDIFF(Day, inp.DateFirst, @piDueDate) >= 0
    AND   inp.DateFirst != CONVERT(SMALLDATETIME,'01 Jan 1900',113)
    AND   ROUND(ISNULL(inp.InstalAmount,0),2) >= 0.01


    /* Add delete records for mandates that have been cancelled */
    INSERT INTO #DDTempPayment
        (MandateId,
         AcctNo,
         Amount)
    SELECT
        man.MandateId,
        man.Acctno,
        -1.0
    FROM    DDMandate man
    WHERE   man.Status = 'C'      -- $DDMS_Current
    AND     man.DueDayId = @piDueDayId
    AND     DATEDIFF(Day, man.EndDate, @piDueDate) >= 0


    /* Delete rows from temp table that are already in the DDPayment table from
    ** a previous run. Mark the rows to delete, then delete the marked rows.
    */
    UPDATE  #DDTempPayment
    SET     DeleteRow = 1
    FROM    DDMandate man, DDPayment pay
    WHERE   man.AcctNo          = #DDTempPayment.AcctNo
    AND     pay.MandateId       = man.MandateId
    AND     pay.OrigPaymentType = 'N'        -- $DDPT_Normal
    AND     pay.OrigMonth       = MONTH(@piDueDate)

    DELETE  FROM #DDTempPayment
    WHERE   DeleteRow = 1


    /* Reduce the payment amount to the Outstanding Balance if it is smaller */
    UPDATE  #DDTempPayment
    SET     Amount = ROUND(ISNULL(Acct.OutStBal,0),2)
    FROM    Acct
    WHERE   Acct.AcctNo = #DDTempPayment.AcctNo
    AND     ROUND(ISNULL(Acct.OutStBal,0),2) < #DDTempPayment.Amount
    AND     #DDTempPayment.Amount != -1.0


    /* Reduce the payment amount to zero if the account is in advance.
    ** This assumes that the arrears calculation has been run today.
    ** Check if in advance by >= payment because the due date is on/after today.
    */
    UPDATE  #DDTempPayment
    SET     Amount = 0
    FROM    Acct
    WHERE   Acct.AcctNo = #DDTempPayment.AcctNo
    AND     ROUND(ISNULL(Acct.Arrears,0)*-1,2) >= #DDTempPayment.Amount
    AND     #DDTempPayment.Amount != -1.0


    /* Each payment amount <= 0 (but not -1) will be kept to record that the
    ** mandate was processed in that month. The DateEffective will be set to
    ** today and the Status to 'Completed' for < 0.01 payments.
    */
    UPDATE  #DDTempPayment
    SET     DateEffective = @piEffectiveDate,
            Status = 'C'        -- $DDST_Complete
    WHERE   Amount  <  0.01
    AND     Amount != -1.0


    /* Copy the new payment rows to the DDPayment table.
    ** Ensure no other session is using this table by using
    ** an exclusive table lock.
    */
    INSERT INTO DDPayment WITH (TABLOCKX)
        (MandateId,
         PaymentType,
         OrigPaymentType,
         OrigMonth,
         DateEffective,
         Amount,
         Status,
         RejectCount)
    SELECT
        MandateId,
        'N',    -- $DDPT_Normal
        'N',    -- $DDPT_Normal
        MONTH(@piDueDate),
        DateEffective,
        Amount,
        Status,
        0
    FROM #DDTempPayment


    /* Set the Status for mandates that will be deleted */
    UPDATE DDMandate WITH (TABLOCKX)
    SET   Status = 'D'      -- $DDMS_Deleted
    FROM  #DDTempPayment
    WHERE DDMandate.Status = 'C'    -- $DDMS_Current
    AND   DDMandate.AcctNo = #DDTempPayment.AcctNo
    AND   #DDTempPayment.Amount = -1.0


    SET NOCOUNT OFF

    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDSubmitPaymentsSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
