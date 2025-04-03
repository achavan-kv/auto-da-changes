SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DDNewMandateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DDNewMandateSP]
GO


CREATE PROCEDURE DN_DDNewMandateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_DDNewMandateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Insert a new Giro Mandate
-- Author       : D Richardson
-- Date         : 20 Aug 2003
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    
    @piAcctNo          CHAR(12),
    @piDueDayId        INTEGER,
    @piBankAcctName    CHAR(40),
    @piBankCode        CHAR(4),
    @piBankBranchNo    CHAR(3),
    @piBankAcctNo      CHAR(11),
    @piReceiveDate     SMALLDATETIME,
    @piReturnDate      SMALLDATETIME,
    @piSubmitDate      SMALLDATETIME,
    @piApprovalDate    SMALLDATETIME,
    @piStartDate       SMALLDATETIME,
    @piEndDate         SMALLDATETIME,
    @piComment         VARCHAR(200),
    @piCancelReason    CHAR(1),
    @piNoFees          TINYINT,
    @piRejectCount     INTEGER,
    @piChangedBy       INTEGER,

    @Return          INTEGER OUTPUT

AS DECLARE
    -- Local variables
    
    @OldMandateCount    INTEGER,
    @OldMandateId       INTEGER,
    @NewMandateId       INTEGER

BEGIN

    SET NOCOUNT ON
    SET @Return = 0

    -- Blank dates must be NULL
    IF (@piReceiveDate <= CONVERT(SMALLDATETIME,'01 Jan 1900',106))
    BEGIN
        SET @piReceiveDate = null;
    END
    IF (@piReturnDate <= CONVERT(SMALLDATETIME,'01 Jan 1900',106))
    BEGIN
        SET @piReturnDate = null;
    END
    IF (@piSubmitDate <= CONVERT(SMALLDATETIME,'01 Jan 1900',106))
    BEGIN
        SET @piSubmitDate = null;
    END
    IF (@piApprovalDate <= CONVERT(SMALLDATETIME,'01 Jan 1900',106))
    BEGIN
        SET @piApprovalDate = null;
    END
    IF (@piStartDate <= CONVERT(SMALLDATETIME,'01 Jan 1900',106))
    BEGIN
        SET @piStartDate = null;
    END
    IF (@piEndDate <= CONVERT(SMALLDATETIME,'01 Jan 1900',106))
    BEGIN
        SET @piEndDate = null;
    END

    -- If there is already a current mandate for the same Courts
    -- Customer Account Number, then record the old Mandate Id.
    SELECT @OldMandateId = MandateID
    FROM   DDMandate
    WHERE  Status = 'C'
    AND    AcctNo = @piAcctNo;

    SET @OldMandateCount = @@ROWCOUNT

    IF @OldMandateCount > 0
    BEGIN

        -- Set the end date on the old mandate to keep as a history record
        UPDATE DDMandate
        SET    EndDate = DATEADD(Day, Country.DDLeadTime, GETDATE()),
               Status  = 'H',
               ChangedBy = @piChangedBy
        FROM   Country
        WHERE  MandateId = @OldMandateId;

    END;
    

    -- Insert the new mandate
    INSERT INTO DDMandate
        (AcctNo,
         DueDayId,
         BankAcctName,
         BankCode,
         BankBranchNo,
         BankAcctNo,
         ReceiveDate,
         ReturnDate,
         SubmitDate,
         ApprovalDate,
         StartDate,
         EndDate,
         Comment,
         CancelReason,
         NoFees,
         RejectCount,
         ChangedBy,
         Status)
     VALUES
        (
         @piAcctNo,
         @piDueDayId,
         @piBankAcctName,
         @piBankCode,
         @piBankBranchNo,
         @piBankAcctNo,
         @piReceiveDate,
         @piReturnDate,
         @piSubmitDate,
         @piApprovalDate,
         @piStartDate,
         @piEndDate,
         @piComment,
         @piCancelReason,
         @piNoFees,
         @piRejectCount,
         @piChangedBy,
         'C'
        );


    -- Get the new Mandate id
    SELECT @NewMandateId = MandateID
    FROM   DDMandate
    WHERE  Status = 'C'
    AND    AcctNo = @piAcctNo;

    IF @OldMandateCount > 0
    BEGIN

        -- Update any payments not yet sent to the bank to the new mandate id
        UPDATE DDPayment
        SET    MandateId = @NewMandateId
        WHERE  MandateId = @OldMandateId
        AND    Status IN ('I','R','H');
            
    END;
    
    SET @Return = @@ERROR
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

