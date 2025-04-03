
IF EXISTS (	SELECT	1 
			FROM	dbo.sysobjects
			WHERE	id = object_id('[dbo].[DN_TermsTypeSaveDetailsSP]') 
					AND OBJECTPROPERTY(id, 'IsProcedure') = 1)
BEGIN
	DROP PROCEDURE [dbo].[DN_TermsTypeSaveDetailsSP]
END
GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Modified By:	Jez Hemans
-- Modified On date: 26/07/2007
-- Description:	CR903 - Additional field 'StoreType' to be saved to termstypetable as well
-- =============================================

CREATE PROCEDURE [dbo].[DN_TermsTypeSaveDetailsSP]
-- ================================================
-- Project      : CoSACS .NET
-- File Name    : DN_TermsTypeSaveDetailsSP.prc
-- File Type    : MSSQL Server Stored Procedure Script
-- Title        : Save TermsType Details
-- Author       : ??
-- Date         : ??
--
-- This procedure will save the basic TermsType Details
-- 
-- Change Control
-- --------------
-- Date			By		Description
-- ----			--		-----------
-- 05 Sep 2007	jec		CR906 changes
-- 01 Jul 2020  Amit	CR - MMI Implementation
-- ================================================
	-- Add the parameters for the stored procedure here
    @termstype VARCHAR(4),
    @description varchar(20),
    @mthsintfree smallint,
    @instalpredel char(1),
    @affinity char(1),
    @noarrearsletter smallint,
    @defaultdeposit float,
    @depositispercentage bit,
    @isactive smallint,
    @countrycode char(1),
    @paymentholidays bit,
    @agrtext varchar(400),
    @minTerm smallint,
    @MaxTerm smallint,
    @CashBackMonth smallint,
    @CashBackPc smallint,
    @CashBackAmount float,
    @AgreementPrint varchar(80),
    @DeferredMonths smallint,
    @FullRebateDays smallint,
    @STCPc smallint,
    @STCAmount float,
    @defaultterm smallint,
    @delnonstocks smallint,
	@user int =0,
    @apr varchar(6),
    @donotsecuritise smallint,
	@storeType CHAR(1),
    @loanNewCustomer bit,
    @loanRecentCustomer bit,
    @loanExistingCustomer bit,
    @loanStaff bit,
	@isLoan bit,		-- CR906
	@IsMmiActive BIT,
	@MmiThresholdPercentage float,
    --@pClubTier1 char(1),
    --@pClubTier2 char(1),
    @return int OUTPUT

