

SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_DDLoadRejectionFileSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_DDLoadRejectionFileSP
END
GO


CREATE PROCEDURE DN_DDLoadRejectionFileSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDLoadRejectionFileSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Load the rejections from the bank file and pre-process
-- Author       : D Richardson
-- Date         : 4 May 2006
--
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piRejectFileName       VARCHAR(200),
    @piRejectFormatName     VARCHAR(200),
    @piToday                SMALLDATETIME,
    @return                 INTEGER       OUTPUT

AS  DECLARE
    -- Local variables
    @curPaymentId           INTEGER,
    @paymentId              INTEGER,
    @rejectId               INTEGER,
    @rowCount               INTEGER

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    -- Load the rejections file from the bank
    TRUNCATE TABLE DDRejections

    EXECUTE
        (' BULK INSERT dbo.DDRejections ' +
         ' FROM ''' + @piRejectFileName + '''' +
         ' WITH (DATAFILETYPE = ''CHAR'',' +
         '       FORMATFILE = ''' + @piRejectFormatName + ''') ')

    /* Put the decmal places back into the amount */
	UPDATE DDRejections
	SET    Amount = Amount / 100
	

    /* Load matching payments that were submitted before today.
    **
    ** One Rejection:Many Payments relationship.
    ** There is no date information in the rejection records from the bank,
    ** so each rejection record could match to multiple payments that have
    ** been submitted on different dates.
    **
    ** One Payment:Many Rejections relationship.
    ** A payment can match to more than one rejection record, because it is valid
    ** for rejection records to appear to be duplicated where there are multiple
    ** fee payments on the same mandate.
    **
    ** Therefore the full product of matching payments are retrieved into a temp table.
    ** Subsequent queries will resolve duplicates and assign the earliest incomplete
    ** payment to each rejection record.
    */
    
    /* Create a temporary table of all submitted payments that match rejections */
    CREATE TABLE #DDTempSubmit
        (RejectId       INT NOT NULL,
         PaymentId      INT NOT NULL)
        


    /* Create the Many:Many relationship between Payments and Rejections.
    ** This relationship will be resolved by subsequent queries.
    ** Only include submitted payments that could be effective before today.
    **
    ** The data being matched from the reject record is loaded as character
    ** data, and the match must allow for different case chars being equivalent.
    ** We will not assume that fields that appear to be number fields will
    ** never contain letters.
    */
    INSERT INTO #DDTempSubmit
       (RejectId,
        PaymentId)
    SELECT
        trej.RejectId,
        pay.PaymentId
    FROM   DDRejections trej, DDMandate man, DDPayment pay, DDBankPayCode pcode
    WHERE  man.AcctNo = trej.AcctNo
    AND    LOWER(LTRIM(RTRIM(man.BankCode))) = trej.BankCode
    AND    LOWER(LTRIM(RTRIM(man.BankBranchNo))) = trej.BankBranchNo
    AND    LOWER(LTRIM(RTRIM(man.BankAcctNo))) = trej.BankAcctNo
    AND    pay.MandateId = man.MandateId
    AND    pay.Amount = trej.Amount
    AND    pay.PaymentType = pcode.PaymentType
    AND    LOWER(LTRIM(RTRIM(pcode.PayCode))) = trej.RejectCode
    AND    pcode.DueDayId = man.DueDayId
    AND    pay.Status = 'S'    -- $DDST_Submitted
    AND    DATEDIFF(Day, pay.DateEffective, @piToday) > 0



    /* Resolve the many payments per rejection record relationship, by updating
    ** the rejections temp table with the earliest matching payments.
    */
    UPDATE DDRejections
    SET    PaymentId = (SELECT MIN(tsub.PaymentId)
                        FROM   #DDTempSubmit tsub
                        WHERE  tsub.RejectId = DDRejections.RejectId)



    /* Resolve the many rejections per payment record relationship, by finding each
    ** duplicated payment and selecting the next earliest matching payment.
    **
    ** Duplicates must be resolved because it is valid to have duplicate entries
    ** in a rejections file for more than one outstanding fee payment (on the
    ** same mandate for the same amount). These duplicates should be very rare, so
    ** the following loop will modify each duplicate row as a separate query.
    ** The PaymentId is updated to identify the earliest incomplete payment after
    ** the earliest payment already identifed. This will correct any number of
    ** duplicate rejections.
    */

    /* The rows are sorted so that all duplicate payment ids are adjacent and
    ** the full data set does not have to be scanned to find each duplicate.
    */
    SET @curPaymentId = -1
    DECLARE Resolve_Csr CURSOR LOCAL
    FOR
        SELECT PaymentId, RejectId FROM DDRejections ORDER BY PaymentId

    OPEN Resolve_Csr

    FETCH NEXT FROM Resolve_Csr
    INTO @paymentId, @rejectId

    WHILE @@FETCH_STATUS = 0
    BEGIN
        /* Ignore unmatched rejection records */
        IF  @paymentId IS NOT NULL
        BEGIN
			IF @paymentId != @curPaymentId
			BEGIN
				/* Record the new payment key */
				SET @curPaymentId = @paymentId
			END
			ELSE
			BEGIN
				/* Another rejection record has matched to the same payment key,
				** so find the earliest payment after the payment already matched.
				*/

				/* Remove the submitted payments already allocated */
				DELETE FROM #DDTempSubmit
				WHERE  PaymentId IN (SELECT PaymentId FROM DDRejections)
	            
				/* Allocate the next most recent matching payment to this rejection */
				UPDATE DDRejections
				SET    PaymentId = (SELECT MIN(tsub.PaymentId)
									FROM   #DDTempSubmit tsub
									WHERE  tsub.RejectId = DDRejections.RejectId)
				WHERE DDRejections.RejectId = @rejectId

				SET @rowCount = @@ROWCOUNT;

				IF @rowCount = 0
				BEGIN
					/* There was no payment that could be allocated to this rejection,
					** so clear the payment details.
					*/
					UPDATE DDRejections
					SET    PaymentId = NULL
					WHERE  RejectId = @rejectId
				END
			END
		END
                   
        FETCH NEXT FROM Resolve_Csr
        INTO @paymentId, @rejectId
    END

    CLOSE Resolve_Csr
    DEALLOCATE Resolve_Csr


    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END


GO
GRANT EXECUTE ON DN_DDLoadRejectionFileSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
