SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_DDUpdateMandateSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_DDUpdateMandateSP]
GO


CREATE PROCEDURE DN_DDUpdateMandateSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS Transact ? 2002 Strategic Thought Ltd.
-- File Name    : DN_DDUpdateMandateSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update an existing Giro Mandate
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
    
    @piMandateId       INTEGER,
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
    @piCancelReason    VARCHAR(1),
    @piNoFees          TINYINT,
    @piChangedBy       INTEGER,

    @Return            INTEGER OUTPUT

AS -- DECLARE
    -- Local variables

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

    UPDATE DDMandate
    SET AcctNo       = @piAcctNo,
        DueDayId     = @piDueDayId,
        BankAcctName = @piBankAcctName,
        BankCode     = @piBankCode,
        BankBranchNo = @piBankBranchNo,
        BankAcctNo   = @piBankAcctNo,
        ReceiveDate  = @piReceiveDate,
        ReturnDate   = @piReturnDate,
        SubmitDate   = @piSubmitDate,
        ApprovalDate = @piApprovalDate,
        StartDate    = @piStartDate,
        EndDate      = @piEndDate,
        Comment      = @piComment,
        CancelReason = @piCancelReason,
        NoFees       = @piNoFees,
        ChangedBy    = @piChangedBy
    WHERE MandateId = @piMandateId;

    SET @Return = @@ERROR
    RETURN @Return
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

