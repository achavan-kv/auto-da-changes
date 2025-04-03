SET QUOTED_IDENTIFIER ON 
GO
SET ANSI_NULLS ON 
GO

if exists (select * from dbo.sysobjects where id = object_id('[dbo].[DN_AccountUpdateStatusSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_AccountUpdateStatusSP]
GO

/****** Object:  StoredProcedure [dbo].[DN_AccountUpdateStatusSP]    Script Date: 11/05/2007 11:01:21 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE PROCEDURE [dbo].[DN_AccountUpdateStatusSP]

--------------------------------------------------------------------------------
--
-- Project      : eCoSACS ? 2003 Strategic Thought Ltd.
-- File Name    : DN_AccountUpdateStatusSP.sql
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Update account status on HP and RF accounts
-- Author       : D Richardson
-- Date         : 25 Feb 2003
--
--
-- Change Control
-- --------------
-- Date    ByDescription
-- ----    -------------
-- 17/04/09  jec CR976 Refinance Arrangements - cater for RFN/RFD transtypes
-- 24/02/11  jec CR1090 Instalment waiver
-- 11/03/11  ip #3308 - Do not update status to 1 if instalment is waived but a deposit is required.
-- 09/10/12  jec #10138 - LW75030 - SUCR - Cash Loan - include CLD as delivery. 
--------------------------------------------------------------------------------

    -- Parameters
    
    @acctno     varchar(12),
    @return     int OUTPUT

AS
BEGIN

    SET NOCOUNT ON

    SET @Return = 0;

    DECLARE    @deposit money
    DECLARE    @instalment money
    DECLARE    @transvalue money
    DECLARE    @currstatus char(1)
    DECLARE    @newstatus char(1)
    DECLARE    @instalpredel char(1)
    DECLARE    @accttype char(1)
    DECLARE    @InstalWaived int


    -- Do not change the status if the account status is not
    -- currently '0' or 'U' or the account type is neither HP nor RF 
    SELECT  @currstatus = A.currstatus,
            @accttype   = AT.accttype
    FROM    acct A, accttype AT
    WHERE   A.acctno        = @acctno     
    AND     AT.genaccttype  = A.accttype

    IF (@accttype IN ('C', 'S', 'L')    /* Acct Type Translation DSR 29/9/03 */
        OR @currstatus NOT IN ('0', 'U', '1'))
    BEGIN
        return; 
    END

    -- Check if stage one sanctioning is complete
    IF EXISTS
        (SELECT 1
         FROM   proposal P, proposalflag PF
         WHERE  P.acctno        = @acctno
         AND    PF.acctno     = P.acctno
         AND    PF.checktype    = 'S1'
         AND    PF.datecleared  IS NOT NULL )
    BEGIN
        -- Get all the required variables
        SELECT  @deposit        = AG.deposit,
                @instalment     = IP.instalamount,
                @instalpredel   = TT.instalpredel,
                @InstalWaived   = IP.InstalmentWaived		--CR1090
        FROM    acct A, agreement AG, instalplan IP, termstypetable TT
        WHERE   A.acctno        = @acctno
        AND     AG.acctno       = A.acctno
        AND     IP.acctno       = A.acctno
        AND     TT.termstype    = A.termstype

        SELECT  @transvalue = SUM(transvalue)
        FROM    fintrans
        WHERE   acctno = @acctno 
        AND     transtypecode not in ('GRT','DEL','ADD','CRE','CRF','RFN','CLD')	-- #10138 --CR976 jec 17/04/09
        -- include Refinance Deposit in paid amount      CR976 jec 17/04/09
		SELECT  @transvalue = @transvalue + ISNULL(SUM(Transvalue), 0) 
		FROM    FinTrans 
		WHERE   AcctNo = @AcctNo 
		AND     TransTypeCode IN
				('RFD') 
		and transvalue<0 

        -- Init to the current status
        SET @newstatus = @currstatus

        IF (@instalpredel = 'N' AND @deposit = 0)
        OR (@instalpredel = 'N' AND isnull(@transvalue * -1,0) >= @deposit)
        OR (@instalpredel = 'Y' AND isnull(@transvalue * -1,0) >= @instalment)
        OR (@InstalWaived=1 AND isnull(@transvalue * -1,0) >= @deposit) --IP - 11/03/11 - #3308
        BEGIN
            -- Either no instalment nor deposit required
            -- OR the deposit is paid
            -- OR the instalment is paid
            SET @newstatus = '1'
        END
        ELSE
        BEGIN
            SET @newstatus = 'U'
        END

        IF @newstatus != @currstatus
        BEGIN
            -- Update account status
            UPDATE acct
            SET    currstatus = @newstatus 
            WHERE  acctno = @acctno
        END

    END


    SET @Return = @@ERROR
    
END

