Drop PROCEDURE App_TransactData
go

CREATE PROCEDURE App_TransactData
--------------------------------------------------------------------------------
-- *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
-- John Croft - Courts International IT
-- This procedure is a copy of sp_TransactData but without the final Select statement
-- It is executed in the ScorexApplication procedure.
-- *-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-*-
-- Calculate Customer Account data for the Transact input buffer.
-- The stored procedure Transact_CustStatus is called to return values
-- for each customer.
--
-- Change Control
-- --------------
-- Date     By   Description
-- ----     --   -----------
-- 01/11/01 AA   CR376 Transact change
-- 20/11/01 AA   Load up the monthly income from the proposal table rather
--               than the employment table
-- 05/12/01 DSR  Correction to instalment as % of net monthly income
-- 06/12/01 DSR  FR75479 Transact UAT e89
-- 11/12/01 AA   FR76557 Do not include cancelled accounts for status checks
-- 07/05/02 DSR  CR262 / FR76209 Calculate Weighted Averages with Highest Status
-- 17/06/02 DSR  Literal AcctNo removed from Most Expensive Product query
--09/07/03 AA-instalment income ratio change
--29/07/03 AA-- again including other commitments this time.
-- AA- 11/09/03 removing rent from income to produce disposable income
--------------------------------------------------------------------------------

    -- Parameters
    @AcctNo             char(12),
    -- Local variables
    -- Account being sanctioned
    @ProdCat            varchar(3) = null output,
    @ProdCode           varchar(8)  = null output,
    @LoanAmt            MONEY  = null output,
    @TotValue           MONEY  = null output,
    @DepPercent         FLOAT  = null output,
    @InstalPercent      FLOAT  = null output,

    -- MAIN Applicant
    @CurNumAcc          INTEGER  = null output,
    @SetNumAcc          INTEGER  = null output,
    @CurRecent          char(1) = null output,
    @SetRecent          char(1) = null output,
    @CurHiEver          char(1) = null output,
    @SetHiEver          char(1) = null output,
    @CurHiNow           char(1) = null output,
    @SetHiNow           char(1) = null output,
    @CurWeightAvg       FLOAT = null output,
    @SetWeightAvg       FLOAT = null output,
    @SetLargest         char(1) = null output,
    @SetLargestSize     char(1) = null output,
    @TotalInstal        MONEY = null output,
    @TotalOutStBal      MONEY = null output,
    @NumAppsLst90       SMALLINT = null output,
    @RejLst90           char(1) = null output,

    -- JOINT Applicant
    @jCurNumAcc         INTEGER = null output,
    @jSetNumAcc         INTEGER = null output,
    @jCurRecent         char(1) = null output,
    @jSetRecent         char(1) = null output,
    @jCurHiEver         char(1) = null output,
    @jSetHiEver         char(1) = null output,
    @tempchar           char(1) = null output,
    @tempfloat          FLOAT = null output, 
    @jSetLargest        char(1) = null output,
    @jSetLargestSize    char(1) = null output,
    @jTotalInstal       MONEY = null output,
    @jTotalOutStBal     MONEY = null output,
    @jNumAppsLst90      SMALLINT = null output,
    @jRejLst90          char(1) = null output,
    @CustId             varchar(20)= null output,
    @jCustId            varchar(20)= null output,
    @noselect           tinyint =0
AS DECLARE
    -- The above variables are for the procedure results
    -- The below variables are just for the local procedure

    @SQLError           INTEGER,
    @CurObjectName      varchar(3),
    @DateProp           DATETIME,
    @MonthlyGross       MONEY,
    @OtherPmnts         MONEY,
    @MthlyRent          MONEY,
    @InstalAmount       MONEY,
    @OrdVal             FLOAT,
    @NetIncome       MONEY

    
BEGIN

    SET NOCOUNT ON

    -- Defaults if no values to retrieve
    SET @ProdCat       = ' '
    SET @ProdCode      = ' '
    SET @LoanAmt       = 0.0
    SET @TotValue      = 0.0
    SET @DepPercent    = 0.0
    SET @InstalPercent = 0.0

    -- Current Object Name for error logging
    SET @CurObjectName = '(SP) sp_TransactData'


    /* MAIN Customer Id on this account */
    Select @CustId = CustId
    From   custacct
    Where  acctno   = @acctno
    and    hldorjnt = 'H'

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select MAIN Customer Id',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    /* JOINT (or Spouse) Customer Id on this account (if any) */
    Select @jCustId = CustId
    From   custacct
    Where  acctno   = @acctno
    and    hldorjnt IN ('J','S')
    
    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select JOINT Customer Id',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    /* Date of this credit application */
    select @DateProp = Max(DateProp)
    from   Proposal
    where  CustId = @CustId

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select latest DateProp',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    /* 118 Product Category and Code of most expensive item on account */
    SELECT TOP 1
           @ProdCat   = ISNULL(s.Category, ' '),
           @ProdCode  = ISNULL(s.ItemNo, ' '),
           @OrdVal    = ISNULL(l.OrdVal, 0)
    FROM   LineItem l, StockItem s
    WHERE  l.AcctNo    = @AcctNo
    AND    s.ItemNo    = l.ItemNo
    AND    s.StockLocn = l.StockLocn
    AND    s.ItemType  = 'S'
    ORDER BY l.OrdVal DESC

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select most expensive product',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    /* 122 Loan Amount of stock items */
    Select @LoanAmt = ISNULL(SUM(ordval),0)
    from   lineitem l, stockitem s
    where  l.acctno    = @AcctNo
    and    s.itemno    = l.itemno
    and    s.stocklocn = l.stocklocn
    and    s.itemtype  = 'S'

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select Loan Amount',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    /* 123 Total Value excluding service charge */
    Select @TotValue = ISNULL(agrmttotal - servicechg, 0)
    from   agreement
    where  acctno = @acctno
    
    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select Total Value',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    /* 129 */
    /* Deposit as % of Agreement Total */
    Select @DepPercent = ISNULL(deposit/agrmttotal * 100, 0)
    From   agreement
    Where  acctno = @acctno
    and    agrmttotal > 0

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select Deposit %',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    /* 130 */
    /* Monthly Installment as % of NET Monthly Income */
    Select @MonthlyGross = ISNULL(mthlyincome,0) + isnull (addincome,0),
           @OtherPmnts   = ISNULL(OtherPmnts,0) + isnull (commitments1, 0) +
                            isnull (commitments2, 0) +  isnull (commitments3, 0) 
    from   proposal  
    where  CustId   = @CustId
    and    dateprop = @dateprop

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select Monthly Gross',                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    Select @InstalAmount = ISNULL(InstalAmount,0)
    from   instalplan
    where  acctno = @acctno

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select Instalment Amount',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END


    Select @MthlyRent = ISNULL(MthlyRent,0)
    from   custaddress
    where  CustId = @CustId
    and    addtype = 'H'
    and    datemoved is Null

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Select Monthly Rent',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END

    SET @NetIncome = @MonthlyGross - @OtherPmnts-@MthlyRent
    IF @NetIncome > 0 and @InstalAmount > 0
    BEGIN
        Set @InstalPercent =  @InstalAmount / @NetIncome * 100
    END
    ELSE
    BEGIN
        Set @InstalPercent = 2
    END

    ------------------------------------------------------------------------------


    /* CR262 New status calculation for MAIN Applicant */
    EXECUTE @SQLError = sp_Transact_CustStatus
        @piCustId           = @CustId,
        @piAcctNo           = @AcctNo,
        @poCurNumAcc        = @CurNumAcc        OUTPUT,
        @poSetNumAcc        = @SetNumAcc        OUTPUT,
        @poCurRecent        = @CurRecent        OUTPUT,
        @poSetRecent        = @SetRecent        OUTPUT,
        @poCurHiEver        = @CurHiEver        OUTPUT,
        @poSetHiEver        = @SetHiEver        OUTPUT,
        @poCurHiNow         = @CurHiNow         OUTPUT,
        @poSetHiNow         = @SetHiNow         OUTPUT,
        @poCurWeightAvg     = @CurWeightAvg     OUTPUT,
        @poSetWeightAvg     = @SetWeightAvg     OUTPUT,
        @poSetLargest       = @SetLargest       OUTPUT,
        @poSetLargestSize   = @SetLargestSize   OUTPUT,
        @poTotalInstal      = @TotalInstal      OUTPUT,
        @poTotalOutStBal    = @TotalOutStBal    OUTPUT,
        @poNumAppsLst90     = @NumAppsLst90     OUTPUT,
        @poRejLst90         = @RejLst90         OUTPUT

    IF (@SQLError <> 0) RETURN @SQLError
