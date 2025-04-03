SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO
/*PC 04/12/06 added additional fields for thailand scoring //CR 866 */
/*AA 13/05/04 applicant 2 income and additional income now included in income*/
-- IP 30/10/12		#11535 - Return the sum of agreement totals of all accounts for the customer
--					excluding the most recent credit account.
-- IP 31/10/12		#11536 - Return the sum of outstanding balance of all accounts for the customer
--					excluding the most recent credit account.

if exists (select * from dbo.sysobjects
           where id = object_id('[dbo].[DN_getscoredetailsSP]') 
           and OBJECTPROPERTY(id, 'IsProcedure') = 1)
    drop procedure [dbo].[DN_getscoredetailsSP]
GO

CREATE procedure DN_GetScoreDetailsSP

    -- Parameters

    @AcctNo                             VARCHAR(12),
    @Return                             INT             OUTPUT
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
    @custaddresspocode                  VARCHAR(6),
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
    
    EXEC BehaveScoreGetParameters
	@numacctsarrears = @numacctsarrears OUT , -- SMALLINT
	@numactiveaccts = @numactiveaccts OUT ,
	@worstintsarrslast6months = @worstintsarrslast6months OUT ,
	@arrearstotalPercent3months = @arrearstotalPercent3months OUT ,
	@arrearstotalPercent9months = @arrearstotalPercent9months OUT ,
	@balanceTotalPercent3months = @balanceTotalPercent3months OUT ,
	@balanceTotalPercent9months = @balanceTotalPercent9months OUT ,
	@monthssincelastGr1inarrears = @monthssincelastGr1inarrears OUT ,
	@monthssincelastGr2inarrears = @monthssincelastGr2inarrears OUT ,
	@worstcurrentstatusChangelast9Months = @worstcurrentstatusChangelast9Months OUT ,
	@worstcurrentstatusChangelast12Months = @worstcurrentstatusChangelast12Months OUT ,
	@custid = @custid

    

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

    SELECT  isnull(@customerage,0)                          as Age,                                         --numeric
            isnull(@bankbankname,'')                       as 'Bank Name',
            isnull(@employmentempmtstatus,'')              as 'Employment Status',
            isnull(@employmentpayfreq,'')                  as 'Frequency of Pay',
            isnull(@custTelTelno,'')                       as 'Home Telephone',
            isnull(@instalpercent,0)                        as 'Instalment/Income ratio',
            isnull(@proposalMaritalstat,'')                as 'Marital Status',
            isnull(@proposaldependants,0)                   as 'No of Dependants',                          --numeric
            isnull(@worktype,'')                           as 'Occupation',
            isnull(@prodcode,'')                           as 'Product Code (highest value items purchased)',
            isnull(@custaddressresstatus,'')               as 'Residential Status',
            isnull(@setrecent,'')                          as 'Statusof most recent settled account',
            isnull(@NumAppsLst90,0)                         as 'No. of applications in the last 90 days',   --numeric
            isnull(@setlargestsize,'')                     as 'Size of largest settled',
            isnull(@addresstime,0)                          as 'Time at current address (mm)',              --numeric
            isnull(@employmenttime,0)                       as 'Time in current employment (mm)',           --numeric
            isnull(@CustomerTitle,'')                      as 'Title',
            isnull(@curhiever,'')                          as 'Worst Current Ever',
            isnull(@sethiever,'')                          as 'Worst Settled Ever',
            isnull(@curhinow,'')                           as 'Worst Current Now',
            isnull(@sethinow,'')                           as 'Worst Settled when settled',
            isnull(@bankaccttype,'')                       as 'Bank account type',
            isnull(@proposalnationality,'')                as 'Nationality',
            isnull(@customerethnicity,'')                  as 'Ethnic Group',
            isnull(@itemcount,0)                            as 'Number of stock items purchased',           --numeric
            isnull(@jemploymentworktype,'')                as 'Spouse Occupation',
            isnull(@custaddresspocode,'')                  as 'Post Code',
            isnull(@paddresstime,0)                         as 'Time at previous address (mm)',             --numeric
            isnull(@setnumacc,0)                            as 'Number of settled accounts',                --numeric
            isnull(@currecent,'')                          as 'Status of most recent current account',
            CAST(isnull(@custtelarea,'0') as int)          as 'Telephone area code',
            isnull(@loanamt,0)                              as 'Total val of items purchased (ex. Service charge)',
            isnull(@totvalue,0)                             as 'Value of stock items',
            isnull(@deppercent,0)                           as 'Deposit percentage',
            isnull(@accttype, '')                          as 'account type',
            isnull(@prodcat, 0)                             as 'Product Category',
            isnull(@hasdelivery, 0)                         as 'Different Delivery Address',
            isnull(@Agreement, 0)                           as 'Agreement Total(- deposit)',
            isnull(@SetLargest,'')                         as 'Status Largest Settled',
            isnull(@instalno, 0)                            as 'Number of Instalments',
            isnull(@termstype, '')                         as 'Termstype',
            isnull(@privilege, '')                         as 'Privilege Club Y/N',
            isnull(@rindicset, '')                         as 'R indicator set Y/N',
            isnull(@totalliability, 0)                      as 'Total Liability',
            isnull(@hashomephone,'N')                      as 'Has home phone Y/N',
            isnull(@hasworkphone,'N')                      as 'Has work phone Y/N',
            isnull(@timecurrbank,0)                         as 'Time at current bank (mm)',
            isnull(@expenses, 0)                            as 'Monthly Expenses(ex accom)',
            isnull(@proptype,'')                           as 'Property Type',
            isnull(@gender, '')                             as 'Gender',
            isnull(@Arrears, 0)                             as 'Customer Arrears',
            isnull(@income, 0)                              as 'Income',
            isnull(@mobilephone, 0)                         as 'Mobile Phone Y/N',
            isnull(@LawsuitTimeSinceLast, 0)                as 'CB_Time Since Lawsuit (mm)',
            isnull(@Lawsuits, 0)                            as 'CB_No. of Lawsuits',
            isnull(@Lawsuits12Months, 0)                    as 'CB_No. of Lawsuits Last 12 Mths',
            isnull(@LawsuitsAvgValue, 0)                    as 'CB_Avg Value Of Lawsuits',
            isnull(@LawsuitsTotalValue, 0)                  as 'CB_Total Value Of Lawsuits',
            isnull(@Bankruptcies, 0)                        as 'CB_No. of Bankruptcies',
            isnull(@Bankruptcies12Months, 0)                as 'CB_No. of Bankruptcies Last 12 Mths',
            isnull(@BankruptciesTotalValue, 0)              as 'CB_Total Value Of Bankruptcies',
            isnull(@BankruptciesAvgValue, 0)                as 'CB_Avg Value Of Bankruptcies',
            isnull(@PreviousEnquiries, 0)                   as 'CB_Previous Enquiries',
            isnull(@PreviousEnquiriesTotalValue, 0)         as 'CB_Total Value Of Prev Enquiries',
            isnull(@PreviousEnquiriesAvgValue, 0)           as 'CB_Avg Value Of Prev Enquiries',
            isnull(@PreviousEnquiries12Months, 0)           as 'CB_Prev Enquiries Last 12 Mths',
            isnull(@PreviousEnquiriesAvgValue12Months, 0)   as 'CB_Avg Value Of Prev Enquiries Last 12 Mths',
            isnull(@Bankruptciestimesincelast, 0)           as 'CB_Time Since Last Bankruptcy (mm)',
            isnull(@Lawsuits24months, 0)                    as 'CB_No. of Lawsuits Last 24 Mths',
            isnull(@Bankruptcies24months, 0)                as 'CB_No. of Bankruptcies Last 24 Mths',
            isnull(@IncomePercent, 0)                       as 'Disposable Income/Income Ratio',
            isnull(@RFLimit, 0)                             as 'RF Spending Limit',             /* dummy column will be populated in the code */
            isnull(@score, 0)                               as 'Score',                         /* dummy column will be populated in the code */
            isnull(@DateLastAcceptedGRMaxCurrent,'')        as 'Date last Accepted > Date Highest Current Y/N',
            isnull(@DateLastAcceptedGRMaxSettled,'')        as 'Date last Accepted > Date Highest Settled Y/N',
            isnull(@jemploymentEmpmtstatus, '')             as 'Spouse Employment Status',
            isnull(@monthssincedelivery, 1000)              as 'Months since last delivery',
            isnull(@paymentmethod, '')                     as 'Pay Method',
            isnull(convert (int,@CurNumAcc),0)              as 'No of Current Accounts',
            isnull(@instalpercentAllAccounts,0)             as 'Instalment/Income ratio All Accounts',
            isnull(@monthssinceaccepted, 0)                 as 'Months since last accepted application',
            isnull(@TransportType, '')						as 'Transport Type'			,	--CR 866
			isnull(@EducationLevel, '')						as 'Education Level'		,	--CR 866
			isnull(@DistanceFromStore, 0)					as 'Distance From Store',		--CR 866
			isnull(@Industry, '')							as 'Industry',					--CR 866
			isnull(@JobTitle, '')							as 'JobTitle',					--CR 866
			isnull(@Organisation, '')						as 'Organisation',				--CR 866
			ISNULL(@TelAreaCat,'')						    AS 'Telephone Area Category',         -- SC 71262 5/06/2009 TelCodeCat 
--CR 1034 Behavioural scoring parameters below
			ISNULL(@numacctsarrears,0)					AS 'No. of A/cs in Arrears',
			ISNULL(@numactiveaccts,0)					AS 'No. active A/cs',
			ISNULL(@worstintsarrslast6months,0)	 AS 'Worst Inst/arrs last 6 months',
			ISNULL(@arrearstotalPercent3months,0) AS 'Arrears Percent Comp 3 Months'	 ,
			ISNULL(@arrearstotalPercent9months,0) AS 'Arrears Percent Comp 9 Months'	 ,
			ISNULL(@balanceTotalPercent3months,0)  AS 'Balance Percent Comp 3 Months',
			ISNULL(@balanceTotalPercent9months,0)  AS 'Balance Percent Comp 9 Months',
			ISNULL(@monthssincelastGr1inarrears,0) AS 'Months Since Gr1 in Arrears',
			ISNULL(@monthssincelastGr2inarrears,0) AS 'Months Since Gr2 in Arrears',
			ISNULL(@worstcurrentstatusChangelast9Months,0) AS 'Worst Current Status Change last 9 Months'  ,
			ISNULL(@worstcurrentstatusChangelast12Months  ,0) AS 'Worst Current Status Change last 12 Months',
			ISNULL(@agreementTotAccts, 0)				AS 'Agreement Total Account',									--IP - 30/10/12 - #11535  
			ISNULL(@outstbalTotAccts, 0)				AS 'Outstanding Balance of Accounts'							--IP - 31/10/12 - #11536  
--CR 1034 End of Behavioural scoring parameters
	
			/*these columns have to match those in the scoringoperand table*/

GO
SET QUOTED_IDENTIFIER OFF
GO
SET ANSI_NULLS ON
GO

