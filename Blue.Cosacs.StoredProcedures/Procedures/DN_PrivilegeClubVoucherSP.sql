SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS OFF
GO

IF EXISTS (SELECT * FROM SysObjects
           WHERE Name = 'DN_PrivilegeClubVoucherSP' AND Type = 'P')
BEGIN
    DROP PROCEDURE DN_PrivilegeClubVoucherSP
END
GO


CREATE PROCEDURE DN_PrivilegeClubVoucherSP

--------------------------------------------------------------------------------
--
-- Project      : CoSACS .NET
-- File Name    : DN_PrivilegeClubVoucherSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Check whether an account qualifies for a free instalment
-- Author       : D Richardson
-- Date         : 27 Mar 2006
--
-- Change Control
-- --------------
-- Date      By  Description
-- ----      --  -----------
--
--------------------------------------------------------------------------------

    -- Parameters
    @acctNo            CHAR(12),
    @voucherAmount     MONEY OUTPUT,
    @return            INTEGER OUTPUT

AS  DECLARE
    -- Local variables
    @tierPCEnabled             VARCHAR(10),
    @consecutiveInstalments    INTEGER,
    @failedToQualify           SMALLINT,
    @instalmentsPaid           INTEGER,
    @instalAmount              MONEY

BEGIN

    SET NOCOUNT ON
    SET @return = 0
    SET @voucherAmount = 0
    SET @tierPCEnabled = 'False'
    SET @failedToQualify = 0

    -------------------------------------------------------------------------
    -- Load country parameters
    -------------------------------------------------------------------------
    -- PRINT 'Load country params (' + CONVERT(VARCHAR,GETDATE(),113) + ')'
    SELECT @tierPCEnabled = Value FROM CountryMaintenance WHERE CodeName = 'TierPCEnabled'
    SELECT @consecutiveInstalments = Value FROM CountryMaintenance WHERE CodeName = 'ConsecutiveInstalments'

    -- Check that free instalments is switched on
    IF (ISNULL(@tierPCEnabled,'False') = 'False' OR ISNULL(@consecutiveInstalments,0) = 0)
        SET @failedToQualify = 1

    -------------------------------------------------------------------------
    -- Check whether this account qualifies
    -------------------------------------------------------------------------

    -- The holding customer must be qualified for Privilege Club Tier1/2
    IF (@failedToQualify = 0)
    BEGIN
         IF NOT EXISTS (SELECT 1 FROM CustAcct ca, CustCatCode cc
                        WHERE  ca.AcctNo = @acctNo
                        AND    ca.HldOrJnt = 'H'
                        AND    cc.CustId = ca.CustId
                        AND    cc.Code IN ('TIR1','TIR2')
                        AND    cc.DateDeleted IS NULL)
        BEGIN
            SET @failedToQualify = 1
        END
    END

    -- The account must not have had a previous free instalment in the past year
    IF (@failedToQualify = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM AcctCode
                   WHERE  AcctNo = @acctNo
                   AND    Code = 'FREE'
                   AND    DATEADD(Year,1,DateCoded) > GETDATE()
                   AND    DateDeleted IS NULL)
        BEGIN
            SET @failedToQualify = 1
        END
    END

    -- The account must not be settled and not have a status code above '2'
    IF (@failedToQualify = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM Acct
                   WHERE  AcctNo = @acctNo
                   AND    (CurrStatus NOT IN ('U','0','1','2') 
                           OR HighstStatus NOT IN ('S','U','0','1','2')))
        BEGIN
            SET @failedToQualify = 1
        END
    END

    -- The account must have never had a status code above '2'
    IF (@failedToQualify = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM Status
                   WHERE  AcctNo = @acctNo
                   AND    StatusCode NOT IN ('S','U','0','1','2'))
        BEGIN
            SET @failedToQualify = 1
        END
    END

    -- The minimumn number of instalments must be paid
    IF (@failedToQualify = 0)
    BEGIN
        SELECT @instalmentsPaid = ROUND(SUM(-f.TransValue / ip.InstalAmount),0,1),
               @instalAmount = ip.InstalAmount
        FROM   Instalplan ip, FinTrans f
        WHERE  ip.AcctNo = @acctNo
        AND    ip.InstalAmount >= 0.01
        AND    f.AcctNo = ip.AcctNo
        AND    f.TransTypeCode IN ('PAY','DDN','DDR','DDE')
        GROUP BY ip.InstalAmount

        IF @ConsecutiveInstalments > ISNULL(@instalmentsPaid,0)
            SET @failedToQualify = 1
    END

    IF (@failedToQualify = 0)
    BEGIN
        SET @voucherAmount = ROUND((@instalAmount / 4.33),0,1)
    END

    SET @return = @@error
    SET NOCOUNT OFF
END
GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO
