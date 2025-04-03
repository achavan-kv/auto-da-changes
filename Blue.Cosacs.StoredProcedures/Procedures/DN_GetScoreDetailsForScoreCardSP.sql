
IF EXISTS (SELECT * FROM sysobjects 
		   WHERE NAME = 'DN_GetScoreDetailsForScoreCardSP'
		   AND xtype = 'p')
BEGIN
	DROP PROCEDURE DN_GetScoreDetailsForScoreCardSP
END
GO


CREATE procedure [dbo].[DN_GetScoreDetailsForScoreCardSP]
    @AcctNo                             VARCHAR(12),
    @Return                             INT     OUT        
AS
DECLARE

    -- Local variables
    @accttype                           VARCHAR(2),
    @addresstime                        INT,
    @agreement                          DECIMAL(20,5),
    @Arrears                            DECIMAL(20,5),
    @bankaccttype                       VARCHAR(2),
    @bankBankname                       VARCHAR(20),
    @Bankruptcies                       INT,
    @Bankruptcies12Months               INT,
    @Bankruptcies24months               INT,
    @BankruptciesAvgValue               DECIMAL(20,5),
    @Bankruptciestimesincelast          INT,
    @BankruptciesTotalValue             DECIMAL(20,5),
    @CurHiEver                          CHAR(1),
    @CurHiNow                           CHAR(1),
    @CurNumAcc                          DECIMAL(20,5),
    @CurRecent                          CHAR(1),
    @custaddresspocode                  VARCHAR(10),
    @custaddressResstatus               VARCHAR(2),
    @customerAge                        INT,
    @customerEthnicity                  CHAR(1),
    @customerTitle                      VARCHAR(25),
    @custtelarea                        VARCHAR(6),
    @custtelTelno                       VARCHAR(20),
    @DateLastAcceptedGRMaxCurrent       CHAR(1),
    @DateLastAcceptedGRMaxSettled       CHAR(1),
    @DepPercent                         DECIMAL(20,5),
    @employmentEmpmtstatus              CHAR(1),
    @employmentPayfreq                  CHAR(1),
    @EmploymentTime                     INT,
    @expenses                           DECIMAL(20,5),
    @gender                             CHAR(1),
    @hasdelivery                        INT,
    @hashomephone                       CHAR(1),
    @hasworkphone                       CHAR(1),
    @income                             DECIMAL(20,5),
    @IncomePercent                      DECIMAL(20,5),
    @instalno                           INTEGER,
    @InstalPercent                      DECIMAL(20,5),
    @InstalPercentAllAccounts           DECIMAL(20,5),
    @itemcount                          INT,
    @jemploymentEmpmtstatus             VARCHAR (3),
    @jemploymentworktype                VARCHAR(2),
    @Lawsuits                           INT,
    @Lawsuits12Months                   INT,
    @Lawsuits24months                   INT,
    @LawsuitsAvgValue                   DECIMAL(20,5),
    @LawsuitsTotalValue                 DECIMAL(20,5),
    @LawsuitTimeSinceLast               INT,
    @LoanAmt                            DECIMAL(20,5),
    @mobilephone                        CHAR(1),
    @monthssincedelivery                DECIMAL(20,5),
    @NumAppsLst90                       DECIMAL(20,5),
    @paddresstime                       INT,
    @paymentmethod                      VARCHAR(5),
    @PreviousEnquiries                  INT,
    @PreviousEnquiries12Months          INT,
    @PreviousEnquiriesAvgValue          DECIMAL(20,5),
    @PreviousEnquiriesAvgValue12Months  DECIMAL(20,5),
    @PreviousEnquiriesTotalValue        DECIMAL(20,5),
    @privilege                          CHAR(1),
    @ProdCat                            INT,
    @ProdCode                           VARCHAR(8),
    @proposalDependants                 INT,
    @proposalMaritalstat                CHAR(1),
    @proposalnationality                VARCHAR (2),
    @proptype                           CHAR(4),
    @RFLimit                            DECIMAL(20,5),
    @rindicset                          CHAR(1),
    @score                              INT,
    @SetHiEver                          CHAR(1),
    @SetHiNow                           CHAR(1),
    @SetLargest                         CHAR(1),
    @SetLargestSize                     CHAR(1),
    @SetNumAcc                          INT,
    @SetRecent                          CHAR(1),
    @termstype                          VARCHAR(2),
    @timecurrbank                       INTEGER,
    @totalliability                     DECIMAL(20,5),
    @TotValue                           DECIMAL(20,5),
    @worktype                           CHAR(2),
    @monthssinceaccepted                DECIMAL(20,5),
	@TransportType						TTransportType,  --CR 866    
	@EducationLevel						TEducationLevel,	--CR 866    		
	@DistanceFromStore					INT,	--CR 866                        
	@Industry							TIndustry,	--CR 866
	@JobTitle							TJobTitle, --CR 866     
	@Organisation						TOrganisation, --CR 866    
	@TelAreaCat						    VARCHAR(128),-- SC 71262 5/06/2009 TelCodeCat 
--CR 1034 Behavioural scoring parameters
    @numacctsarrears					SMALLINT	,
    @numactiveaccts					SMALLINT		,
    @worstintsarrslast6months	 DECIMAL(20,5)  ,
    @arrearstotalPercent3months	 DECIMAL(20,5)  ,
    @arrearstotalPercent9months	 DECIMAL(20,5)  ,
    @balanceTotalPercent3months  DECIMAL(20,5)  ,  
    @balanceTotalPercent9months  DECIMAL(20,5)  , 
    @monthssincelastGr1inarrears DECIMAL(20,5)  , 
    @monthssincelastGr2inarrears DECIMAL(20,5)  ,
    @worstcurrentstatusChangelast9Months   DECIMAL(20,5), 
    @worstcurrentstatusChangelast12Months   DECIMAL(20,5),
    @agreementTotAccts	DECIMAL(20,5),				--IP - 30/10/12 - #11535  
    @outstbalTotAccts DECIMAL(20,5)					--IP - 30/10/12 - #11536 	 
--CR 1034 End of Behavioural scoring parameters


    SET @Return = 0
    
    declare @custid varchar(20)
    SELECT @custid = custid FROM custacct WHERE acctno = @acctno 
    AND hldorjnt = 'H'


    EXECUTE DN_GetScoreDetailsByParamSP
        @AcctNo                             = @AcctNo,
        @Return                             = @Return                               OUTPUT,
        @accttype                           = @accttype                             OUTPUT,  
        @addresstime                        = @addresstime                          OUTPUT,  
        @agreement                          = @agreement                            OUTPUT,  
        @Arrears                            = @Arrears                              OUTPUT,  
        @bankaccttype                       = @bankaccttype                         OUTPUT,  
        @bankBankname                       = @bankBankname                         OUTPUT,  
        @Bankruptcies                       = @Bankruptcies                         OUTPUT,  
        @Bankruptcies12Months               = @Bankruptcies12Months                 OUTPUT,  
        @Bankruptcies24months               = @Bankruptcies24months                 OUTPUT,  
        @BankruptciesAvgValue               = @BankruptciesAvgValue                 OUTPUT,  
        @Bankruptciestimesincelast          = @Bankruptciestimesincelast            OUTPUT,  
        @BankruptciesTotalValue             = @BankruptciesTotalValue               OUTPUT,  
        @CurHiEver                          = @CurHiEver                            OUTPUT,  
        @CurHiNow                           = @CurHiNow                             OUTPUT,  
        @CurNumAcc                          = @CurNumAcc                            OUTPUT,  
        @CurRecent                          = @CurRecent                            OUTPUT,  
        @custaddresspocode                  = @custaddresspocode                    OUTPUT,  
        @custaddressResstatus               = @custaddressResstatus                 OUTPUT,  
        @customerAge                        = @customerAge                          OUTPUT,  
        @customerEthnicity                  = @customerEthnicity                    OUTPUT,  
        @customerTitle                      = @customerTitle                        OUTPUT,  
        @custtelarea                        = @custtelarea                          OUTPUT,  
        @custtelTelno                       = @custtelTelno                         OUTPUT,  
        @DateLastAcceptedGRMaxCurrent       = @DateLastAcceptedGRMaxCurrent         OUTPUT,  
        @DateLastAcceptedGRMaxSettled       = @DateLastAcceptedGRMaxSettled         OUTPUT,  
        @DepPercent                         = @DepPercent                           OUTPUT,  
        @employmentEmpmtstatus              = @employmentEmpmtstatus                OUTPUT,  
        @employmentPayfreq                  = @employmentPayfreq                    OUTPUT,  
        @EmploymentTime                     = @EmploymentTime                       OUTPUT,  
        @expenses                           = @expenses                             OUTPUT,  
        @gender                             = @gender                               OUTPUT,  
        @hasdelivery                        = @hasdelivery                          OUTPUT,  
        @hashomephone                       = @hashomephone                         OUTPUT,  
        @hasworkphone                       = @hasworkphone                         OUTPUT,  
        @income                             = @income                               OUTPUT,  
        @IncomePercent                      = @IncomePercent                        OUTPUT,  
        @instalno                           = @instalno                             OUTPUT,  
        @InstalPercent                      = @InstalPercent                        OUTPUT,  
        @InstalPercentAllAccounts           = @InstalPercentAllAccounts             OUTPUT,  
        @itemcount                          = @itemcount                            OUTPUT,  
        @jemploymentEmpmtstatus             = @jemploymentEmpmtstatus               OUTPUT,  
        @jemploymentworktype                = @jemploymentworktype                  OUTPUT,  
        @Lawsuits                           = @Lawsuits                             OUTPUT,  
        @Lawsuits12Months                   = @Lawsuits12Months                     OUTPUT,  
        @Lawsuits24months                   = @Lawsuits24months                     OUTPUT,  
        @LawsuitsAvgValue                   = @LawsuitsAvgValue                     OUTPUT,  
        @LawsuitsTotalValue                 = @LawsuitsTotalValue                   OUTPUT,  
        @LawsuitTimeSinceLast               = @LawsuitTimeSinceLast                 OUTPUT,  
        @LoanAmt                            = @LoanAmt                              OUTPUT,  
        @mobilephone                        = @mobilephone                          OUTPUT,  
        @monthssincedelivery                = @monthssincedelivery                  OUTPUT,  
        @NumAppsLst90                       = @NumAppsLst90                         OUTPUT,  
        @paddresstime                       = @paddresstime                         OUTPUT,  
        @paymentmethod                      = @paymentmethod                        OUTPUT,  
        @PreviousEnquiries                  = @PreviousEnquiries                    OUTPUT,  
        @PreviousEnquiries12Months          = @PreviousEnquiries12Months            OUTPUT,  
        @PreviousEnquiriesAvgValue          = @PreviousEnquiriesAvgValue            OUTPUT,  
        @PreviousEnquiriesAvgValue12Months  = @PreviousEnquiriesAvgValue12Months    OUTPUT,  
        @PreviousEnquiriesTotalValue        = @PreviousEnquiriesTotalValue          OUTPUT,  
        @privilege                          = @privilege                            OUTPUT,  
        @ProdCat                            = @ProdCat                              OUTPUT,  
        @ProdCode                           = @ProdCode                             OUTPUT,  
        @proposalDependants                 = @proposalDependants                   OUTPUT,  
        @proposalMaritalstat                = @proposalMaritalstat                  OUTPUT,  
        @proposalnationality                = @proposalnationality                  OUTPUT,  
        @proptype                           = @proptype                             OUTPUT,  
        @RFLimit                            = @RFLimit                              OUTPUT,  
        @rindicset                          = @rindicset                            OUTPUT,  
        @score                              = @score                                OUTPUT,  
        @SetHiEver                          = @SetHiEver                            OUTPUT,  
        @SetHiNow                           = @SetHiNow                             OUTPUT,  
        @SetLargest                         = @SetLargest                           OUTPUT,  
        @SetLargestSize                     = @SetLargestSize                       OUTPUT,  
        @SetNumAcc                          = @SetNumAcc                            OUTPUT,  
        @SetRecent                          = @SetRecent                            OUTPUT,  
        @termstype                          = @termstype                            OUTPUT,  
        @timecurrbank                       = @timecurrbank                         OUTPUT,  
        @totalliability                     = @totalliability                       OUTPUT,  
        @TotValue                           = @TotValue                             OUTPUT,  
        @worktype                           = @worktype                             OUTPUT,   
		@monthssinceaccepted				= @monthssinceaccepted					OUTPUT,
		@TransportType						= @TransportType						OUTPUT, --CR 866
		@EducationLevel						= @EducationLevel						OUTPUT, --CR 866
		@DistanceFromStore					= @DistanceFromStore				    OUTPUT, --CR 866
		@Industry							= @Industry								OUTPUT, --CR 866
		@JobTitle							= @JobTitle								OUTPUT, --CR 866
		@Organisation						= @Organisation							OUTPUT, --CR 866
		@TelAreaCat                         = @TelAreaCat						    OUTPUT, -- SC 71262 5/06/2009 TelCodeCat 
		@agreementTotAccts					= @agreementTotAccts					OUTPUT,  --IP - 30/10/12 - #11535  
		@outstbalTotAccts					= @outstbalTotAccts	 
	
    
	SET @Return = @@ERROR
	select
      top 1 @custaddresspocode= ISNULL(cuspocode,deliveryarea)
       from custaddress	
       where (datemoved is null or datemoved =N'1-Jan-1900')
       and custid=@custid and addtype = 'H' order by datechange desc

 SELECT		ISNULL(@customerage,0)                         as age,
            ISNULL(@employmentempmtstatus,null)            as 'employmentstatus_woe',
            ISNULL(@proposalMaritalstat,null)              as 'maritalstatus_woe',
            ISNULL(@proposaldependants,0)                  as 'numberdependents',                          --numeric
            ISNULL(@worktype,null)                         as 'occupation_woe',
            ISNULL(@custaddressresstatus,null)             as 'residentialstatus_woe',
            ISNULL(@addresstime,0)                         as 'timecurrentaddress',
            ISNULL(@employmenttime,0)                      as 'timecurrentemployment', 
            ISNULL(@custaddresspocode,null)                as 'postcode_woe',
            ISNULL(@gender, null)                          as 'gender_woe',
            ISNULL(@mobilephone, null)                     as 'mobilenumber_woe'  --,

DECLARE @CUSTHISTORY INT=17		 
SELECT @CUSTHISTORY=VALUE FROM COUNTRYMAINTENANCE WHERE CODENAME ='BEHAVIOURALMONTHSHISTORY'

--Below sp fetch customer history
EXEC USP_SCDE_CALCULATE_ARREARS_BALANCE_DAYS @CUSTID,@CUSTHISTORY
		
--***********************************************************
GO


