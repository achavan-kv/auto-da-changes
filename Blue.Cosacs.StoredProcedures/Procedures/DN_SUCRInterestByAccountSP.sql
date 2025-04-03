SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_SUCRInterestByAccountSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_SUCRInterestByAccountSP
END
GO


CREATE PROCEDURE DN_SUCRInterestByAccountSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_SUCRInterestByAccountSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : To list SUCR Interest by account on settled and unsettled accounts
-- Author       : D Richardson
-- Date         : 17 Feb 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @piRunNo        SMALLINT,             -- batch/run number must be supplied
    @piBranchNo     SMALLINT = 0,         -- supply branch number or 0 for all.
    @return         INTEGER = 0 OUTPUT

AS DECLARE
    -- Local variables
    @minBranchNo    SMALLINT,
    @maxBranchNo    SMALLINT,
    @curDateFinish  SMALLDATETIME,
    @prevDateStart  SMALLDATETIME,
    @prevRunnNo     INTEGER

BEGIN
    SET NOCOUNT ON
    SET @return = 0

    IF @piBranchNo = 0
    BEGIN
        SET @minBranchNo = -999
        SET @maxBranchNo = 1000
    END
    ELSE
    BEGIN
        SET @minBranchNo = @piBranchNo
        SET @maxBranchNo = @piBranchNo
    END

    SELECT @curDateFinish = DateFinish
    FROM   InterfaceControl
    WHERE  Interface = 'UPDSMRY'
    AND    RunNo = @piRunNo

    SELECT @prevRunnNo = MAX(runno)
    FROM   InterfaceControl
    WHERE  Interface = 'UPDSMRY'
    AND    Result = 'P'
    AND    RunNo < @piRunNo

    SELECT @prevDateStart = DateStart
    FROM   InterfaceControl
    WHERE  Interface = 'UPDSMRY'
    AND    RunNo = @prevRunnNo

    -- INTONUNSETT by account
    SELECT AcctNo, SUM(TransValue) AS Interest
    FROM   FinTrans
    WHERE  DateTrans BETWEEN @prevDateStart AND @curDateFinish
    AND    (RunNo = 0 OR RunNo > @piRunNo)
    AND    TransTypeCode in ('ADM','INT')
    AND    LEFT(AcctNo,3) BETWEEN @minBranchNo AND @maxBranchNo
    GROUP BY AcctNo
    ORDER BY AcctNo

    -- INTONSETT by account
    -- RunNo is a SMALLINT on FinTrans so @RunNo MUST be SMALLINT for index to be used!
    SELECT AcctNo, SUM(TransValue) AS Interest
    FROM   FinTrans
    WHERE  RunNo = @piRunNo
    AND    TransTypeCode in ('ADM','INT')
    AND    LEFT(AcctNo,3) BETWEEN @minBranchNo AND @maxBranchNo
    GROUP BY AcctNo
    ORDER BY AcctNo


    SET NOCOUNT OFF
    SET @return = @@ERROR
    RETURN @return
END

GO
GRANT EXECUTE ON DN_SUCRInterestByAccountSP TO PUBLIC
GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