-- set values to 0 if ='N'
If @CurHiEver='N' set @CurHiEver='0'
If @SetHiEver='N' set @SetHiEver='0'
If @CurHiNow='N' set @CurHiNow='0'
If @SetHiNow='N' set @SetHiNow='0'
If @SetLargest='N' set @SetLargest='0'
	
    IF ISNULL(@jCustId,'') <> ''
    BEGIN
        /* CR262 New status calculation for JOINT Applicant */
        EXECUTE @SQLError = sp_Transact_CustStatus
            @piCustId           = @jCustId,
            @piAcctNo           = @AcctNo,
            @poCurNumAcc        = @jCurNumAcc       OUTPUT,
            @poSetNumAcc        = @jSetNumAcc       OUTPUT,
            @poCurRecent        = @jCurRecent       OUTPUT,
            @poSetRecent        = @jSetRecent       OUTPUT,
            @poCurHiEver        = @jCurHiEver       OUTPUT,
            @poSetHiEver        = @jSetHiEver       OUTPUT,
            @poCurHiNow         = @tempchar         OUTPUT,
            @poSetHiNow         = @tempchar         OUTPUT,
            @poCurWeightAvg     = @tempfloat        OUTPUT,
            @poSetWeightAvg     = @tempfloat        OUTPUT,
            @poSetLargest       = @jSetLargest      OUTPUT,
            @poSetLargestSize   = @jSetLargestSize  OUTPUT,
            @poTotalInstal      = @jTotalInstal     OUTPUT,
            @poTotalOutStBal    = @jTotalOutStBal   OUTPUT,
            @poNumAppsLst90     = @jNumAppsLst90    OUTPUT,
            @poRejLst90         = @jRejLst90        OUTPUT
            
        IF (@SQLError <> 0) RETURN @SQLError
    END
    ELSE
    BEGIN
        -- No JOINT Applicant
        SET @jCurNumAcc         = 0        SET @jSetNumAcc         = 0
        SET @jCurRecent         = ' '
        SET @jSetRecent         = ' '
        SET @jCurHiEver         = ' '
        SET @jSetHiEver         = ' '
        SET @jSetLargest = ' '
        SET @jSetLargestSize    = ' '        SET @jTotalInstal       = 0
        SET @jTotalOutStBal     = 0
        SET @jNumAppsLst90      = 0
        SET @jRejLst90          = ' '
    END


    /* now storing this information on a separate table later to be used
       for extract into scorexdata */
    if @CustId   is null set @CustId   = ' '
    if @dateprop is null set @dateprop = getdate()
        
    delete from Scoretrak
    where acctno   = @acctno
    and   CustId   = @CustId
    and   dateprop = @dateprop

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Delete from ScoreTrak',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END
 /* jec 07/06/04
 if @noselect= 0

  begin
    insert into scoretrak
       (TotValue,
        LoanAmt,
        ProdCat,
        ProdCode,
        worstcurrent,
        statmstreccur,
        worstsettled,
        statmstrecset,
        statlargeset,
        sizelargeset,
        existinstalls,
        existbalance,
        numcurrent,
        numsettled,
        Deptotamt,
        installinc,
        jworstcurrent,
        jstatmstreccur,
        jworstsettled,
        jstatmstrecset,
        jstatlargeset,
        jsizelargeset,
        jexistinstalls,
        jexistbalance,
        jnumcurrent,
        jnumsettled,
        NumAppsLst90,
        RejLst90,
        jNumAppsLst90,
        jRejLst90,
        CustId,
        acctno,
        dateprop)
    values
        (@TotValue,
         @LoanAmt,
         @ProdCat,
         @ProdCode,
         @CurHiEver,
         @CurRecent,
         @SetHiEver,
         @SetRecent,
         @SetLargest,
         @SetLargestSize,
         @TotalInstal,
         @TotalOutStBal,
         @CurNumAcc,
         @SetNumAcc,
         @DepPercent,
         @InstalPercent,
         @jCurHiEver,
         @jCurRecent,
         @jSetHiEver,
         @jSetRecent,
         @jSetLargest,
         @jSetLargestSize,
         @jTotalInstal,
         @jTotalOutStBal,
         @jCurNumAcc,
         @jSetNumAcc,
         @NumAppsLst90,
         @RejLst90,
         @jNumAppsLst90,
         @jRejLst90,
         @CustId,
         @acctno,
         @dateprop )

    SET @SQLError = @@ERROR
    IF (@SQLError <> 0)
    BEGIN
        EXECUTE sp_LogErrorMsg @piErrNo      = @SQLError,
                               @piMsg        = 'Insert into ScoreTrak',
                               @piObjectName = @CurObjectName
        RETURN @SQLError
    END 

 end 
*/   
 if @noselect= 0
 /*   Select
        @TotValue           AS TotValue,
        @LoanAmt            AS LoanAmt,
        @ProdCat            AS ProdCat ,
        @ProdCode           AS ProdCode,
        @CurHiEver          AS worstcurrent ,
        @CurRecent          AS statmstreccur,
        @SetHiEver          AS worstsettled,
        @SetRecent          AS statmstrecset,
        @SetLargest         AS statlargeset ,
        @SetLargestSize     AS sizelargeset,
        @TotalInstal        AS existinstalls,
        @TotalOutStBal      AS existbalance,
        @CurNumAcc          AS CurNumAcc,
        @SetNumAcc          AS SetNumAcc,
        @DepPercent         AS Deptotamt,
        @InstalPercent      AS Installinc,
        @jCurHiEver         AS jworstcurrent,
        @jCurRecent         AS jstatmstreccur,
        @jSetHiEver         AS jworstsettled,
        @jSetRecent         AS jstatmstrecset,
        @jSetLargest        AS jstatlargeset,
        @jSetLargestSize    AS jsizelargeset,
        @jTotalInstal       AS jexistinstalls,
        @jTotalOutStBal     AS jexistbalance,
        @jCurNumAcc         AS jCurNumAcc,
        @jSetNumAcc         AS jSetNumAcc,
        @NumAppsLst90       AS NumAppsLst90,
        @RejLst90           AS RejLst90,
        @jNumAppsLst90      AS jNumAppsLst90,        @jRejLst90          AS jRejLst90,
        @CurHiNow           AS CurHiNow,
        @SetHiNow           AS SetHiNow,
        @CurWeightAvg       AS CurWeightAvg,
        @SetWeightAvg       AS SetWeightAvg
*/
        RETURN 0
END



GO