AS

    SET @return = 0            --initialise return code
    declare
    @Pdescription varchar(20),
    @Pmthsintfree smallint,
    @Pinstalpredel char(1),
    @Paffinity char(1),
    @Pnoarrearsletter smallint,
    @Pdefaultdeposit float,
    @Pdepositispercentage bit,
    @Pisactive smallint,
    @Pcountrycode char(1),
    @Ppaymentholidays bit,
    @Pagrtext varchar(400),
    @PminTerm smallint,
    @PMaxTerm smallint,
    @PCashBackMonth smallint,
    @PCashBackPc smallint,
    @PCashBackAmount float,
    @PAgreementPrint varchar(80),
    @PDeferredMonths smallint,
    @PFullRebateDays smallint,
    @PSTCPc smallint,
    @PSTCAmount float,
    @Pdefaultterm smallint,
    @Pdelnonstocks smallint,
    @PstoreType CHAR(1),
	@PisLoan bit,		-- CR906
    @PloanNewCustomer bit,
    @PloanRecentCustomer bit,
    @PloanExistingCustomer bit,
    @PloanStaff bit,
    @genvar varchar(500),
	@isAmortized bit,
	@PIsMmiActive VARCHAR(500),
	@PMmiThresholdPercentage VARCHAR(500)


    SELECT
     @Pdescription=	isnull(	convert(varchar(500),	description),'')	,
     @Pmthsintfree=	isnull(	convert(varchar(500),	mthsintfree),'')	,
     @Pinstalpredel=	isnull(	convert(varchar(500),	instalpredel),'')	,
     @Paffinity=	isnull(	convert(varchar(500),	affinity),'')	,
     @Pnoarrearsletter=	isnull(	convert(varchar(500),	noarrearsletters),'')	,
     @Pdefaultdeposit=	isnull(	convert(varchar(500),	defaultdeposit),'')	,
     @Pdepositispercentage=	isnull(	convert(varchar(500),	depositispercentage),'')	,
     @Pisactive=	isnull(	convert(varchar(500),	isactive),'')	,
     @Pcountrycode=	isnull(	convert(varchar(500),	countrycode),'')	,
     @Ppaymentholidays=	isnull(	convert(varchar(500),	paymentholidays),'')	,
     @Pagrtext=	isnull(	convert(varchar(500),	agrtext),'')	,
     @PminTerm=	isnull(	convert(varchar(500),	minTerm),'')	,
     @PMaxTerm=	isnull(	convert(varchar(500),	MaxTerm),'')	,
     @PCashBackMonth=	isnull(	convert(varchar(500),	CashBackMonth),'')	,
     @PCashBackPc=	isnull(	convert(varchar(500),	CashBackPc),'')	,
     @PCashBackAmount=	isnull(	convert(varchar(500),	CashBackAmount),'')	,
     @PAgreementPrint=	isnull(	convert(varchar(500),	AgreementPrint),'')	,
     @PDeferredMonths=	isnull(	convert(varchar(500),	DeferredMonths),'')	,
     @PFullRebateDays=	isnull(	convert(varchar(500),	FullRebateDays),'')	,
     @PSTCPc=	isnull(	convert(varchar(500),	STCPc),'')	,
     @PSTCAmount=	isnull(	convert(varchar(500),	STCAmount),'')	,
     @Pdefaultterm=	isnull(	convert(varchar(500),	defaultterm),'')	,
     @Pdelnonstocks=	isnull(	convert(varchar(500),	delnonstocks),''),
     @PstoreType=	isnull(	convert(varchar(500),	StoreType),''),
     @PisLoan=	isnull(	convert(varchar(500),	IsLoan),''),			-- CR906
     @PloanNewCustomer=	isnull(	convert(varchar(500),	LoanNewCustomer),''),
     @PloanRecentCustomer=	isnull(	convert(varchar(500),	LoanRecentCustomer),''),
     @PloanExistingCustomer=	isnull(	convert(varchar(500),	LoanExistingCustomer),''),
     @PloanStaff=	isnull(	convert(varchar(500),	LoanStaff),''),
	 @PIsMmiActive = ISNULL(	CONVERT(VARCHAR(500), IsMmiActive),''),
     @PMmiThresholdPercentage = ISNULL(CONVERT(VARCHAR(500), MmiThresholdPercentage),'')
     FROM termstypetable 
     WHERE termstype = @termstype

     if @description !=@pdescription
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='description',
        @origvalue = @Pdescription,@newvalue = @description

     set @genvar = convert(varchar,@mthsintfree)
     if @genvar !=@Pmthsintfree
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='mthsintfree',
        @origvalue = @Pmthsintfree,@newvalue = @genvar

     set @genvar = convert(varchar,@instalpredel)
     if @genvar !=@Pinstalpredel
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='instalpredel',
        @origvalue = @Pinstalpredel,@newvalue = @genvar

     set @genvar = convert(varchar,@affinity)
     if @genvar !=@Paffinity
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='description',
        @origvalue = @Paffinity,@newvalue = @genvar

     set @genvar = convert(varchar,@noarrearsletter)
     if @genvar !=@Pnoarrearsletter
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='noarrearsletter',
        @origvalue = @Pnoarrearsletter,@newvalue = @genvar

     set @genvar = convert(varchar,@defaultdeposit)
     if @genvar !=@Pdefaultdeposit
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='defaultdeposit',
        @origvalue = @Pdefaultdeposit,@newvalue = @genvar

     set @genvar = convert(varchar,@depositispercentage)
     if @genvar !=@Pdepositispercentage
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='depositispercentage',
        @origvalue = @Pdepositispercentage,@newvalue = @genvar

     set @genvar = convert(varchar,@isactive)
     if @genvar !=@Pisactive
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='isactive',
        @origvalue = @Pisactive,@newvalue = @genvar

     set @genvar = convert(varchar,@countrycode)
     if @genvar !=@Pcountrycode
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='countrycode',
        @origvalue = @Pcountrycode,@newvalue = @genvar

     set @genvar = convert(varchar,@paymentholidays)
     if @genvar !=@Ppaymentholidays
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='paymentholidays',
        @origvalue = @Ppaymentholidays,@newvalue = @genvar
        
     set @genvar = convert(varchar,@agrtext)
     if @genvar !=@Pagrtext
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='agrtext',
        @origvalue = @Pagrtext ,@newvalue = @genvar     set @genvar = convert(varchar,@agrtext)

     set @genvar = convert(varchar,@minterm)
     if @genvar !=@PminTerm 
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='minTerm',
        @origvalue = @PminTerm,@newvalue = @genvar
        
     set @genvar = convert(varchar,@maxterm)
   if @genvar !=@PmaxTerm  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='maxTerm',
        @origvalue = @PmaxTerm ,@newvalue = @genvar

   set @genvar = convert(varchar,@CashBackMonth )
   if @genvar !=@PCashBackMonth  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='CashBackMonth',
        @origvalue = @PCashBackMonth ,@newvalue = @genvar

   set @genvar = convert(varchar,@CashBackPc )
   if @genvar !=@PCashBackPc  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='CashBackPc',
        @origvalue = @PCashBackPc ,@newvalue = @genvar

   set @genvar = convert(varchar,@CashBackAmount )
   if @genvar !=@PCashBackAmount 
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='CashBackAmount',
        @origvalue = @PCashbackAmount,@newvalue = @genvar

   set @genvar = convert(varchar,@AgreementPrint )
   if @genvar !=@PAgreementPrint 
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='agreementprint',
        @origvalue = @PAgreementprint,@newvalue = @genvar

   set @genvar = convert(varchar,@DeferredMonths )
   if @genvar !=@PDeferredMonths  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='DeferredMonths',
        @origvalue = @PDeferredMonths ,@newvalue = @genvar

   set @genvar = convert(varchar,@FullRebateDays )
   if @genvar !=@PFullRebateDays  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='FullRebateDays',
        @origvalue = @PFullRebateDays ,@newvalue = @genvar

   set @genvar = convert(varchar,@STCPc )
   if @genvar !=@PSTCPc  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='STCPc',
        @origvalue = @PSTCPc ,@newvalue = @genvar

   set @genvar = convert(varchar,@STCAmount )
   if @genvar !=@PSTCAmount  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='STCAmount',
        @origvalue = @PSTCAmount ,@newvalue = @genvar
        
   set @genvar = convert(varchar,@defaultterm )
   if @genvar !=@Pdefaultterm  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='defaultterm',
        @origvalue = @Pdefaultterm ,@newvalue = @genvar

   set @genvar = convert(varchar,@delnonstocks )
   if @genvar !=@Pdelnonstocks  
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='delnonstocks',
        @origvalue = @Pdelnonstocks ,@newvalue = @genvar


    set @genvar = convert(varchar,@storeType )
   if @genvar !=@PstoreType 
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='StoreType',
        @origvalue = @PstoreType ,@newvalue = @genvar

	 --CR906
	 set @genvar = convert(varchar,@isLoan)
     if @genvar !=@PisLoan
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='isLoan',
        @origvalue = @PisLoan,@newvalue = @genvar

     set @genvar = convert(varchar,@loanNewCustomer)
     if @genvar !=@PloanNewCustomer
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='LoanNewCustomer',
        @origvalue = @PloanNewCustomer,@newvalue = @genvar

     set @genvar = convert(varchar,@loanRecentCustomer)
     if @genvar !=@PloanRecentCustomer
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='LoanRecentCustomer',
        @origvalue = @PloanRecentCustomer,@newvalue = @genvar

    set @genvar = convert(varchar,@loanExistingCustomer)
     if @genvar !=@PloanExistingCustomer
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='LoanExistingCustomer',
        @origvalue = @PloanExistingCustomer,@newvalue = @genvar

    set @genvar = convert(varchar,@loanStaff)
     if @genvar !=@PloanStaff
	exec DN_TermsTypeHistorySaveSP @termstype =@termstype,@empeenochange = @user,@changedfield='LoanStaff',
        @origvalue = @PloanStaff,@newvalue = @genvar

	SET @genvar = CONVERT(VARCHAR,@IsMmiActive)
    IF (@genvar != @PIsMmiActive)
		EXEC DN_TermsTypeHistorySaveSP	@termstype = @termstype, @empeenochange = @user, @changedfield='IsMmiActive',
										@origvalue = @PIsMmiActive, @newvalue = @genvar

	SET @genvar = CONVERT(VARCHAR,@MmiThresholdPercentage)
    IF (@genvar != @PMmiThresholdPercentage)
		EXEC DN_TermsTypeHistorySaveSP	@termstype = @termstype, @empeenochange = @user, @changedfield='MmiThresholdPercentage',
										@origvalue = @PMmiThresholdPercentage, @newvalue = @genvar

	SELECT @isAmortized = Value FROM countrymaintenance WHERE CodeName='CL_Amortized'

    UPDATE  termstypetable
    SET     description = @description,
            mthsintfree = @mthsintfree,
            instalpredel = @instalpredel,
            affinity = @affinity,
            noarrearsletters = @noarrearsletter,
            defaultdeposit = @defaultdeposit,
            depositispercentage = @depositispercentage,
            isactive = @isactive,
            paymentholidays = @paymentholidays,
            agrtext = @agrtext,
            DefaultTerm = @defaultterm,
            MinTerm = @MinTerm,
            MaxTerm = @MaxTerm,
            CashBackMonth       = @CashBackMonth,
            CashBackPc          = @CashBackPc,
            CashBackAmount      = @CashBackAmount,
            AgreementPrint      = @AgreementPrint,
            DeferredMonths      = @DeferredMonths,
            FullRebateDays      = @FullRebateDays,
            STCPc               = @STCPc,
            STCAmount           = @STCAmount,
            delnonstocks		= @delnonstocks,
            apr					= @apr,
            DoNotSecuritise		= @donotsecuritise,
            StoreType           = @storeType,
            isLoan				= @isLoan,			-- CR906
            LoanNewCustomer     = @loanNewCustomer,
            LoanRecentCustomer  = @loanRecentCustomer,
            LoanExistingCustomer = @loanExistingCustomer,
            LoanStaff           = @loanStaff,
			IsMmiActive			= @IsMmiActive,
			MmiThresholdPercentage = @MmiThresholdPercentage
            --PClubTier1			= @pClubTier1,
            --PClubTier2			= @pClubTier2
			
    WHERE   termstype = @termstype

    IF(@@rowcount = 0)
    BEGIN
        INSERT
        INTO    termstypetable
                (termstype, description, mthsintfree, instalpredel, affinity,
                noarrearsletters, defaultdeposit, depositispercentage, isactive,
                defaultterm, agrtexty, agrtextx,
                maxterm, minterm,dtnetfirstin, depositpaid, countrycode, agrtext, apr,
                CashBackMonth, CashBackPc, CashBackAmount,
                AgreementPrint, DeferredMonths,
                FullRebateDays, STCPc, STCAmount, delnonstocks, DoNotSecuritise,StoreType, IsLoan, --CR906, PClubTier1, PClubTier2)
                LoanNewCustomer, LoanRecentCustomer, LoanExistingCustomer, LoanStaff, isAmortized, IsMmiActive, MmiThresholdPercentage)
        VALUES (@termstype, @description, @mthsintfree, @instalpredel, @affinity,
                @noarrearsletter, @defaultdeposit, @depositispercentage, @isactive,
                @defaultterm,0,0,
                @maxterm, @minterm, 'N', 'N', @countrycode, @agrtext, @apr,
                @CashBackMonth, @CashBackPc, @CashBackAmount,
                @AgreementPrint, @DeferredMonths,
                @FullRebateDays, @STCPc, @STCAmount, @delnonstocks, @donotsecuritise,@storeType, @isLoan, --CR906, @pClubTier1, @pClubTier2)
                @loanNewCustomer, @loanRecentCustomer, @loanExistingCustomer, @loanStaff, @isAmortized, @IsMmiActive, @MmiThresholdPercentage)
    END

    IF (@@error != 0)
    BEGIN
        SET @return = @@error
    END

GO


