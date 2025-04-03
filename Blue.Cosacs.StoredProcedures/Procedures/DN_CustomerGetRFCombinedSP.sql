
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

IF  EXISTS (SELECT 1 
	FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[DN_CustomerGetRFCombinedSP]') 
	AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[DN_CustomerGetRFCombinedSP]

GO

CREATE PROCEDURE [dbo].[DN_CustomerGetRFCombinedSP]
    @custid                         VARCHAR(20),
    @available_credit               MONEY       OUT,
    @cardprinted                    CHAR(1)    OUT,
    @total_agreements               MONEY       OUT,
    @total_arrears                  MONEY       OUT,
    @total_balances                 MONEY       OUT,
    @total_credit                   MONEY       OUT,
    @total_delivered_instalments    MONEY       OUT,
    @total_all_instalments          MONEY       OUT,
    @rfcardseqno                    TINYINT     OUT,
    @datenextpaymentdue             DATETIME    OUT,
    @return                         INT         OUT

AS
    SET @return = 0             --initialise return code

    SELECT  @total_credit = RFCreditLimit,
            @cardprinted  = RFCardPrinted,
            @rfcardseqno  = rfcardseqno
    FROM    customer
    WHERE   custid = @custid

    SET @return = @@ERROR

    IF @return = 0
    BEGIN
        SELECT  @total_agreements            = ISNULL(SUM(acct.agrmttotal),0),
                @total_balances              = ISNULL(SUM(acct.outstbal),0),
                @total_arrears               = ISNULL(SUM(acct.arrears),0),
                @total_delivered_instalments = ISNULL(SUM(instalplan.instalamount),0)
        FROM    custacct
        INNER JOIN acct ON acct.acctno = custacct.acctno
        INNER JOIN instalplan ON instalplan.acctno = acct.acctno
        INNER JOIN agreement ON agreement.acctno = acct.acctno
        WHERE   custid = @custid 
        AND     acct.accttype NOT IN ('C', 'S')
        AND     custacct.hldorjnt      = 'H'
        AND     acct.outstbal          > 0
        AND     agreement.deliveryflag = 'Y'

        SET @return = @@ERROR
    END

    IF @return = 0
    BEGIN
        SELECT  @total_all_instalments = ISNULL(SUM(instalplan.instalamount),0)
        FROM    custacct
        INNER JOIN acct ON acct.acctno = custacct.acctno
        INNER JOIN instalplan ON instalplan.acctno =acct.acctno
        INNER JOIN agreement ON agreement.acctno =acct.acctno
        WHERE   custid = @custid 
        AND     acct.accttype        = 'R' 
        AND     custacct.hldorjnt    = 'H'
        AND     acct.currstatus     != 'S'
        --AND agreement.deliveryflag = 'Y' 

        -- Removed delivery flag condition so that this account 
        -- would appear on the ready finance printout
        -- Sure I have reinstated this before, cannot remember why 
        -- we may need a new variable to cater for different circumstances

        SET @return = @@ERROR
    END

    IF @return = 0
    BEGIN
        EXECUTE DN_CustomerGetRFLimitSP
            @custid     = @custid,
            @AcctList   = '',
            @limit      = @total_credit OUTPUT,
            @available  = @available_credit OUTPUT,
            @return     = @return OUTPUT

        SET @return = @@ERROR
    END


    -- getting earliest next payment due account
    DECLARE @firstdayofthismonth    DATETIME,
            @nextdueday             SMALLINT

    SELECT  instalplan.datefirst,
            acct.arrears,
            instalplan.acctno
    INTO    #accounts
    FROM    instalplan
	INNER JOIN proposal ON instalplan.acctno = proposal.acctno
	INNER JOIN acct ON acct.acctno       = proposal.acctno
    WHERE   proposal.custid   = @custid
    --AND     acct.acctno       = proposal.acctno
    --AND     instalplan.acctno = proposal.acctno
    AND     acct.outstbal     > 0
    AND     acct.arrears      > -instalplan.instalamount     -- exclude accounts 1 month in advance


    -- Only get a next due date if the customer has an account with a due date
    IF @@ROWCOUNT > 0
    BEGIN
        SET @firstdayofthismonth = DATEADD(DAY, -DAY(GETDATE())+1, GETDATE())

        -- Get next earliest due date this month
        SELECT  @nextdueday = ISNULL(MIN(DAY(datefirst)),0)
        FROM    #accounts
        WHERE   DAY(datefirst) >= DAY(GETDATE())

        SET @datenextpaymentdue = DATEADD(DAY, @nextdueday-1, @firstdayofthismonth)

        IF @nextdueday = 0      --none this month so try next month
        BEGIN
            SELECT  @nextdueday = ISNULL(MIN(DAY(datefirst)),0)
            FROM    #accounts   
            
            SET @datenextpaymentdue = DATEADD(DAY, @nextdueday-1, @firstdayofthismonth)
            SET @datenextpaymentdue = DATEADD(MONTH, 1, @datenextpaymentdue)
        END

        -- now remove hours minutes and seconds
        SET @datenextpaymentdue = ISNULL(CONVERT(DATETIME,CONVERT(VARCHAR(11),@datenextpaymentdue)),'01-jan-1900')
    END
    ELSE
    BEGIN
        -- No accounts with a due date
        SET @datenextpaymentdue = CONVERT(DATETIME, '1 Jan 1900', 106)
    END

GO
SET QUOTED_IDENTIFIER OFF 
GO
SET ANSI_NULLS ON 
GO

