SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[DN_TermsTypeDetailsGetSP]') and OBJECTPROPERTY(id, 'IsProcedure') = 1)
drop procedure [dbo].[DN_TermsTypeDetailsGetSP]
GO


CREATE PROCEDURE dbo.DN_TermsTypeDetailsGetSP
    @countryCode VARCHAR(2),
    @termsType VARCHAR(2),
    @AcctNo CHAR(12),
    @OverrideBand VARCHAR(4),
    @dateOpened datetime,
    @return int OUTPUT
AS

    SET @return = 0            --initialise return code
    
    DECLARE @mthsintfree smallint
    DECLARE @servpcent float
    DECLARE @depositpaid char
    DECLARE @instalpredel char
    DECLARE @dtnetfirstin char
    DECLARE @minterm int
    DECLARE @maxterm int
    DECLARE @defaultdeposit float
    DECLARE @defaultterm int
    DECLARE @affinity char
    DECLARE @description VARCHAR(20)
    DECLARE @apr VARCHAR(6)
    DECLARE @inspcent float
    DECLARE @adminpcent float
    DECLARE @insincluded smallint
    DECLARE @includewarranty smallint
    DECLARE @paymentholidays smallint
    DECLARE @paymentholidaymin smallint
    DECLARE @agrtext VARCHAR(400)
    DECLARE @delnonstocks smallint
    DECLARE @CashBackMonth smallint
    DECLARE @CashBackPc smallint
    DECLARE @CashBackAmount float
    DECLARE @AgreementPrint VARCHAR(4)
    DECLARE @AgreementPrintDesc VARCHAR(128)
    DECLARE @DeferredMonths smallint
    DECLARE @FullRebateDays smallint
    DECLARE @STCPc smallint
    DECLARE @STCAmount float,@avvtt smallint
    DECLARE @ScoringBand VARCHAR(4)
    DECLARE @dateRevised datetime
    DECLARE @rowCount INTEGER
	DECLARE @IsLoan bit
    DECLARE @AdminValue float

    
    Select @avvtt= allow from avvtt


    -- Scoring Band
    IF (@OverrideBand = '')
    BEGIN
        SELECT  @ScoringBand = ISNULL(ScoringBand,'')
        FROM    Proposal
        WHERE   AcctNo = @AcctNo
    END
    ELSE
        SET @ScoringBand = @OverrideBand


    -- CR806 - IntrateHistory columns are now EXCLUDED from this select.
    -- Therefore TermsTypeTable is queried instead of the view to ignore the
    -- bands on IntrateHistory and to return historical Terms Types.
    SELECT  @mthsintfree        = t.mthsintfree,
            --@servpcent          = servpcent,
            @depositpaid        = t.depositpaid,
            @instalpredel       = t.instalpredel,
            @dtnetfirstin       = t.dtnetfirstin,
            @minterm            = t.minterm,
            @maxterm            = t.maxterm,
            @defaultdeposit     = t.defaultdeposit,
            @defaultterm        = t.defaultterm,
            @affinity           = t.affinity,
            @description        = t.description,
            @apr                = t.apr,
            --@inspcent           = inspcent,
            --@adminpcent         = adminpcent,
            --@insincluded        = insincluded,
            --@includewarranty    = includewarranty,
            @paymentholidays    = t.paymentholidays,
            @agrtext            = t.agrtext,
            @delnonstocks       = t.delnonstocks,
            @CashBackMonth      = t.CashBackMonth,
            @CashBackPc         = t.CashBackPc,
            @CashBackAmount     = t.CashBackAmount,
            @AgreementPrint     = t.AgreementPrint,
            @AgreementPrintDesc = c.CodeDescript,
            @DeferredMonths     = t.DeferredMonths,
            @FullRebateDays     = t.FullRebateDays,
            @STCPc              = t.STCPc,
            @STCAmount          = t.STCAmount,
			@IsLoan				= t.IsLoan
    FROM    termstypetable t
    LEFT OUTER JOIN Code c 
    ON c.code = t.AgreementPrint and c.category = 'AGT'
    WHERE   t.termstype   = @termsType
    AND     t.countrycode = @countryCode



    -- HISTORY values from IntrateHistory
    -- A history record can only be returned if the date acct opened falls within a date range
    SELECT  @inspcent           = inspcent,
            @adminpcent         = adminpcent,
            @insincluded        = insincluded,
            @includewarranty    = includewarranty,
            @servpcent          = intrate,
            @paymentholidaymin  = paymentholidaymin,
            @AdminValue         = AdminValue
    FROM    intratehistory
    WHERE   termstype = @termsType
    AND     band      = @ScoringBand
    AND     (@dateOpened between datefrom and dateto
             OR (@dateOpened >= datefrom AND dateto = CONVERT(DATETIME, '01-Jan-1900',106)))

    SET @rowCount = @@ROWCOUNT

    IF (@rowCount = 0)
    BEGIN
        -- If the history record is not found then we have to get the record at the last date of revision.
        -- This can happen because revise agreement still uses date acct open after the account has
        -- been revised to a new terms type. In some cases (especially scoring bands) the history record
        -- may exist at the date of revision but not at the date acct opened. (The last date of revision
        -- might not be when the terms type was changed, but at least it must have a record at that date.)
        
        SELECT  @dateRevised = MAX(DateAgrmtRevised)
        FROM    RevisedHist
        WHERE   AcctNo = @AcctNo
        
        SELECT  @inspcent           = inspcent,
                @adminpcent         = adminpcent,
                @insincluded        = insincluded,
                @includewarranty    = includewarranty,
                @servpcent          = intrate,
                @paymentholidaymin  = paymentholidaymin,
                @AdminValue         = AdminValue
        FROM    intratehistory
        WHERE   termstype = @termsType
        AND     band      = @ScoringBand
        AND     (@dateRevised between datefrom and dateto
                 OR (@dateRevised >= datefrom AND dateto = CONVERT(DATETIME, '01-Jan-1900',106)))

        SET @rowCount = @@ROWCOUNT
    END
    
    
    IF (@rowCount = 0)
    BEGIN
        -- In case there was no revision history for this account
        
        SELECT  @dateRevised = MAX(DateFrom)
        FROM    intratehistory
        WHERE   termstype = @termsType
        AND     band      = @ScoringBand
        AND     DateFrom <= GETDATE()
        
        SELECT  @inspcent           = inspcent,
                @adminpcent         = adminpcent,
                @insincluded        = insincluded,
                @includewarranty    = includewarranty,
                @servpcent          = intrate,
                @paymentholidaymin  = paymentholidaymin,
                @AdminValue         = AdminValue
        FROM    intratehistory
        WHERE   termstype = @termsType
        AND     band      = @ScoringBand
        AND     DateFrom  = @dateRevised
    END


    --
    -- If an IntrateHistory record was NOT found ISNULL will return zero
    --
    SELECT  @mthsintfree                    as mthsintfree,
            ISNULL(@servpcent,0)            as servpcent,
            @depositpaid                    as depositpaid,
            @instalpredel                   as instalpredel,
            @dtnetfirstin                   as dtnetfirstin,
            @minterm                        as minterm,
            @maxterm                        as maxterm,
            @defaultdeposit                 as defaultdeposit,
            @defaultterm                    as defaultterm,
            @affinity                       as affinity,
            @description                    as description,
            @apr                            as apr,
            @CashBackMonth                  as CashBackMonth,
            @CashBackPc                     as CashBackPc,
            @CashBackAmount                 as CashBackAmount,
            @AgreementPrint                 as AgreementPrint,
            @AgreementPrintDesc             as AgreementPrintDesc,
            @DeferredMonths                 as DeferredMonths,
            @FullRebateDays                 as FullRebateDays,
            @STCPc                          as STCPc,
            @STCAmount                      as STCAmount,
            ISNULL(@inspcent,0)             as inspcent,
            ISNULL(@adminpcent,0)           as adminpcent,
            ISNULL(@insincluded,0)          as insincluded,
            ISNULL(@includewarranty,0)      as includewarranty,
            @paymentholidays                as paymentholidays,
            ISNULL(@paymentholidaymin,0)    as paymentholidaymin,
            @agrtext                        as agrtext,
            ISNULL(@delnonstocks,0)         as delnonstocks,
            @avvtt                          as avvtt,
            @ScoringBand                    as Band,
			@IsLoan							as IsLoan,
            ISNULL(@AdminValue,0)           as AdminValue


    SET @return = @@error

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO